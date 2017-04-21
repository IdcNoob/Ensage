namespace ItemManager.Core.Modules.AutoActions.HpMpRestore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abilities;

    using Attributes;

    using Ensage;
    using Ensage.Common.Objects.UtilityObjects;

    using Interfaces;

    using Menus;
    using Menus.Modules.AutoActions.HpMpRestore;

    [AbilityBasedModule(AbilityId.item_magic_stick)]
    [AbilityBasedModule(AbilityId.item_magic_wand)]
    internal class AutoMagicStick : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly AutoMagicStickMenu menu;

        private readonly Sleeper sleeper = new Sleeper();

        private MagicStick magicStick;

        public AutoMagicStick(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.AutoHealsMenu.AutoMagicStickMenu;

            Refresh();

            Game.OnUpdate += OnUpdate;
        }

        public List<AbilityId> AbilityIds { get; } = new List<AbilityId>
        {
            AbilityId.item_magic_stick,
            AbilityId.item_magic_wand
        };

        public void Dispose()
        {
            Game.OnUpdate -= OnUpdate;
        }

        public void Refresh()
        {
            magicStick = manager.MyHero.UsableAbilities.FirstOrDefault(x => AbilityIds.Contains(x.Id)) as MagicStick;
        }

        private void OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            sleeper.Sleep(100);

            if (Game.IsPaused || !manager.MyHero.CanUseItems() || !magicStick.CanBeCasted())
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