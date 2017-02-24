namespace VisionControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Permissions;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using Heroes;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Units;
    using Units.Mines;
    using Units.Wards;

    internal class VisionControl
    {
        #region Fields

        private readonly List<EnemyHero> enemyHeroes = new List<EnemyHero>();

        private readonly List<IUnit> units = new List<IUnit>();

        private Team enemyTeam;

        private Hero hero;

        private Team heroTeam;

        private MultiSleeper sleeper;

        private Font textFont;

        #endregion

        #region Properties

        private static MenuManager Menu => Variables.Menu;

        #endregion

        #region Public Methods and Operators

        public void DrawingOnEndScene()
        {
            if (Drawing.Direct3DDevice9 == null)
            {
                return;
            }

            foreach (var unit in units.OfType<Ward>())
            {
                if (unit.ShowTexture)
                {
                    var position = Utils.WorldToMiniMap(unit.Position, new Vector2(20, 35));
                    textFont.DrawText(
                        null,
                        "*",
                        (int)position.X,
                        (int)position.Y,
                        unit is SentryWard ? Color.Blue : Color.Yellow);
                }
            }
        }

        public void OnAddEntity(EntityEventArgs args)
        {
            var unit = args.Entity as Unit;

            if (unit == null || !unit.IsValid || !unit.IsAlive || unit.Team == heroTeam)
            {
                return;
            }

            string abilityName;
            if (!Variables.UnitAbilityNames.TryGetValue(unit.Name, out abilityName) || !Menu.IsEnabled(abilityName))
            {
                return;
            }

            Func<Unit, IUnit> func;
            if (Variables.Units.TryGetValue(abilityName, out func))
            {
                if (unit.ClassID == ClassID.CDOTA_NPC_Observer_Ward && UpdateData<ObserverWard>(unit))
                {
                    return;
                }

                if (unit.ClassID == ClassID.CDOTA_NPC_Observer_Ward_TrueSight && UpdateData<SentryWard>(unit))
                {
                    return;
                }

                if (unit.Name == "npc_dota_techies_remote_mine" && UpdateData<RemoteMine>(unit, 20))
                {
                    return;
                }

                units.Add(func.Invoke(unit));
            }
        }

        public void OnClose()
        {
            units.Clear();
            enemyHeroes.Clear();
            Menu.OnClose();
        }

        public void OnDraw()
        {
            foreach (var unit in units)
            {
                Vector2 screenPosition;
                Drawing.WorldToScreen(unit.Position, out screenPosition);

                screenPosition -= unit.PositionCorrection;

                if (unit.ShowTexture)
                {
                    Drawing.DrawRect(screenPosition, unit.TextureSize, unit.Texture);

                    //if (unit is Ward)
                    //{
                    //    Drawing.DrawText(
                    //        "*",
                    //        "Arial",
                    //        Utils.WorldToMiniMap(unit.Position, new Vector2(20, 35)),
                    //        new Vector2(45),
                    //        unit is SentryWard ? Color.Blue : Color.Yellow,
                    //        FontFlags.None);
                    //}
                }

                if (!unit.ShowTimer)
                {
                    continue;
                }

                Drawing.DrawText(
                    TimeSpan.FromSeconds(unit.EndTime - Game.RawGameTime).ToString(@"m\:ss"),
                    "Arial",
                    screenPosition + new Vector2(0, unit.TextureSize.Y),
                    new Vector2(22),
                    Color.White,
                    FontFlags.None);
            }
        }

        public void OnInt32PropertyChange(Entity entity, Int32PropertyChangeEventArgs args)
        {
            if (args.PropertyName == "m_iHealth" && args.NewValue <= 0)
            {
                OnRemoveUnit(entity as Unit);
            }
        }

        public void OnLoad()
        {
            Variables.Menu = new MenuManager();
            hero = ObjectManager.LocalHero;
            enemyTeam = hero.GetEnemyTeam();
            heroTeam = hero.Team;
            sleeper = new MultiSleeper();

            textFont = new Font(
                Drawing.Direct3DDevice9,
                new FontDescription
                {
                    FaceName = "Tahoma",
                    Height = 30,
                    OutputPrecision = FontPrecision.Raster,
                    Quality = FontQuality.ClearTypeNatural,
                    CharacterSet = FontCharacterSet.Hangul,
                    MipLevels = 3,
                    PitchAndFamily = FontPitchAndFamily.Modern,
                    Weight = FontWeight.Heavy,
                    Width = 12
                });
        }

        public void OnParticleEffectAdded(Entity sender, ParticleEffectAddedEventArgs args)
        {
            DelayAction.Add(
                1000,
                () => {
                    if (args.Name.Contains("techies_remote_mine_plant"))
                    {
                        if (!Menu.IsEnabled("techies_remote_mines") || sender.Team == heroTeam)
                        {
                            return;
                        }

                        var position = args.ParticleEffect.GetControlPoint(1);

                        if (position.IsZero || units.Any(x => x is RemoteMine && x.Position.Distance2D(position) < 10))
                        {
                            return;
                        }

                        units.Add(new RemoteMine(position));
                    }

                    if (args.Name.Contains("techies_remote_mines_detonate"))
                    {
                        var remote =
                            units.FirstOrDefault(
                                x =>
                                    x is RemoteMine
                                    && x.Position.Distance2D(args.ParticleEffect.GetControlPoint(0)) < 10);

                        if (remote != null)
                        {
                            remote.ParticleEffect?.Dispose();
                            units.Remove(remote);
                        }
                    }
                });
        }

        public void OnPostReset()
        {
            textFont.OnResetDevice();
        }

        public void OnPreReset()
        {
            textFont.OnLostDevice();
        }

        public void OnRemoveUnit(Unit unit)
        {
            if (unit == null)
            {
                return;
            }

            var dead = units.FirstOrDefault(x => x.Handle == unit.Handle);

            if (dead == null)
            {
                return;
            }

            dead.ParticleEffect?.Dispose();
            units.Remove(dead);
        }

        public void OnUpdate()
        {
            if (sleeper.Sleeping(this))
            {
                return;
            }

            if (!sleeper.Sleeping(enemyHeroes))
            {
                foreach (var enemy in
                    Ensage.Common.Objects.Heroes.GetByTeam(enemyTeam)
                        .Where(x => x.IsValid && !x.IsIllusion && !enemyHeroes.Exists(z => z.Handle == x.Handle)))
                {
                    enemyHeroes.Add(new EnemyHero(enemy));
                }
                sleeper.Sleep(2000, enemyHeroes);
            }

            foreach (var enemy in enemyHeroes)
            {
                if (!enemy.IsVisible)
                {
                    enemy.ObserversCount = 0;
                    enemy.SentryCount = 0;
                    continue;
                }

                if (PlacedWard(enemy, ClassID.CDOTA_Item_ObserverWard))
                {
                    AddWard<ObserverWard>(enemy);
                }

                if (PlacedWard(enemy, ClassID.CDOTA_Item_SentryWard))
                {
                    AddWard<SentryWard>(enemy);
                }
            }

            if (!sleeper.Sleeping(units))
            {
                var removeUnits = units.Where(x => x.Duration > 0 && x.EndTime <= Game.RawGameTime).ToList();
                if (removeUnits.Any())
                {
                    removeUnits.ForEach(x => x.ParticleEffect?.Dispose());
                    units.RemoveAll(x => removeUnits.Contains(x));
                }
                sleeper.Sleep(1000, units);
            }

            sleeper.Sleep(100, this);
        }

        #endregion

        #region Methods

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        private void AddWard<T>(EnemyHero enemy) where T : Ward
        {
            var wardPosition = enemy.WardPosition();
            var wardExists =
                units.OfType<T>().Any(x => !x.RequiresUpdate && x.Distance(wardPosition) <= 400 && enemy.Angle(x) < 0.5);

            if (wardExists)
            {
                return;
            }

            var shiftPosition = units.OfType<Ward>().Any(x => x.Distance(wardPosition) <= 50);

            if (shiftPosition)
            {
                wardPosition += new Vector3(60, 0, 0);
            }

            units.Add((T)Activator.CreateInstance(typeof(T), wardPosition));
        }

        private bool GaveWard(EnemyHero enemy)
        {
            return
                enemyHeroes.Any(
                    x =>
                        x.IsValid && !x.Equals(enemy) && x.IsAlive && x.Distance(enemy) <= 600
                        && x.ObserversCount + x.SentryCount < x.CountObservers() + x.CountSentries());
        }

        private bool PlacedWard(EnemyHero enemy, ClassID id)
        {
            var count = enemy.CountWards(id);

            if (count < enemy.GetWardsCount(id))
            {
                enemy.SetWardsCount(id, count);

                if (Menu.IsEnabled(id) && !GaveWard(enemy) && !enemy.DroppedWard(id))
                {
                    return true;
                }
            }
            else if (count > enemy.GetWardsCount(id) && !TookWard(enemy))
            {
                enemy.SetWardsCount(id, count);
            }

            return false;
        }

        private bool TookWard(EnemyHero enemy)
        {
            return
                enemyHeroes.Any(
                    x =>
                        x.IsValid && !x.Equals(enemy) && x.IsAlive && x.Distance(enemy) <= 600
                        && x.ObserversCount + x.SentryCount > x.CountObservers() + x.CountSentries());
        }

        private bool UpdateData<T>(Unit unit, float maxDistance = 400) where T : IUpdatable
        {
            if (unit == null || !unit.IsValid)
            {
                return false;
            }

            var updatable = units.OfType<T>().FirstOrDefault(x => x.Distance(unit) < maxDistance && x.RequiresUpdate);

            if (updatable == null)
            {
                return false;
            }

            updatable.UpdateData(unit);
            return true;
        }

        #endregion
    }
}