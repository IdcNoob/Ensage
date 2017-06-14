namespace ItemManager.Menus.Modules.AutoActions.HpMpRestore
{
    using System;

    using Ensage.Common.Menu;

    using EventArgs;

    internal class AutoBottleMenu
    {
        public AutoBottleMenu(Menu mainMenu)
        {
            var menu = new Menu("Bottle", "bottle");

            var enabled = new MenuItem("autoBottleEnabled", "Enabled").SetValue(true);
            enabled.SetTooltip("Auto use bottle at base");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) =>
                {
                    IsEnabled = args.GetNewValue<bool>();
                    OnEnabledChange?.Invoke(null, new BoolEventArgs(IsEnabled));
                };
            IsEnabled = enabled.IsActive();

            var autoSelfBottle = new MenuItem("autoBottleSelf", "Self bottle").SetValue(true);
            autoSelfBottle.SetTooltip("Auto bottle usage on self while at base");
            menu.AddItem(autoSelfBottle);
            autoSelfBottle.ValueChanged += (sender, args) => AutoSelfBottle = args.GetNewValue<bool>();
            AutoSelfBottle = autoSelfBottle.IsActive();

            var autoAllyBottle = new MenuItem("autoBottleAlly", "Ally bottle").SetValue(true);
            autoAllyBottle.SetTooltip("Auto bottle usage on allies while at base");
            menu.AddItem(autoAllyBottle);
            autoAllyBottle.ValueChanged += (sender, args) => AutoAllyBottle = args.GetNewValue<bool>();
            AutoAllyBottle = autoAllyBottle.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public event EventHandler<BoolEventArgs> OnEnabledChange;

        public bool AutoAllyBottle { get; private set; }

        public bool AutoSelfBottle { get; private set; }

        public bool IsEnabled { get; private set; }
    }
}