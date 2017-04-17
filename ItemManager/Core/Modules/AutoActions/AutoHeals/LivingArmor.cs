namespace ItemManager.Core.Modules.AutoActions.AutoHeals
{
    using System;
    using System.Linq;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using EventArgs;

    using Menus;
    using Menus.Modules.AutoActions.Heals;

    using Utils;

    [Module]
    internal class LivingArmor : IDisposable
    {
        private const string LivingArmorModifier = "modifier_treant_living_armor";

        private readonly Manager manager;

        private readonly LivingArmorMenu menu;

        private readonly Sleeper sleeper = new Sleeper();

        private Ability livingArmor;

        private bool subscribed;

        public LivingArmor(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.AutoHealsMenu.LivingArmorMenu;

            manager.OnAbilityAdd += OnAbilityAdd;
            manager.OnAbilityRemove += OnAbilityRemove;
        }

        public void Dispose()
        {
        }

        private void OnAbilityAdd(object sender, AbilityEventArgs abilityEventArgs)
        {
            if (!abilityEventArgs.IsMine || abilityEventArgs.Ability.Id != AbilityId.treant_living_armor || subscribed)
            {
                return;
            }

            livingArmor = abilityEventArgs.Ability;

            subscribed = true;
            Game.OnUpdate += OnUpdate;
        }

        private void OnAbilityRemove(object sender, AbilityEventArgs abilityEventArgs)
        {
            if (!abilityEventArgs.IsMine || abilityEventArgs.Ability.Id != AbilityId.treant_living_armor || !subscribed)
            {
                return;
            }

            subscribed = false;
            Game.OnUpdate -= OnUpdate;
        }

        private void OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            sleeper.Sleep(300);

            if (!menu.IsEnabled || !manager.MyHeroCanUseAbilities() || !livingArmor.CanBeCasted() || Game.IsPaused)
            {
                return;
            }

            if (menu.IsEnabledHero)
            {
                var hero = manager.Units.OfType<Hero>()
                    .Where(
                        x => x.IsValid && x.IsAlive && x.Team == manager.MyTeam && !x.IsIllusion && !x.IsInvul()
                             && !x.HasModifier(LivingArmorModifier))
                    .OrderBy(x => x.HealthPercentage())
                    .FirstOrDefault(x => x.HealthPercentage() < menu.HeroHpThreshold);

                if (hero != null)
                {
                    if (menu.HeroEnemySearchRange <= 0 || ObjectManager.GetEntitiesParallel<Hero>()
                            .Any(
                                x => x.IsAlive && !x.IsIllusion && x.Distance2D(hero) <= menu.HeroEnemySearchRange
                                     && x.Team != manager.MyTeam && !x.HasModifier(LivingArmorModifier)))
                    {
                        PrintMessage(hero.GetRealName());
                        livingArmor.UseAbility(hero);
                        sleeper.Sleep(1000);
                        return;
                    }
                }
            }

            if (menu.IsEnabledTower)
            {
                var tower = manager.Units.OfType<Tower>()
                    .Where(
                        x => x.IsValid && x.IsAlive && x.Team == manager.MyTeam && !x.HasModifier(LivingArmorModifier))
                    .OrderBy(x => x.HealthPercentage())
                    .FirstOrDefault(x => x.HealthPercentage() < menu.TowerHpThreshold);

                if (tower != null)
                {
                    PrintMessage(tower.Name);
                    livingArmor.UseAbility(tower);
                    sleeper.Sleep(1000);
                    return;
                }
            }

            if (menu.IsEnabledCreep)
            {
                if (menu.IsEnabledCreepUnderTower)
                {
                    var creep = manager.Units.OfType<Tower>()
                        .Where(x => x.IsValid && x.IsAlive && x.Team != manager.MyTeam)
                        .Select(x => x.AttackTarget)
                        .Where(
                            x => x is Creep && x.IsValid && x.IsAlive && x.IsSpawned
                                 && !x.HasModifier(LivingArmorModifier))
                        .OrderBy(x => x.HealthPercentage())
                        .FirstOrDefault();

                    if (creep != null)
                    {
                        PrintMessage(creep.Name);
                        livingArmor.UseAbility(creep);
                        sleeper.Sleep(1000);
                        return;
                    }
                }

                var lowHpCreep = manager.Units.OfType<Creep>()
                    .Where(
                        x => x.IsValid && x.IsAlive && x.IsSpawned && x.Team == manager.MyTeam && !x.IsInvul()
                             && !x.HasModifier(LivingArmorModifier))
                    .OrderBy(x => x.HealthPercentage())
                    .FirstOrDefault(x => x.HealthPercentage() < menu.CreepHpThreshold);

                if (lowHpCreep != null)
                {
                    PrintMessage(lowHpCreep.Name);
                    livingArmor.UseAbility(lowHpCreep);
                    sleeper.Sleep(1000);
                }
            }
        }

        private void PrintMessage(string name)
        {
            if (menu.IsEnabledNotification)
            {
                Game.PrintMessage(
                    "<font face='Verdana' color='#ff7700'>[</font>Item Manager<font face='Verdana' color='#ff7700'>]</font>"
                    + " Living armor => " + name);
            }
        }
    }
}