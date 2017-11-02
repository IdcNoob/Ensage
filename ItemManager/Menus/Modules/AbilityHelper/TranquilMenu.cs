namespace ItemManager.Menus.Modules.AbilityHelper
{
    using System;

    using Ensage.Common.Menu;

    using EventArgs;

    internal class TranquilMenu
    {
        public TranquilMenu(Menu mainMenu)
        {
            var menu = new Menu("Tranquil boots", "tranquilDrop");

            var key = new MenuItem("tranquilDropKey", "Drop tranquil boots").SetValue(new KeyBind('-', KeyBindType.Press));
            menu.AddItem(key);
            key.ValueChanged += (sender, args) =>
                {
                    IsActive = args.GetNewValue<KeyBind>().Active;
                    OnTranquilDrop?.Invoke(null, new BoolEventArgs(IsActive));
                };

            mainMenu.AddSubMenu(menu);
        }

        public event EventHandler<BoolEventArgs> OnTranquilDrop;

        public bool IsActive { get; private set; }
    }
}