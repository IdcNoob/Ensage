namespace ItemManager.Core.Modules.AutoActions.HpMpRestore
{
    using System.Linq;

    using Abilities;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.SDK.Helpers;

    using Interfaces;

    using Menus;
    using Menus.Modules.AutoActions.HpMpRestore;

    using Utils;

    [AbilityBasedModule(AbilityId.item_magic_stick)]
    [AbilityBasedModule(AbilityId.item_magic_wand)]
    internal class AutoMagicStick : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly AutoMagicStickMenu menu;

        private MagicStick magicStick;

        public AutoMagicStick(Manager manager, MenuManager menu, AbilityId abilityId)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.AutoHealsMenu.AutoMagicStickMenu;

            AbilityId = abilityId;
            Refresh();

            UpdateManager.Subscribe(OnUpdate, 100);
        }

        public AbilityId AbilityId { get; }

        public void Dispose()
        {
            UpdateManager.Unsubscribe(OnUpdate);
        }

        public void Refresh()
        {
            magicStick = manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId) as MagicStick;
        }

        private void OnUpdate()
        {
            if (Game.IsPaused || !manager.MyHero.CanUseItems() || !magicStick.CanBeCasted()
                || manager.MyHero.HasModifier(ModifierUtils.IceBlastDebuff))
            {
                return;
            }

            if (menu.EnemySearchRange > 0 && !ObjectManager.GetEntitiesParallel<Hero>()
                    .Any(
                        x => x.IsAlive && !x.IsIllusion && x.IsVisible && x.Team != manager.MyHero.Team
                             && x.Distance2D(manager.MyHero.Position) <= menu.EnemySearchRange))
            {
                return;
            }

            if (manager.MyHero.Health <= menu.HealthThreshold
                || manager.MyHero.HealthPercentage <= menu.HealthThresholdPct)
            {
                magicStick.Use();
            }
        }
    }
}