namespace ItemManager.Core.Modules.AutoActions.HpMpRestore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abilities;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using Interfaces;

    using Menus;
    using Menus.Modules.AutoActions.HpMpRestore;

    using Utils;

    [AbilityBasedModule(AbilityId.treant_living_armor)]
    internal class AutoLivingArmor : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly LivingArmorMenu menu;

        private readonly Sleeper sleeper = new Sleeper();

        private LivingArmor livingArmor;

        public AutoLivingArmor(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.AutoHealsMenu.LivingArmorMenu;

            Refresh();

            Game.OnUpdate += OnUpdate;
        }

        public List<AbilityId> AbilityIds { get; } = new List<AbilityId>
        {
            AbilityId.treant_living_armor
        };

        public void Dispose()
        {
            Game.OnUpdate -= OnUpdate;
        }

        public void Refresh()
        {
            livingArmor = manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityIds.First()) as LivingArmor;
        }

        private void OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            sleeper.Sleep(300);

            if (!menu.IsEnabled || !manager.MyHero.CanUseAbilities() || !livingArmor.CanBeCasted() || Game.IsPaused)
            {
                return;
            }

            if (menu.IsEnabledHero)
            {
                var hero = manager.Units.OfType<Hero>()
                    .Where(
                        x => x.IsValid && x.IsAlive && x.Team == manager.MyHero.Team && !x.IsIllusion && !x.IsInvul()
                             && !x.HasModifier(ModifierUtils.LivingArmorModifier))
                    .OrderBy(x => x.HealthPercentage())
                    .FirstOrDefault(x => x.HealthPercentage() < menu.HeroHpThreshold);

                if (hero != null)
                {
                    if (menu.HeroEnemySearchRange <= 0 || ObjectManager.GetEntitiesParallel<Hero>()
                            .Any(
                                x => x.IsAlive && !x.IsIllusion && x.Distance2D(hero) <= menu.HeroEnemySearchRange
                                     && x.Team != manager.MyHero.Team
                                     && !x.HasModifier(ModifierUtils.LivingArmorModifier)))
                    {
                        PrintMessage(hero);
                        livingArmor.Use(hero);
                        sleeper.Sleep(1000);
                        return;
                    }
                }
            }

            if (menu.IsEnabledTower)
            {
                var tower = manager.Units.OfType<Tower>()
                    .Where(
                        x => x.IsValid && x.IsAlive && x.Team == manager.MyHero.Team
                             && !x.HasModifier(ModifierUtils.LivingArmorModifier))
                    .OrderBy(x => x.HealthPercentage())
                    .FirstOrDefault(x => x.HealthPercentage() < menu.TowerHpThreshold);

                if (tower != null)
                {
                    PrintMessage(tower);
                    livingArmor.Use(tower);
                    sleeper.Sleep(1000);
                    return;
                }
            }

            if (menu.IsEnabledCreep)
            {
                if (menu.IsEnabledCreepUnderTower)
                {
                    var creep = manager.Units.OfType<Tower>()
                        .Where(x => x.IsValid && x.IsAlive && x.Team != manager.MyHero.Team)
                        .Select(x => x.AttackTarget)
                        .Where(
                            x => x is Creep && x.IsValid && x.IsAlive && x.IsSpawned
                                 && !x.HasModifier(ModifierUtils.LivingArmorModifier))
                        .OrderBy(x => x.HealthPercentage())
                        .FirstOrDefault();

                    if (creep != null)
                    {
                        PrintMessage(creep);
                        livingArmor.Use(creep);
                        sleeper.Sleep(1000);
                        return;
                    }
                }

                var lowHpCreep = manager.Units.OfType<Creep>()
                    .Where(
                        x => x.IsValid && x.IsAlive && x.IsSpawned && x.Team == manager.MyHero.Team && !x.IsInvul()
                             && !x.HasModifier(ModifierUtils.LivingArmorModifier))
                    .OrderBy(x => x.HealthPercentage())
                    .FirstOrDefault(x => x.HealthPercentage() < menu.CreepHpThreshold);

                if (lowHpCreep != null)
                {
                    PrintMessage(lowHpCreep);
                    livingArmor.Use(lowHpCreep);
                    sleeper.Sleep(1000);
                }
            }
        }

        private void PrintMessage(Entity entity)
        {
            if (menu.IsEnabledNotification)
            {
                Game.PrintMessage(
                    "<font face='Verdana' color='#ff7700'>[</font>Item Manager<font face='Verdana' color='#ff7700'>]</font>"
                    + " Living armor => " + Game.Localize(entity.Name));
            }
        }
    }
}