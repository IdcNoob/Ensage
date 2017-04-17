namespace ItemManager.Menus.Modules.AutoActions.HpMpRestore
{
    using Ensage.Common.Menu;

    internal class AutoArcaneBootsMenu
    {
        public AutoArcaneBootsMenu(Menu mainMenu)
        {
            var menu = new Menu("Arcane boots", "autoArcaneBoots");

            var autoUse = new MenuItem("autoUseArcaneBoots", "Auto use").SetValue(true);
            autoUse.SetTooltip("Auto use arcane boots when you are missing mp");
            menu.AddItem(autoUse);
            autoUse.ValueChanged += (sender, args) => AutoUse = args.GetNewValue<bool>();
            AutoUse = autoUse.IsActive();

            var allyRange = new MenuItem("autoBootsAllyRange", "Ally search range").SetValue(new Slider(2000, 0, 5000));
            allyRange.SetTooltip("Don't use if ally with low mp in range");
            menu.AddItem(allyRange);
            allyRange.ValueChanged += (sender, args) => AllySearchRange = args.GetNewValue<Slider>().Value;
            AllySearchRange = allyRange.GetValue<Slider>().Value;

            var notifyAllies = new MenuItem("autoArcaneNotify", "Notify allies").SetValue(true);
            notifyAllies.SetTooltip("Notify allies if they are out of arcane boots cast range");
            menu.AddItem(notifyAllies);
            notifyAllies.ValueChanged += (sender, args) => NotifyAllies = args.GetNewValue<bool>();
            NotifyAllies = notifyAllies.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public int AllySearchRange { get; private set; }

        public bool AutoUse { get; private set; }

        public bool NotifyAllies { get; private set; }
    }
}