namespace ItemManager.Menus.Modules.Recovery
{
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Menu;

    internal class RecoveryMenu
    {
        private AbilityToggler abilityToggler;

        public RecoveryMenu(Menu mainMenu)
        {
            var menu = new Menu("Recovery Abuse", "recoveryAbuse");

            var key = new MenuItem("recoveryKey", "Hotkey").SetValue(new KeyBind('-', KeyBindType.Press));
            menu.AddItem(key);
            key.ValueChanged += (sender, args) => IsActive = args.GetNewValue<KeyBind>().Active;

            menu.AddItem(
                new MenuItem("itemsToggler", "Enabled items:").SetValue(
                    abilityToggler = new AbilityToggler(ItemsToUse.ToDictionary(x => x.Key, x => true))));

            var autoSelfBottle = new MenuItem("autoBottleSelf", "Auto self bottle").SetValue(true);
            autoSelfBottle.SetTooltip("Auto bottle usage on you and your allies while at base");
            menu.AddItem(autoSelfBottle);
            autoSelfBottle.ValueChanged += (sender, args) => AutoSelfBottle = args.GetNewValue<bool>();
            AutoSelfBottle = autoSelfBottle.IsActive();

            var autoAllyBottle = new MenuItem("autoBottleAlly", "Auto ally bottle").SetValue(true);
            autoAllyBottle.SetTooltip("Auto bottle usage on you and your allies while at base");
            menu.AddItem(autoAllyBottle);
            autoAllyBottle.ValueChanged += (sender, args) => AutoAllyBottle = args.GetNewValue<bool>();
            AutoAllyBottle = autoAllyBottle.IsActive();

            var toBackpack = new MenuItem("toBackpack", "Move items to backpack").SetValue(true);
            toBackpack.SetTooltip("Move items to backpack to disable them instead of dropping on the ground");
            menu.AddItem(toBackpack);
            toBackpack.ValueChanged += (sender, args) => ItemsToBackpack = args.GetNewValue<bool>();
            ItemsToBackpack = toBackpack.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public bool AutoAllyBottle { get; private set; }

        public bool AutoSelfBottle { get; private set; }

        public bool IsActive { get; private set; }

        public bool ItemsToBackpack { get; private set; }

        public Dictionary<string, AbilityId> ItemsToUse { get; } = new Dictionary<string, AbilityId>
        {
            { "item_arcane_boots", AbilityId.item_arcane_boots },
            { "item_bottle", AbilityId.item_bottle },
            { "item_guardian_greaves", AbilityId.item_guardian_greaves },
            { "item_magic_stick", AbilityId.item_magic_stick },
            { "item_mekansm", AbilityId.item_mekansm },
            { "item_soul_ring", AbilityId.item_soul_ring },
            { "item_urn_of_shadows", AbilityId.item_urn_of_shadows }
        };

        public bool IsAbilityEnabled(string itemName)
        {
            return abilityToggler.IsEnabled(itemName);
        }
    }
}