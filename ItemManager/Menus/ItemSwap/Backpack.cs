namespace ItemManager.Menus.ItemSwap
{
    using System;
    using System.Collections.Generic;

    using Ensage.Common.Menu;

    internal class Backpack
    {
        #region Constructors and Destructors

        public Backpack(Menu mainMenu)
        {
            var menu = new Menu("Backpack", "backpackSwapper");

            menu.AddItem(
                new MenuItem("backpackSwapItems", "Items to swap:").SetValue(
                    abilityToggler = new AbilityToggler(new Dictionary<string, bool>())));

            var key = new MenuItem("backpackKey", "Hotkey").SetValue(new KeyBind('Z', KeyBindType.Press));
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

        #endregion

        #region Fields

        private AbilityToggler abilityToggler;

        #endregion

        #region Public Events

        public event EventHandler OnSwap;

        #endregion

        #region Public Methods and Operators

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

        #endregion
    }
}