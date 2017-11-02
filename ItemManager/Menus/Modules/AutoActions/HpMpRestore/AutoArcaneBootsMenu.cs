namespace ItemManager.Menus.Modules.AutoActions.HpMpRestore
{
    using System;

    using Ensage.Common.Menu;

    using EventArgs;

    internal class AutoArcaneBootsMenu
    {
        public AutoArcaneBootsMenu(Menu mainMenu)
        {
            var menu = new Menu("Arcane boots", "autoArcaneBoots");

            var enabled = new MenuItem("autoUseArcaneBoots", "Enabled").SetValue(true);
            enabled.SetTooltip("Auto use arcane boots when you are missing mp");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) =>
                {
                    IsEnabled = args.GetNewValue<bool>();
                    OnEnabledChange?.Invoke(null, new BoolEventArgs(IsEnabled));
                };
            IsEnabled = enabled.IsActive();

            var allyRange = new MenuItem("autoBootsAllyRange", "Ally search range").SetValue(new Slider(2000, 0, 5000));
            allyRange.SetTooltip("Don't use if ally with low mp in range");
            menu.AddItem(allyRange);
            allyRange.ValueChanged += (sender, args) => AllySearchRange = args.GetNewValue<Slider>().Value;
            AllySearchRange = allyRange.GetValue<Slider>().Value;

            var fountainRange = new MenuItem("autoBootsFountainRange", "Fountain range").SetValue(new Slider(4000, 2000, 8000));
            fountainRange.SetTooltip("Don't use if fountain is closer (2k - fountain; 4k - base; 8k - t2)");
            menu.AddItem(fountainRange);
            fountainRange.ValueChanged += (sender, args) => FountainRange = args.GetNewValue<Slider>().Value;
            FountainRange = fountainRange.GetValue<Slider>().Value;

            var notifyAllies = new MenuItem("autoArcaneNotify", "Notify allies").SetValue(true);
            notifyAllies.SetTooltip("Notify allies if they are out of arcane boots cast range");
            menu.AddItem(notifyAllies);
            notifyAllies.ValueChanged += (sender, args) => NotifyAllies = args.GetNewValue<bool>();
            NotifyAllies = notifyAllies.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public event EventHandler<BoolEventArgs> OnEnabledChange;

        public int AllySearchRange { get; private set; }

        public int FountainRange { get; private set; }

        public bool IsEnabled { get; private set; }

        public bool NotifyAllies { get; private set; }
    }
}