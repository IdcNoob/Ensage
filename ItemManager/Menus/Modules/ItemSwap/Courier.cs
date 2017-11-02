namespace ItemManager.Menus.Modules.ItemSwap
{
    using System;
    using System.Collections.Generic;

    using Ensage.Common.Menu;

    internal class Courier
    {
        private AbilityToggler abilityToggler;

        public Courier(Menu mainMenu)
        {
            var menu = new Menu("Courier", "courierSwapper");

            menu.AddItem(
                new MenuItem("courierSwapItems", "Items:").SetValue(abilityToggler = new AbilityToggler(new Dictionary<string, bool>())));

            var key = new MenuItem("courierKey", "Hotkey").SetValue(new KeyBind('-', KeyBindType.Press));
            menu.AddItem(key);
            key.ValueChanged += (sender, args) =>
                {
                    if (args.GetNewValue<KeyBind>().Active)
                    {
                        OnSwap?.Invoke(this, EventArgs.Empty);
                    }
                };

            mainMenu.AddSubMenu(menu);
        }

        public event EventHandler OnSwap;

        public void AddItem(string itemName)
        {
            abilityToggler.Add(itemName, false);
        }

        public bool ItemEnabled(string abilityName)
        {
            return abilityToggler.IsEnabled(abilityName);
        }

        public void RemoveItem(string itemName)
        {
            abilityToggler.Remove(itemName);
        }
    }
}