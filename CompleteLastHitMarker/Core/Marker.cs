namespace CompleteLastHitMarker.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Abilities;

    using Attributes;

    using Ensage;
    using Ensage.Common.Objects.UtilityObjects;

    using Menus;

    using SharpDX;

    using Units;
    using Units.Base;

    internal class Marker
    {
        private readonly List<KillableUnit> killableUnits = new List<KillableUnit>();

        private List<Type> abilityTypes;

        private MyHero hero;

        private MenuManager menu;

        private Sleeper sleeper;

        public void OnClose()
        {
            ObjectManager.OnAddEntity -= OnAddEntity;
            ObjectManager.OnRemoveEntity -= OnRemoveEntity;
            Game.OnUpdate -= OnUpdate;
            Drawing.OnDraw -= OnDraw;

            killableUnits.Clear();
            menu.OnClose();
        }

        public void OnLoad()
        {
            abilityTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(
                    x => x.Namespace == "CompleteLastHitMarker.Abilities.Passive"
                         || x.Namespace == "CompleteLastHitMarker.Abilities.Active")
                .ToList();

            menu = new MenuManager();
            hero = new MyHero(ObjectManager.LocalHero, menu, abilityTypes);
            sleeper = new Sleeper();

            AddTowers();
            AddCreeps();

            ObjectManager.OnAddEntity += OnAddEntity;
            ObjectManager.OnRemoveEntity += OnRemoveEntity;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private void AddCreeps()
        {
            foreach (var tower in ObjectManager.GetEntitiesParallel<Creep>().Where(x => x.IsValid && x.IsAlive))
            {
                killableUnits.Add(new KillableCreep(tower));
            }
        }

        private void AddTowers()
        {
            foreach (var tower in ObjectManager.GetEntitiesParallel<Tower>().Where(x => x.IsValid && x.IsAlive))
            {
                killableUnits.Add(new KillableTower(tower));
            }
        }

        private void OnAddEntity(EntityEventArgs args)
        {
            var creep = args.Entity as Creep;
            if (creep != null && creep.IsValid && killableUnits.All(x => x.Handle != args.Entity.Handle))
            {
                killableUnits.Add(new KillableCreep(creep));
                return;
            }

            var item = args.Entity as Item;
            if (item != null && item.IsValid && item.Purchaser?.Hero?.Handle == hero.Handle)
            {
                var type = abilityTypes.FirstOrDefault(
                    x => x.GetCustomAttribute<AbilityAttribute>()?.AbilityId == item.Id);

                if (type != null)
                {
                    hero.AddNewAbility((DefaultAbility)Activator.CreateInstance(type, item));
                }
            }
        }

        private void OnDraw(EventArgs args)
        {
            if (!menu.IsEnabled)
            {
                return;
            }

            foreach (var unit in killableUnits.Where(x => x.DamageCalculated && x.IsValid()))
            {
                var hpBarPosition = menu.AutoAttackMenu.AutoAttackHealthBar.GetHealthBarPosition(unit);
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
                                    ? menu.AutoAttackMenu.AutoAttackColors.CanBeKilledColor(hero.Team, unit.Team)
                                    : menu.AutoAttackMenu.AutoAttackColors.CanNotBeKilledColor(hero.Team, unit.Team);

                    // empty hp + border
                    Drawing.DrawRect(hpBarPosition + new Vector2(-1), hpBarSize + new Vector2(2), Color.Black, false);

                    // current hp
                    Drawing.DrawRect(
                        hpBarPosition,
                        new Vector2(currentHpBarSize, hpBarSize.Y),
                        menu.AutoAttackMenu.AutoAttackColors.DefaultHealthColor(hero.Team, unit.Team),
                        false);

                    // damage
                    Drawing.DrawRect(
                        hpBarPosition + new Vector2(startPositionShift, 0),
                        new Vector2(size, hpBarSize.Y),
                        color,
                        false);
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

                    if (damage <= 0 || !menu.AbilitiesMenu.ShowWarningBorder && damage < health
                        || damage + myAutoAttackDamageDone < health)
                    {
                        continue;
                    }

                    var abilitiesCount = abilities.Count;
                    var startPositionShift = new Vector2(
                        menu.AbilitiesMenu.Texture.X + (hpBarSize.X - menu.AbilitiesMenu.Texture.Size * abilitiesCount)
                        / 2,
                        unit.DefaultTextureY + menu.AbilitiesMenu.Texture.Y);

                    if (menu.AbilitiesMenu.ShowBorder && damage >= health
                        || menu.AbilitiesMenu.ShowWarningBorder && damage < health)
                    {
                        var color = damage < health
                                        ? menu.AbilitiesMenu.AbilitiesColor.CanNotBeKilledColor
                                        : menu.AbilitiesMenu.AbilitiesColor.CanBeKilledColor;
                        var borderWidth = menu.AbilitiesMenu.Texture.Size * 0.1f;
                        var startBorderPosition = hpBarPosition + startPositionShift;

                        //border
                        Drawing.DrawRect(
                            startBorderPosition + new Vector2(-borderWidth),
                            new Vector2(
                                menu.AbilitiesMenu.Texture.Size * abilitiesCount + borderWidth * 2,
                                menu.AbilitiesMenu.Texture.Size + borderWidth * 2),
                            color);
                    }

                    for (var i = 0; i < abilitiesCount; i++)
                    {
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
            hero.RemoveAbility(args.Entity.Handle);
        }

        private void OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            sleeper.Sleep(750);

            if (!menu.IsEnabled || !hero.IsAlive)
            {
                return;
            }

            foreach (var unit in killableUnits.Where(x => x.IsValid() && x.Distance(hero) < 2000))
            {
                unit.CalculateAutoAttackDamageTaken(hero);
                unit.CalculateAbilityDamageTaken(hero, menu.AbilitiesMenu);
            }
        }
    }
}