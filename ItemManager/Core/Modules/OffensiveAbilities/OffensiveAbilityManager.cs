namespace ItemManager.Core.Modules.OffensiveAbilities
{
    using System;
    using System.Linq;

    using Core.Abilities.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;
    using Ensage.Common.Objects.UtilityObjects;
    using Ensage.SDK.Helpers;

    using Interfaces;

    using Menus;
    using Menus.Modules.OffensiveAbilities;
    using Menus.Modules.OffensiveAbilities.AbilitySettings;

    using Utils;

    internal abstract class OffensiveItemBase : IAbilityBasedModule
    {
        protected IOffensiveAbility Ability;

        protected Unit CurrentTarget;

        protected Unit LastTarget;

        protected AbilitySettingsMenu Menu;

        protected OffensiveItemBase(Manager manager, MenuManager menu, AbilityId abilityId)
        {
            Manager = manager;
            OffMenu = menu.OffensiveItemsMenu;

            AbilityId = abilityId;
            Refresh();

            UpdateManager.Subscribe(OnUpdate, 100);
        }

        public AbilityId AbilityId { get; }

        protected Manager Manager { get; }

        protected OffensiveItemsMenu OffMenu { get; }

        protected Sleeper Sleeper { get; } = new Sleeper();

        public void Dispose()
        {
            UpdateManager.Unsubscribe(OnUpdate);
        }

        public void Refresh()
        {
            Ability = Manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId) as IOffensiveAbility;

            if (Ability == null)
            {
                Dispose();
            }
        }

        protected virtual bool CheckTarget(Unit target)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }

            if (!Menu.IsEnabled(target.StoredName()) || !OffMenu.IsEnabled(target.StoredName()))
            {
                return false;
            }

            if (!target.IsAlive || !target.IsVisible || target.IsInvul()
                || target.Distance2D(Manager.MyHero.Position) > Ability.GetCastRange())
            {
                return false;
            }

            if (!Menu.HexStack && target.IsReallyHexed())
            {
                return false;
            }

            if (!Menu.SilenceStack && target.IsSilenced() && !target.IsReallyHexed())
            {
                return false;
            }

            if (!Menu.RootStack && target.IsRooted())
            {
                return false;
            }

            if (!Menu.StunStack && target.IsStunned())
            {
                return false;
            }

            if (!Menu.DisarmStack && target.IsDisarmed() && !target.IsReallyHexed())
            {
                return false;
            }

            return true;
        }

        private void DelayedUse()
        {
            if (CurrentTarget == null || !CurrentTarget.Equals(LastTarget))
            {
                return;
            }

            if (!Ability.CanBeCasted(CurrentTarget) || !Manager.MyHero.CanUseItems())
            {
                return;
            }

            if (!CheckTarget(CurrentTarget))
            {
                return;
            }

            Manager.MyHero.UsableAbilities.Where(x => x.IsOffensiveAbility).ForEach(x => x.SetSleep(200));
            Ability.Use(CurrentTarget);
        }

        private void OnUpdate()
        {
            if (Sleeper.Sleeping || !Menu.Enabled || Game.IsPaused || !Manager.MyHero.IsAlive)
            {
                return;
            }

            CurrentTarget = CheckTarget(Manager.MyHero.Target) ? Manager.MyHero.Target : null;

            if (Menu.AlwaysUse && CurrentTarget == null)
            {
                CurrentTarget = EntityManager<Hero>.Entities
                    .Where(x => x.IsValid && x.IsAlive && x.Team != Manager.MyHero.Team && !x.IsIllusion)
                    .OrderBy(x => x.Distance2D(Manager.MyHero.Position))
                    .FirstOrDefault(CheckTarget);
            }

            if (CurrentTarget == null || !Ability.CanBeCasted(CurrentTarget) || !Manager.MyHero.CanUseItems())
            {
                return;
            }

            LastTarget = CurrentTarget;
            UpdateManager.BeginInvoke(DelayedUse, Menu.Delay);
            Sleeper.Sleep(Math.Min(200, Menu.Delay));
        }
    }
}