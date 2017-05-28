namespace ItemManager.Core.Modules.AutoActions.HpMpRestore
{
    using System.Linq;

    using Abilities.Base;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.SDK.Helpers;

    using Interfaces;

    using Menus;
    using Menus.Modules.AutoActions.HpMpRestore;

    using Utils;

    [AbilityBasedModule(AbilityId.treant_living_armor)]
    internal class AutoLivingArmor : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly LivingArmorMenu menu;

        private UsableAbility livingArmor;

        public AutoLivingArmor(Manager manager, MenuManager menu, AbilityId abilityId)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.AutoHealsMenu.LivingArmorMenu;

            AbilityId = abilityId;
            Refresh();

            UpdateManager.Subscribe(OnUpdate, 300);
        }

        public AbilityId AbilityId { get; }

        public void Dispose()
        {
            UpdateManager.Unsubscribe(OnUpdate);
        }

        public void Refresh()
        {
            livingArmor = manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId);
        }

        private void OnUpdate()
        {
            if (!menu.IsEnabled || !manager.MyHero.CanUseAbilities() || !livingArmor.CanBeCasted() || Game.IsPaused)
            {
                return;
            }

            if (menu.IsEnabledHero)
            {
                var hero = manager.Units.OfType<Hero>()
                    .Where(
                        x => x.IsValid && x.IsAlive && x.Team == manager.MyHero.Team
                             && (!menu.IgnoreSelf || x.Handle != manager.MyHero.Handle) && !x.IsIllusion && !x.IsInvul()
                             && !x.HasModifier(ModifierUtils.LivingArmorModifier))
                    .OrderBy(x => x.HealthPercentage())
                    .FirstOrDefault(x => x.HealthPercentage() < menu.HeroHpThreshold);

                if (hero != null)
                {
                    if (menu.HeroEnemySearchRange <= 0 || ObjectManager.GetEntitiesParallel<Hero>()
                            .Any(
                                x => x.IsAlive && !x.IsIllusion && x.IsVisible && x.Team != manager.MyHero.Team
                                     && x.Distance2D(hero) <= menu.HeroEnemySearchRange
                                     && !x.HasModifier(ModifierUtils.LivingArmorModifier)))
                    {
                        PrintMessage(hero);
                        livingArmor.Use(hero);
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
                    return;
                }
            }

            if (menu.IsEnabledBarracks)
            {
                var rax = manager.Units.OfType<Building>()
                    .Where(
                        x => x.IsValid && x.UnitType == 80 && x.IsAlive && x.Team == manager.MyHero.Team
                             && !x.HasModifier(ModifierUtils.LivingArmorModifier))
                    .OrderBy(x => x.HealthPercentage())
                    .FirstOrDefault(x => x.HealthPercentage() < menu.BarracksHpThreshold);

                if (rax != null)
                {
                    PrintMessage(rax);
                    livingArmor.Use(rax);
                    return;
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