namespace ItemManager.Menus.ItemSwap
{
    using System;
    using System.Collections.Generic;

    using Ensage.Common.Menu;

    internal class Stash
    {
        #region Constructors and Destructors

        public Stash(Menu mainMenu)
        {
            var menu = new Menu("Stash", "stashSwapper");

            menu.AddItem(
                new MenuItem("stashSwapItems", "Items to swap:").SetValue(
                    abilityToggler = new AbilityToggler(new Dictionary<string, bool>())));

            var key = new MenuItem("stashKey", "Hotkey").SetValue(new KeyBind('X', KeyBindType.Press));
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