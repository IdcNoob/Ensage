namespace ItemManager.Menus.Modules.Recovery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Menu;

    using EventArgs;

    using ItemSettings;

    internal class RecoveryMenu
    {
        private AbilityToggler abilityToggler;

        private PriorityChanger priorityChanger;

        private bool skipNextEvent;

        public RecoveryMenu(Menu mainMenu)
        {
            var menu = new Menu("Recovery abuse", "recoveryAbuse");

            var enabled = new MenuItem("abuseEnabled", "Enabled").SetValue(true);
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) =>
                {
                    IsEnabled = args.GetNewValue<bool>();
                    OnEnabledChange?.Invoke(null, new BoolEventArgs(IsEnabled));
                };
            IsEnabled = enabled.IsActive();

            var key = new MenuItem("recoveryKey", "Hotkey").SetValue(new KeyBind('-', KeyBindType.Press));
            menu.AddItem(key);
            key.ValueChanged += (sender, args) =>
                {
                    if (skipNextEvent)
                    {
                        skipNextEvent = false;
                        return;
                    }

                    IsActive = args.GetNewValue<KeyBind>().Active;
                    OnAbuseChange?.Invoke(null, new BoolEventArgs(IsActive));
                };

            menu.AddItem(
                new MenuItem("recoveryItemsToggler", "Enabled:").SetValue(
                    abilityToggler = new AbilityToggler(ItemsToUse.ToDictionary(x => x.ToString(), x => true))));

            menu.AddItem(
                new MenuItem("recoveryItemsPriority", "Order:").SetValue(
                    priorityChanger = new PriorityChanger(ItemsToUse.Select(x => x.ToString()).ToList())));

            var toBackpack = new MenuItem("recoveryToBackpack", "Move items to backpack").SetValue(false);
            toBackpack.SetTooltip("Move items to backpack to \"disable\" them instead of dropping on the ground");
            menu.AddItem(toBackpack);
            toBackpack.ValueChanged += (sender, args) => ItemsToBackpack = args.GetNewValue<bool>();
            ItemsToBackpack = toBackpack.IsActive();

            ItemSettingsMenu = new SettingsMenu(menu);

            mainMenu.AddSubMenu(menu);
        }

        public event EventHandler<BoolEventArgs> OnAbuseChange;

        public event EventHandler<BoolEventArgs> OnEnabledChange;

        public bool IsActive { get; private set; }

        public bool IsEnabled { get; private set; }

        public SettingsMenu ItemSettingsMenu { get; }

        public bool ItemsToBackpack { get; private set; }

        private HashSet<AbilityId> ItemsToUse { get; } = new HashSet<AbilityId>
        {
            AbilityId.item_magic_wand,
            AbilityId.item_magic_stick,
            AbilityId.item_urn_of_shadows,
            AbilityId.item_spirit_vessel,
            AbilityId.item_bottle,
            AbilityId.item_guardian_greaves,
            AbilityId.item_mekansm,
            AbilityId.item_soul_ring,
            AbilityId.item_arcane_boots,
            AbilityId.filler_ability
        };

        public void ForceDisable()
        {
            IsActive = false;
            skipNextEvent = true;
        }

        public uint GetAbilityPriority(string itemName)
        {
            return priorityChanger.GetPriority(itemName);
        }

        public bool IsAbilityEnabled(string itemName)
        {
            return abilityToggler.IsEnabled(itemName);
        }
    }
}