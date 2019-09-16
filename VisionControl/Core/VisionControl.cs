namespace VisionControl.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;

    using Attributes;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Renderer;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using Heroes;

    using SharpDX;

    using Units;
    using Units.Interfaces;
    using Units.Mines;
    using Units.Wards;

    [ExportPlugin("Vision Control", StartupMode.Auto, "IdcNoob")]
    internal class VisionControl : Plugin
    {
        private readonly List<EnemyHero> enemyHeroes = new List<EnemyHero>();

        private readonly Unit hero;

        private readonly Team myTeam;

        private readonly IRenderManager render;

        private readonly List<IUnit> units = new List<IUnit>();

        private readonly Dictionary<Type, UnitAttribute> unitTypes;

        private float lastPingTime;

        private ParticleEffect mapPingParticleEffect;

        private Settings settings;

        [ImportingConstructor]
        public VisionControl([Import] IServiceContext context)
        {
            hero = context.Owner;
            render = context.RenderManager;
            myTeam = context.Owner.Team;

            unitTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract && typeof(BaseUnit).IsAssignableFrom(x))
                .ToDictionary(x => x, x => x.GetCustomAttribute<UnitAttribute>());
        }

        protected override void OnActivate()
        {
            settings = new Settings();

            render.Draw += OnMinimapDraw;
            Drawing.OnDraw += OnMapDraw;
            EntityManager<Unit>.EntityAdded += OnUnitAdded;
            EntityManager<Unit>.EntityRemoved += OnUnitRemoved;
            EntityManager<Hero>.EntityAdded += OnHeroAdded;
            Entity.OnInt32PropertyChange += OnInt32PropertyChange;
            Entity.OnParticleEffectAdded += OnParticleEffectAdded;
            UpdateManager.Subscribe(TimedRemove, 1000);
            UpdateManager.Subscribe(OnUpdate, 100);
            lastPingTime = 0;

            foreach (var hero in EntityManager<Hero>.Entities)
            {
                OnHeroAdded(null, hero);
            }
        }

        protected override void OnDeactivate()
        {
            render.Draw -= OnMinimapDraw;
            Drawing.OnDraw -= OnMapDraw;
            EntityManager<Unit>.EntityAdded -= OnUnitAdded;
            EntityManager<Unit>.EntityRemoved -= OnUnitRemoved;
            EntityManager<Hero>.EntityAdded -= OnHeroAdded;
            Entity.OnInt32PropertyChange -= OnInt32PropertyChange;
            Entity.OnParticleEffectAdded -= OnParticleEffectAdded;
            UpdateManager.Unsubscribe(TimedRemove);
            UpdateManager.Unsubscribe(OnUpdate);

            units.Clear();
            enemyHeroes.Clear();
            settings.Dispose();
        }

        private void AddWard<T>(EnemyHero enemy)
            where T : IWard
        {
            var wardPosition = enemy.WardPosition();

            if (units.OfType<T>()
                .Any(
                    x => !x.RequiresUpdate && Game.RawGameTime - x.CreateTime < 1 && x.Distance(wardPosition) <= x.UpdatableDistance
                         && enemy.Angle(x) < 0.5))
            {
                return;
            }

            var shiftPosition = units.OfType<IWard>().Any(x => x.Distance(wardPosition) <= 50);
            if (shiftPosition)
            {
                wardPosition += new Vector3(60, 0, 0);
            }

            var ward = (IWard)Activator.CreateInstance(typeof(T), wardPosition, settings);
            units.Add(ward);

            if (settings.PingWards)
            {
                UpdateManager.BeginInvoke(() => PingWard(ward), 1000);
            }
        }

        private bool DataUpdated(Unit unit, string abilityName)
        {
            var updatable = units.OfType<IUpdatable>()
                .FirstOrDefault(x => x.RequiresUpdate && x.AbilityName == abilityName && x.Distance(unit.Position) < x.UpdatableDistance);

            if (updatable == null)
            {
                return false;
            }

            updatable.UpdateData(unit);
            return true;
        }

        private bool GaveWard(EnemyHero enemy)
        {
            return enemyHeroes.Any(
                x => x.IsValid && !x.Equals(enemy) && x.IsVisible && x.IsAlive && x.Distance(enemy) <= 600
                     && x.ObserversCount + x.SentryCount
                     < x.CountWards(AbilityId.item_ward_observer) + x.CountWards(AbilityId.item_ward_sentry));
        }

        private void OnHeroAdded(object sender, Hero hero)
        {
            if (!hero.IsValid || hero.IsIllusion || hero.Team == myTeam || enemyHeroes.Any(x => x.Handle == hero.Handle))
            {
                return;
            }

            enemyHeroes.Add(new EnemyHero(hero));
        }

        private void OnInt32PropertyChange(Entity entity, Int32PropertyChangeEventArgs args)
        {
            if (args.NewValue > 0 || args.NewValue == args.OldValue || args.PropertyName != "m_iHealth")
            {
                return;
            }

            OnUnitRemoved(null, entity as Unit);
        }

        private void OnMapDraw(EventArgs args)
        {
            var remotes = units.OfType<RemoteMine>().ToList();
            foreach (var stack in remotes
                .GroupBy(x => remotes.Where(z => z.Distance(x) < 50).Aggregate(new Vector3(), (sum, mine) => sum + mine.Position))
                .Where(x => x.Count() > 1))
            {
                var screenPosition = Drawing.WorldToScreen(stack.Key / stack.Count());
                if (screenPosition.IsZero)
                {
                    continue;
                }

                Drawing.DrawText(
                    "x" + stack.Count(),
                    "Arial",
                    screenPosition + new Vector2(30, -20),
                    new Vector2(22),
                    Color.White,
                    FontFlags.None);
            }

            foreach (var unit in units)
            {
                var screenPosition = Drawing.WorldToScreen(unit.Position);
                if (screenPosition.IsZero)
                {
                    continue;
                }

                screenPosition -= unit.PositionCorrection;

                if (unit.ShowTexture)
                {
                    Drawing.DrawRect(screenPosition, unit.TextureSize, unit.Texture);
                }

                if (unit.ShowTimer)
                {
                    Drawing.DrawText(
                        TimeSpan.FromSeconds(unit.EndTime - Game.RawGameTime).ToString(@"m\:ss"),
                        "Arial",
                        screenPosition + new Vector2(0, unit.TextureSize.Y),
                        new Vector2(22),
                        Color.White,
                        FontFlags.None);
                }
            }
        }

        private void OnMinimapDraw(IRenderer renderer)
        {
            foreach (var ward in units.OfType<IWard>().ToList())
            {
                if (ward.ShowTexture)   
                {
                    renderer.DrawText(ward.Position.WorldToMinimap() - new Vector2(8, 15), "*", ward.Color, 30, "Arial");
                }
            }
        }

        private void OnParticleEffectAdded(Entity sender, ParticleEffectAddedEventArgs args)
        {
            UpdateManager.BeginInvoke(
                () =>
                    {
                        if (args.Name.Contains("techies_remote_mine_plant"))
                        {
                            const string RemoteMinesAbilityName = "techies_remote_mines";
                            if (sender.Team == myTeam || !settings.IsEnabled(RemoteMinesAbilityName))
                            {
                                return;
                            }

                            var position = args.ParticleEffect.GetControlPoint(1);
                            if (position.IsZero || units.OfType<RemoteMine>()
                                    .Any(x => Game.RawGameTime - x.CreateTime <= 1.5 && x.Position.Distance2D(position) < 10))
                            {
                                return;
                            }

                            units.Add(new RemoteMine(position, settings));
                        }
                        else if (args.Name.Contains("techies_remote_mines_detonate"))
                        {
                            var remote = units.OfType<RemoteMine>()
                                .FirstOrDefault(x => x.Position.Distance2D(args.ParticleEffect.GetControlPoint(0)) < 10);

                            if (remote != null)
                            {
                                remote.ParticleEffect?.Dispose();
                                units.Remove(remote);
                            }
                        }
                    },
                1000);
        }

        private void OnUnitAdded(object sender, Unit unit)
        {
            if (!unit.IsValid || !unit.IsAlive || unit.Team == myTeam)
            {
                return;
            }

            var unitName = unit.Name;
            var unitType = unitTypes.FirstOrDefault(x => x.Value.UnitNames.Contains(unitName));

            if (unitType.Key == null)
            {
                return;
            }

            var abilityName = unitType.Value.AbilityName;
            if (!settings.IsEnabled(abilityName))
            {
                return;
            }

            if (typeof(IUpdatable).IsAssignableFrom(unitType.Key))
            {
                if (DataUpdated(unit, abilityName))
                {
                    return;
                }
            }

            units.Add((IUnit)Activator.CreateInstance(unitType.Key, unit, settings));
        }

        private void OnUnitRemoved(object sender, Unit unit)
        {
            if (unit == null || !unit.IsValid || unit.Team == myTeam)
            {
                return;
            }

            var deadUnit = units.FirstOrDefault(x => x.Handle == unit.Handle);
            if (deadUnit == null)
            {
                return;
            }

            deadUnit.ParticleEffect?.Dispose();
            units.Remove(deadUnit);
        }

        private void OnUpdate()
        {
            foreach (var enemy in enemyHeroes.Where(x => x.IsValid))
            {
                if (!enemy.IsVisible)
                {
                    enemy.ObserversCount = 0;
                    enemy.SentryCount = 0;
                    continue;
                }

                if (PlacedWard(enemy, AbilityId.item_ward_observer))
                {
                    AddWard<ObserverWard>(enemy);
                }
                else if (PlacedWard(enemy, AbilityId.item_ward_sentry))
                {
                    AddWard<SentryWard>(enemy);
                }
            }
        }

        private void PingWard(IWard ward)
        {
            if (!ward.RequiresUpdate || lastPingTime + 10 > Game.RawGameTime)
            {
                return;
            }

            lastPingTime = Game.RawGameTime;

            Network.MapPing(ward.Position.ToVector2(), (PingType)5);

            // client ping
            mapPingParticleEffect = new ParticleEffect("particles/ui_mouseactions/ping_enemyward.vpcf", ward.Position);
            mapPingParticleEffect.SetControlPoint(1, new Vector3(1));
            mapPingParticleEffect.SetControlPoint(5, new Vector3(10, 0, 0));
            hero.PlaySound("General.Ping");
        }

        private bool PlacedWard(EnemyHero enemy, AbilityId id)
        {
            var count = enemy.CountWards(id);

            if (count < enemy.GetWardsCount(id))
            {
                enemy.SetWardsCount(id, count);

                if (settings.IsEnabled(id) && !GaveWard(enemy) && !enemy.DroppedWard(id))
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

        private void TimedRemove()
        {
            foreach (var unit in units.Where(x => x.Duration > 0 && Game.RawGameTime >= x.EndTime).ToList())
            {
                unit.ParticleEffect?.Dispose();
                units.Remove(unit);
            }
        }

        private bool TookWard(EnemyHero enemy)
        {
            return enemyHeroes.Any(
                x => x.IsValid && !x.Equals(enemy) && x.IsAlive && x.Distance(enemy) <= 600 && x.ObserversCount + x.SentryCount
                     > x.CountWards(AbilityId.item_ward_observer) + x.CountWards(AbilityId.item_ward_sentry));
        }
    }
}