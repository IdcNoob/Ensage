namespace Evader.Common
{
    using System;

    using Ensage;
    using Ensage.Common.Extensions;

    using SharpDX;

    internal static class Utils
    {
        #region Public Methods and Operators

        public static Vector3 GetBlinkPosition(this Unit unit, Vector3 direction, float remainingTime)
        {
            remainingTime = Math.Max(remainingTime, 0);

            var turnRate = unit.GetTurnRate();
            var angle = (float)(remainingTime * turnRate / 0.03);

            var left = VectorExtensions.FromPolarCoordinates(1, unit.NetworkRotationRad + angle).ToVector3();
            var right = VectorExtensions.FromPolarCoordinates(1, unit.NetworkRotationRad - angle).ToVector3();

            return unit.Position + (left.Distance2D(direction) < right.Distance2D(direction) ? left : right) * 100;
        }

        public static string GetName(this Unit unit)
        {
            var hero = unit as Hero;
            if (hero != null)
            {
                return hero.GetRealName();
            }

            switch (unit.Name)
            {
                case "npc_dota_neutral_centaur_khan":
                    return "Centaur Conqueror";
                case "npc_dota_neutral_satyr_hellcaller":
                    return "Satyr Tormenter";
                case "npc_dota_brewmaster_earth_1":
                case "npc_dota_brewmaster_earth_2":
                case "npc_dota_brewmaster_earth_3":
                    return "Earth Spirit";
                case "npc_dota_brewmaster_storm_1":
                case "npc_dota_brewmaster_storm_2":
                case "npc_dota_brewmaster_storm_3":
                    return "Storm Spirit";
                case "npc_dota_lone_druid_bear1":
                case "npc_dota_lone_druid_bear2":
                case "npc_dota_lone_druid_bear3":
                case "npc_dota_lone_druid_bear4":
                    return "Spirit Bear";
            }

            Debugger.WriteLine("Real name not found for: " + unit.Name);
            return string.Empty;
        }

        public static float GetRealCastRange(this Ability ability)
        {
            var castRange = ability.GetCastRange();

            if (!ability.IsAbilityBehavior(AbilityBehavior.NoTarget))
            {
                castRange += Math.Max(castRange / 9, 80);
            }
            else
            {
                castRange += Math.Max(castRange / 7, 40);
            }

            return castRange;
        }

        public static bool IsRuptured(this Unit unit)
        {
            return unit.HasModifier("modifier_bloodseeker_rupture");
        }

        public static bool IsTurning(this Unit unit)
        {
            return (int)unit.RotationDifference != 0;
        }

        #endregion
    }
}