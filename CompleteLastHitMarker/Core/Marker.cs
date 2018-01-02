namespace CompleteLastHitMarker.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;

    using Abilities;

    using Attributes;

    using Ensage;
    using Ensage.Common.Menu;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using Menus;

    using SharpDX;

    using Units;
    using Units.Base;

    [ExportPlugin("Complete Last Hit Marker", StartupMode.Auto, "IdcNoob")]
    internal class Marker : Plugin
    {
        private readonly Type[] abilityTypes;

        private readonly Hero hero;

        private readonly List<KillableUnit> killableUnits = new List<KillableUnit>();

        private MenuManager menu;

        private MyHero myHero;

        private IUpdateHandler updateHandler;

        [ImportingConstructor]
        public Marker([Import] IServiceContext context)
        {
            hero = context.Owner as Hero;
            abilityTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(
                    x => x.Namespace == "CompleteLastHitMarker.Abilities.Passive"
                         || x.Namespace == "CompleteLastHitMarker.Abilities.Active")
                .ToArray();
        }

        protected override void OnActivate()
        {
            menu = new MenuManager();
            myHero = new MyHero(hero, menu, abilityTypes);

            AddTowers();
            AddCreeps();
            AddCouriers();

            ObjectManager.OnAddEntity += OnAddEntity;
            ObjectManager.OnRemoveEntity += OnRemoveEntity;
            Unit.OnModifierAdded += ModifierChanged;
            Unit.OnModifierRemoved += ModifierChanged;
            Drawing.OnDraw += OnDraw;
            updateHandler = UpdateManager.Subscribe(OnUpdate, menu.UpdateRate, menu.IsEnabled);
            menu.UpdateRate.Item.ValueChanged += UpdateRateOnValueChanged;
        }

        protected override void OnDeactivate()
        {
            ObjectManager.OnAddEntity -= OnAddEntity;
            ObjectManager.OnRemoveEntity -= OnRemoveEntity;
            Drawing.OnDraw -= OnDraw;
            Unit.OnModifierAdded -= ModifierChanged;
            Unit.OnModifierRemoved -= ModifierChanged;
            UpdateManager.Unsubscribe(OnUpdate);
            menu.UpdateRate.Item.ValueChanged -= UpdateRateOnValueChanged;

            killableUnits.Clear();
            menu.Dispose();
        }

        private void AddCouriers()
        {
            foreach (var courier in ObjectManager.GetEntities<Courier>().Where(x => x.IsValid && x.IsVisible && x.Team != myHero.Team))
            {
                killableUnits.Add(new KillableCourier(courier));
            }

            foreach (var courier in ObjectManager.GetDormantEntities<Courier>()
                .Where(x => x.IsValid && x.Team != myHero.Team && killableUnits.All(z => z.Handle != x.Handle)))
            {
                killableUnits.Add(new KillableCourier(courier));
            }
        }

        private void AddCreeps()
        {
            foreach (var creep in ObjectManager.GetEntities<Creep>().Where(x => x.IsValid && x.IsAlive && x.IsVisible))
            {
                killableUnits.Add(new KillableCreep(creep));
            }

            foreach (var creep in ObjectManager.GetDormantEntities<Creep>()
                .Where(x => x.IsValid && x.IsAlive && killableUnits.All(z => z.Handle != x.Handle)))
            {
                killableUnits.Add(new KillableCreep(creep));
            }
        }

        private void AddTowers()
        {
            foreach (var tower in ObjectManager.GetEntities<Tower>().Where(x => x.IsValid && x.IsAlive))
            {
                killableUnits.Add(new KillableTower(tower));
            }
        }

        private void ModifierChanged(Unit sender, ModifierChangedEventArgs args)
        {
            if (sender.Handle != myHero.Handle)
            {
                return;
            }

            OnUpdate();
        }

        private void OnAddEntity(EntityEventArgs args)
        {
            var courier = args.Entity as Courier;
            if (courier != null && courier.IsValid && courier.Team != myHero.Team && killableUnits.All(x => x.Handle != args.Entity.Handle))
            {
                killableUnits.Add(new KillableCourier(courier));
                return;
            }

            var creep = args.Entity as Creep;
            if (creep != null && creep.IsValid && killableUnits.All(x => x.Handle != args.Entity.Handle))
            {
                killableUnits.Add(new KillableCreep(creep));
                return;
            }

            var item = args.Entity as Item;
            if (item != null && item.IsValid && item.Purchaser?.Hero?.Handle == myHero.Handle)
            {
                var type = abilityTypes.FirstOrDefault(x => x.GetCustomAttribute<AbilityAttribute>()?.AbilityId == item.Id);

                if (type != null)
                {
                    myHero.AddNewAbility((DefaultAbility)Activator.CreateInstance(type, item));
                }
            }
        }

        private void OnDraw(EventArgs args)
        {
            if (!menu.IsEnabled)
            {
                return;
            }

            foreach (var unit in killableUnits.Where(x => x.DamageCalculated && x.IsValid())
                .OrderByDescending(x => x.Distance(Game.MousePosition)))
            {
                var hpBarPosition = menu.AutoAttackMenu.AutoAttackHealthBar.GetHealthBarPosition(unit);
                if (hpBarPosition.IsZero)
                {
                    continue;
                }

                var hpBarSize = menu.AutoAttackMenu.AutoAttackHealthBar.GetHealthBarSize(unit);
                var myAutoAttackDamageDone = unit.MyAutoAttackDamageDone;
                var health = unit.Health;

                if (menu.AutoAttackMenu.IsEnabled)
                {
                    float size;
                    var startPositionShift = 0f;
                    var sizeMaximized = false;
                    var canBeKilled = myAutoAttackDamageDone >= health;
                    var currentHpBarSize = hpBarSize.X * unit.HealthPercentage;

                    if (menu.AutoAttackMenu.FillHpBar && canBeKilled)
                    {
                        size = hpBarSize.X;
                        sizeMaximized = true;
                    }
                    else
                    {
                        size = Math.Min(hpBarSize.X * (myAutoAttackDamageDone / unit.MaxHealth), currentHpBarSize);
                    }

                    if (menu.AutoAttackMenu.ShowDamageFromRight && !sizeMaximized)
                    {
                        startPositionShift = currentHpBarSize - size;
                    }

                    var color = canBeKilled
                                    ? menu.AutoAttackMenu.AutoAttackColors.CanBeKilledColor(myHero.Team, unit.Team)
                                    : menu.AutoAttackMenu.AutoAttackColors.CanNotBeKilledColor(myHero.Team, unit.Team);

                    // empty hp + border
                    Drawing.DrawRect(hpBarPosition + new Vector2(-1), hpBarSize + new Vector2(2), Color.Black, false);

                    // current hp
                    Drawing.DrawRect(
                        hpBarPosition,
                        new Vector2(currentHpBarSize, hpBarSize.Y),
                        menu.AutoAttackMenu.AutoAttackColors.DefaultHealthColor(myHero.Team, unit.Team),
                        false);

                    // damage
                    Drawing.DrawRect(hpBarPosition + new Vector2(startPositionShift, 0), new Vector2(size, hpBarSize.Y), color, false);

                    if (menu.AutoAttackMenu.SplitHpBar && myAutoAttackDamageDone > 30)
                    {
                        // damage split
                        for (var i = 1; i <= health / myAutoAttackDamageDone; i++)
                        {
                            Drawing.DrawRect(
                                hpBarPosition + new Vector2((size * i) - 1, 0),
                                new Vector2(2, hpBarSize.Y),
                                Color.Black,
                                true);
                        }
                    }

                    if (unit.TowerHelperHits > 0)
                    {
                        // tower helper hits
                        Drawing.DrawText(
                            "+" + unit.TowerHelperHits,
                            "Arial",
                            hpBarPosition + new Vector2(hpBarSize.X + 5, 0),
                            new Vector2(21),
                            Color.White,
                            FontFlags.None);
                    }
                }

                if (menu.AbilitiesMenu.IsEnabled && unit.AbilityDamageCalculated)
                {
                    var damage = 0f;
                    var abilities = new List<DotaTexture>();

                    foreach (var ability in unit.ActiveAbilities.Where(x => x.Key.CanBeCasted()))
                    {
                        damage += ability.Value;
                        abilities.Add(ability.Key.Texture);

                        if (ability.Key.DealsAutoAttackDamage)
                        {
                            damage += myAutoAttackDamageDone;
                        }

                        if (damage >= health || !menu.AbilitiesMenu.SumDamage)
                        {
                            break;
                        }
                    }

                    if (damage <= 0 || !menu.AbilitiesMenu.ShowWarningBorder && damage < health || damage + myAutoAttackDamageDone < health)
                    {
                        continue;
                    }

                    var abilitiesCount = abilities.Count;
                    var startPositionShift = new Vector2(
                        menu.AbilitiesMenu.Texture.X + ((hpBarSize.X - (menu.AbilitiesMenu.Texture.Size * abilitiesCount)) / 2),
                        unit.DefaultTextureY + menu.AbilitiesMenu.Texture.Y);

                    if (menu.AbilitiesMenu.ShowBorder && damage >= health || menu.AbilitiesMenu.ShowWarningBorder && damage < health)
                    {
                        var color = damage < health
                                        ? menu.AbilitiesMenu.AbilitiesColor.CanNotBeKilledColor
                                        : menu.AbilitiesMenu.AbilitiesColor.CanBeKilledColor;
                        var borderWidth = menu.AbilitiesMenu.Texture.Size * 0.1f;
                        var startBorderPosition = hpBarPosition + startPositionShift;

                        // border
                        Drawing.DrawRect(
                            startBorderPosition + new Vector2(-borderWidth),
                            new Vector2(
                                (menu.AbilitiesMenu.Texture.Size * abilitiesCount) + (borderWidth * 2),
                                menu.AbilitiesMenu.Texture.Size + (borderWidth * 2)),
                            color);
                    }

                    for (var i = 0; i < abilitiesCount; i++)
                    {
                        // ability texture
                        Drawing.DrawRect(
                            hpBarPosition + startPositionShift + new Vector2(menu.AbilitiesMenu.Texture.Size * i, 0),
                            new Vector2(menu.AbilitiesMenu.Texture.Size),
                            abilities[i]);
                    }
                }
            }
        }

        private void OnRemoveEntity(EntityEventArgs args)
        {
            killableUnits.RemoveAll(x => x.Handle == args.Entity.Handle);
            myHero.RemoveAbility(args.Entity.Handle);
        }

        private void OnUpdate()
        {
            if (!menu.IsEnabled || !myHero.IsAlive)
            {
                return;
            }

            foreach (var unit in killableUnits.Where(x => x.IsValid() && x.Distance(myHero) < 2000))
            {
                unit.CalculateAutoAttackDamageTaken(myHero);
                unit.CalculateAbilityDamageTaken(myHero, menu.AbilitiesMenu);
            }

            var tower = killableUnits.OfType<KillableTower>()
                .FirstOrDefault(x => x.IsValid() && x.Team == myHero.Team && x.Distance(myHero) < 1000);
            if (tower == null)
            {
                return;
            }

            var towerTarget = killableUnits.FirstOrDefault(x => x.IsValid() && x.Handle == tower.Target?.Handle);
            if (towerTarget == null)
            {
                return;
            }

            var towerDamage = tower.CalculateAverageDamageOn(towerTarget);
            if (towerDamage <= 0)
            {
                return;
            }

            var hits = Math.Floor(towerTarget.Health / towerDamage);
            var hpLeft = towerTarget.Health - (towerDamage * hits);
            if (hpLeft < 5 && hits > 1)
            {
                hpLeft = towerTarget.Health - (towerDamage * (hits - 1));
            }

            towerTarget.TowerHelperHits = (int)Math.Floor(hpLeft / towerTarget.MyAutoAttackDamageDone);
        }

        private void UpdateRateOnValueChanged(object sender, OnValueChangeEventArgs args)
        {
            var timeoutHandler = updateHandler.Executor as TimeoutHandler;
            if (timeoutHandler != null)
            {
                timeoutHandler.Timeout = args.GetNewValue<Slider>().Value;
            }
        }
    }
}