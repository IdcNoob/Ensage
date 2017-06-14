namespace ItemManager.Menus.Modules.AutoActions.Actions
{
    using System;

    using Ensage.Common.Menu;

    using EventArgs;

    internal class IronTalonMenu
    {
        public IronTalonMenu(Menu mainMenu)
        {
            var menu = new Menu("Iron talon", "ironTalonMenu");

            var enabled = new MenuItem("ironTalonEnabled", "Enabled").SetValue(true);
            enabled.SetTooltip("Auto use iron talon on creeps");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) =>
                {
                    IsEnabled = args.GetNewValue<bool>();
                    OnEnabledChange?.Invoke(null, new BoolEventArgs(IsEnabled));
                };
            IsEnabled = enabled.IsActive();

            var damageThreshold =
                new MenuItem("ironTalonDamageThreshold", "Damage threshold").SetValue(new Slider(300, 100, 600));
            damageThreshold.SetTooltip("Use iron talon only when it will deal more damage");
            menu.AddItem(damageThreshold);
            damageThreshold.ValueChanged += (sender, args) => DamageThreshold = args.GetNewValue<Slider>().Value;
            DamageThreshold = damageThreshold.GetValue<Slider>().Value;

            mainMenu.AddSubMenu(menu);
        }

        public event EventHandler<BoolEventArgs> OnEnabledChange;

        public int DamageThreshold { get; private set; }

        public bool IsEnabled { get; private set; }
    }
}