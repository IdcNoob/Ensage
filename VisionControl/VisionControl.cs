namespace VisionControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using global::VisionControl.Heroes;
    using global::VisionControl.Units;
    using global::VisionControl.Units.Wards;

    using SharpDX;

    internal class VisionControl
    {
        #region Fields

        private readonly List<EnemyHero> enemyHeroes = new List<EnemyHero>();

        private readonly List<IUnit> units = new List<IUnit>();

        private Team enemyTeam;

        private Hero hero;

        private Team heroTeam;

        private MultiSleeper sleeper;

        #endregion

        #region Properties

        private static MenuManager Menu => Variables.Menu;

        #endregion

        #region Public Methods and Operators

        public void OnAddEntity(EntityEventArgs args)
        {
            DelayAction.Add(
                500f,
                () =>
                    {
                        var unit = args.Entity as Unit;

                        if (unit == null || !unit.IsValid || !unit.IsAlive || unit.Team == heroTeam || !Game.IsInGame)
                        {
                            return;
                        }

                        string abilityName;
                        if (!Variables.UnitAbilityNames.TryGetValue(unit.Name, out abilityName)
                            || !Menu.IsEnabled(abilityName))
                        {
                            return;
                        }

                        Func<Unit, IUnit> func;
                        if (Variables.Units.TryGetValue(abilityName, out func))
                        {
                            if (unit.ClassID == ClassID.CDOTA_NPC_Observer_Ward && UpdateWardData<ObserverWard>(unit)
                                || unit.ClassID == ClassID.CDOTA_NPC_Observer_Ward_TrueSight
                                && UpdateWardData<SentryWard>(unit))
                            {
                                return;
                            }

                            units.Add(func.Invoke(unit));
                        }
                    });
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

                    if (unit is Ward)
                    {
                        Drawing.DrawText(
                            "*",
                            "Arial",
                            Utils.WorldToMiniMap(unit.Position, new Vector2(20, 35)),
                            new Vector2(45),
                            unit is SentryWard ? Color.Blue : Color.Yellow,
                            FontFlags.None);
                    }
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
        }

        public void OnRemoveUnit(Unit unit)
        {
            if (unit == null)
            {
                return;
            }

            var dead = units.FirstOrDefault(x => x.Handle == unit.Handle);

            if (dead != null)
            {
                dead.ParticleEffect?.Dispose();
                units.Remove(dead);
            }
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
                        .Where(x => !x.IsIllusion && !enemyHeroes.Exists(z => z.Handle == x.Handle)))
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

                var newObservers = enemy.CountObservers();
                var newSentries = enemy.CountSentries();

                if (newObservers < enemy.ObserversCount)
                {
                    if (Menu.IsEnabled("item_ward_observer") && !GaveWard(enemy)
                        && !enemy.DroppedWard(ClassID.CDOTA_Item_ObserverWard))
                    {
                        units.Add(new ObserverWard(enemy.WardPosition()));
                    }
                    enemy.ObserversCount = newObservers;
                }
                else if (newObservers > enemy.ObserversCount && !TookWard(enemy))
                {
                    enemy.ObserversCount = newObservers;
                }

                if (newSentries < enemy.SentryCount)
                {
                    if (Menu.IsEnabled("item_ward_sentry") && !GaveWard(enemy)
                        && !enemy.DroppedWard(ClassID.CDOTA_Item_SentryWard))
                    {
                        units.Add(new SentryWard(enemy.WardPosition()));
                    }
                    enemy.SentryCount = newSentries;
                }
                else if (newSentries > enemy.SentryCount && !TookWard(enemy))
                {
                    enemy.SentryCount = newSentries;
                }
            }

            var removeUnits = units.Where(x => x.Duration > 0 && x.EndTime <= Game.RawGameTime || !x.IsValid).ToList();
            removeUnits.ForEach(x => x.ParticleEffect?.Dispose());
            units.RemoveAll(x => removeUnits.Contains(x));

            sleeper.Sleep(333, this);
        }

        #endregion

        #region Methods

        private bool GaveWard(EnemyHero enemy)
        {
            return
                enemyHeroes.Any(
                    x =>
                    !x.Equals(enemy) && x.IsAlive && x.Distance(enemy) <= 600
                    && x.ObserversCount + x.SentryCount < x.CountObservers() + x.CountSentries());
        }

        private bool TookWard(EnemyHero enemy)
        {
            return
                enemyHeroes.Any(
                    x =>
                    !x.Equals(enemy) && x.IsAlive && x.Distance(enemy) <= 600
                    && x.ObserversCount + x.SentryCount > x.CountObservers() + x.CountSentries());
        }

        private bool UpdateWardData<T>(Unit unit) where T : Ward
        {
            var ward = units.OfType<T>().FirstOrDefault(x => x.Distance(unit) < 500 && x.RequiresUpdate);

            if (ward == null)
            {
                return false;
            }

            ward.UpdateData(unit);
            return true;
        }

        #endregion
    }
}