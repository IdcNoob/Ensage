namespace Timbersaw
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Objects;

    using SharpDX;

    // copy pasta from Ensage.Common with few modifications

    internal static class TimberPrediction
    {
        #region Static Fields

        private static readonly Dictionary<float, float> LastRotRDictionary = new Dictionary<float, float>();

        private static readonly Dictionary<float, double> RotSpeedDictionary = new Dictionary<float, double>();

        private static readonly Dictionary<float, float> RotTimeDictionary = new Dictionary<float, float>();

        private static readonly Dictionary<float, Vector3> SpeedDictionary = new Dictionary<float, Vector3>();

        private static readonly List<Prediction> TrackTable = new List<Prediction>();

        private static List<Hero> playerList = new List<Hero>();

        #endregion

        #region Public Methods and Operators

        public static bool IsIdle(Unit unit)
        {
            return unit.NetworkActivity != NetworkActivity.Move;
        }

        public static void OnClose()
        {
            Events.OnUpdate -= SpeedTrack;
            LastRotRDictionary.Clear();
            RotSpeedDictionary.Clear();
            RotTimeDictionary.Clear();
            playerList.Clear();
            TrackTable.Clear();
            SpeedDictionary.Clear();
        }

        public static void OnLoad()
        {
            Events.OnUpdate += SpeedTrack;
        }

        public static Vector3 PredictedXYZ(Target target, float delay)
        {
            if (IsIdle(target.Hero))
            {
                return target.Position;
            }

            var targetSpeed = new Vector3();
            if (!LastRotRDictionary.ContainsKey(target.Handle))
            {
                LastRotRDictionary.Add(target.Handle, target.RotationRad);
            }

            var straightTime = StraightTime(target.Hero);
            if (straightTime > 180)
            {
                LastRotRDictionary[target.Handle] = target.RotationRad;
            }

            LastRotRDictionary[target.Handle] = target.RotationRad;
            if ((target.ClassID == ClassID.CDOTA_Unit_Hero_StormSpirit
                 || target.ClassID == ClassID.CDOTA_Unit_Hero_Rubick)
                && target.HasModifier("modifier_storm_spirit_ball_lightning"))
            {
                var ballLightning = target.FindSpell("storm_spirit_ball_lightning", true);
                var firstOrDefault =
                    ballLightning.AbilitySpecialData.FirstOrDefault(x => x.Name == "ball_lightning_move_speed");
                if (firstOrDefault != null)
                {
                    var ballSpeed = firstOrDefault.GetValue(ballLightning.Level - 1);
                    var newpredict = target.Vector3FromPolarAngle() * (ballSpeed / 1000f);
                    targetSpeed = newpredict;
                }
            }
            else
            {
                targetSpeed = target.Vector3FromPolarAngle() * (target.MovementSpeed / 1000f);
            }

            var v = target.GetPosition() + targetSpeed * delay;
            return new Vector3(v.X, v.Y, 0);
        }

        public static void SpeedTrack(EventArgs args)
        {
            if (!Utils.SleepCheck("TimberPrediction.SpeedTrack.Sleep"))
            {
                return;
            }

            if (playerList == null || (playerList.Count < 10 && Utils.SleepCheck("TimberPrediction.SpeedTrack")))
            {
                playerList = Heroes.All;
                Utils.Sleep(1000, "TimberPrediction.SpeedTrack");
            }

            if (!playerList.Any())
            {
                return;
            }

            Utils.Sleep(70, "TimberPrediction.SpeedTrack.Sleep");
            var tick = Environment.TickCount & int.MaxValue;
            var tempTable = new List<Prediction>(TrackTable);
            foreach (var unit in playerList.Where(x => x.IsValid))
            {
                var data =
                    tempTable.FirstOrDefault(
                        unitData => unitData.UnitName == unit.StoredName() || unitData.UnitClassId == unit.ClassID);
                if (data == null && unit.IsAlive && unit.IsVisible)
                {
                    data = new Prediction(
                        unit.StoredName(),
                        unit.ClassID,
                        new Vector3(0, 0, 0),
                        0,
                        new Vector3(0, 0, 0),
                        0,
                        0);
                    TrackTable.Add(data);
                }

                if (data != null && (!unit.IsAlive || !unit.IsVisible))
                {
                    data.LastPosition = new Vector3(0, 0, 0);
                    data.LastRotR = 0;
                    data.Lasttick = 0;
                    continue;
                }

                if (data == null || (data.LastPosition != new Vector3(0, 0, 0) && !(tick - data.Lasttick > 0)))
                {
                    continue;
                }

                if (data.LastPosition == new Vector3(0, 0, 0))
                {
                    data.LastPosition = unit.Position;
                    data.LastRotR = unit.RotationRad;
                    data.Lasttick = tick;
                }
                else
                {
                    data.RotSpeed = data.LastRotR - unit.RotationRad;
                    if (!RotTimeDictionary.ContainsKey(unit.Handle))
                    {
                        RotTimeDictionary.Add(unit.Handle, tick);
                    }

                    if (!LastRotRDictionary.ContainsKey(unit.Handle))
                    {
                        LastRotRDictionary.Add(unit.Handle, unit.RotationRad);
                    }

                    var speed = (unit.Position - data.LastPosition) / (tick - data.Lasttick);
                    if (Math.Abs(data.RotSpeed) > 0.18 && data.Speed != new Vector3(0, 0, 0))
                    {
                        data.Speed = speed;
                        RotTimeDictionary[unit.Handle] = tick;
                    }
                    else
                    {
                        LastRotRDictionary[unit.Handle] = unit.RotationRad;
                        data.Speed = speed;
                    }

                    data.LastPosition = unit.Position;
                    data.LastRotR = unit.RotationRad;
                    data.Lasttick = tick;
                    if (!SpeedDictionary.ContainsKey(unit.Handle))
                    {
                        SpeedDictionary.Add(unit.Handle, data.Speed);
                    }
                    else
                    {
                        SpeedDictionary[unit.Handle] = data.Speed;
                    }

                    if (!RotSpeedDictionary.ContainsKey(unit.Handle))
                    {
                        RotSpeedDictionary.Add(unit.Handle, data.RotSpeed);
                    }
                    else
                    {
                        RotSpeedDictionary[unit.Handle] = data.RotSpeed;
                    }
                }
            }
        }

        public static float StraightTime(Unit unit)
        {
            if (!RotTimeDictionary.ContainsKey(unit.Handle))
            {
                return 0;
            }

            return (Environment.TickCount & int.MaxValue) - RotTimeDictionary[unit.Handle] + Game.Ping;
        }

        #endregion
    }
}