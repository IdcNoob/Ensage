namespace ItemManager.Core.Modules.AutoActions.HpMpRestore
{
    using System.Linq;

    using Abilities.Base;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;

    using EventArgs;

    using Interfaces;

    using Menus;
    using Menus.Modules.AutoActions.HpMpRestore;

    using Utils;

    [AbilityBasedModule(AbilityId.treant_living_armor)]
    internal class AutoLivingArmor : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly LivingArmorMenu menu;

        private readonly IUpdateHandler updateHandler;

        private UsableAbility livingArmor;

        public AutoLivingArmor(Manager manager, MenuManager menu, AbilityId abilityId)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.AutoHealsMenu.LivingArmorMenu;

            AbilityId = abilityId;
            Refresh();

            updateHandler = UpdateManager.Subscribe(OnUpdate, 300, this.menu.IsEnabled);
            this.menu.OnEnabledChange += MenuOnEnabledChange;
        }

        public AbilityId AbilityId { get; }

        public void Dispose()
        {
            menu.OnEnabledChange -= MenuOnEnabledChange;
            UpdateManager.Unsubscribe(OnUpdate);
        }

        public void Refresh()
        {
            livingArmor = manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId);
        }

        private void MenuOnEnabledChange(object sender, BoolEventArgs boolEventArgs)
        {
            updateHandler.IsEnabled = boolEventArgs.Enabled;
        }

        private void OnUpdate()
        {
            if (Game.IsPaused || !livingArmor.CanBeCasted() || !manager.MyHero.CanUseAbilities())
            {
                return;
            }

            if (menu.IsEnabledHero)
            {
                var hero = EntityManager<Hero>.Entities
                    .Where(
                        x => x.IsValid && x.IsAlive && x.Team == manager.MyHero.Team
                             && (!menu.IgnoreSelf || x.Handle != manager.MyHero.Handle) && !x.IsIllusion && !x.IsInvul()
                             && !x.HasModifier(ModifierUtils.LivingArmorModifier))
                    .OrderBy(x => x.HealthPercentage())
                    .FirstOrDefault(x => x.HealthPercentage() < menu.HeroHpThreshold);

                if (hero != null)
                {
                    if (menu.HeroEnemySearchRange <= 0 || EntityManager<Hero>.Entities.Any(
                            x => x.IsAlive && !x.IsIllusion && x.IsVisible && x.Team != manager.MyHero.Team
                                 && x.Distance2D(hero) <= menu.HeroEnemySearchRange && !x.HasModifier(ModifierUtils.LivingArmorModifier)))
                    {
                        PrintMessage(hero);
                        livingArmor.Use(hero);
                        return;
                    }
                }
            }

            if (menu.IsEnabledTower)
            {
                var tower = EntityManager<Tower>.Entities
                    .Where(
                        x => x.IsValid && x.IsAlive && x.Team == manager.MyHero.Team && !x.HasModifier(ModifierUtils.LivingArmorModifier))
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
                    var creep = EntityManager<Tower>.Entities.Where(x => x.IsValid && x.IsAlive && x.Team != manager.MyHero.Team)
                        .Select(x => x.AttackTarget)
                        .Where(
                            x => x is Creep && x.IsValid && x.IsAlive && x.IsSpawned && !x.HasModifier(ModifierUtils.LivingArmorModifier))
                        .OrderBy(x => x.HealthPercentage())
                        .FirstOrDefault();

                    if (creep != null)
                    {
                        PrintMessage(creep);
                        livingArmor.Use(creep);
                        return;
                    }
                }

                var lowHpCreep = EntityManager<Creep>.Entities
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
                var rax = EntityManager<Building>.Entities
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