namespace ItemManager.Core.Modules.AutoActions.HpMpRestore
{
    using System.Linq;

    using Abilities.Base;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.SDK.Helpers;

    using EventArgs;

    using Interfaces;

    using Menus;
    using Menus.Modules.AutoActions.HpMpRestore;

    using Utils;

    [AbilityBasedModule(AbilityId.item_magic_stick)]
    [AbilityBasedModule(AbilityId.item_magic_wand)]
    [AbilityBasedModule(AbilityId.item_cheese)]
    [AbilityBasedModule(AbilityId.item_faerie_fire)]
    internal class InstantHealthRestoreItem : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly InstantHealthRestoreItemMenu menu;

        private UsableAbility usableAbility;

        public InstantHealthRestoreItem(Manager manager, MenuManager menu, AbilityId abilityId)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.AutoHealsMenu.GetMenuFor(abilityId);

            AbilityId = abilityId;
            Refresh();

            if (this.menu.IsEnabled)
            {
                Entity.OnInt32PropertyChange += OnInt32PropertyChange;
            }
            this.menu.OnEnabledChange += MenuOnEnabledChange;
        }

        public AbilityId AbilityId { get; }

        public void Dispose()
        {
            menu.OnEnabledChange -= MenuOnEnabledChange;
            Entity.OnInt32PropertyChange -= OnInt32PropertyChange;
        }

        public void Refresh()
        {
            usableAbility = manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId);
        }

        private void MenuOnEnabledChange(object sender, BoolEventArgs boolEventArgs)
        {
            if (boolEventArgs.Enabled)
            {
                Entity.OnInt32PropertyChange += OnInt32PropertyChange;
            }
            else
            {
                Entity.OnInt32PropertyChange -= OnInt32PropertyChange;
            }
        }

        private void OnInt32PropertyChange(Entity sender, Int32PropertyChangeEventArgs args)
        {
            if (sender.Handle != manager.MyHero.Handle || args.NewValue <= 0 || args.NewValue == args.OldValue
                || args.PropertyName != "m_iHealth")
            {
                return;
            }

            if (!manager.MyHero.CanUseItems() || !usableAbility.CanBeCasted() || manager.MyHero.HasModifier(ModifierUtils.IceBlastDebuff))
            {
                return;
            }

            if (menu.EnemySearchRange > 0 && !EntityManager<Hero>.Entities.Any(
                    x => x.IsValid && x.IsVisible && x.IsAlive && !x.IsIllusion && x.Team != manager.MyHero.Team
                         && x.Distance2D(manager.MyHero.Position) <= menu.EnemySearchRange))
            {
                return;
            }

            var hp = args.NewValue;
            var hpPercentage = (hp / manager.MyHero.MaximumHealth) * 100;

            if (hp <= menu.HealthThreshold || hpPercentage <= menu.HealthThresholdPct)
            {
                usableAbility.Use();
            }
        }
    }
}