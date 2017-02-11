namespace Debugger.Menus.OnAddRemove
{
    using Ensage.Common.Menu;

    internal class Units
    {
        #region Constructors and Destructors

        public Units(Menu mainMenu)
        {
            var menu = new Menu("Units", "units");

            var onAddEnabled = new MenuItem("onUnitsAddEnabled", "On add enabled").SetValue(false);
            menu.AddItem(onAddEnabled);
            onAddEnabled.ValueChanged += (sender, args) => OnAddEnabled = args.GetNewValue<bool>();
            OnAddEnabled = onAddEnabled.IsActive();

            var onRemoveEnabled = new MenuItem("onUnitsRemoveEnabled", "On remove enabled").SetValue(false);
            menu.AddItem(onRemoveEnabled);
            onRemoveEnabled.ValueChanged += (sender, args) => OnRemoveEnabled = args.GetNewValue<bool>();
            OnRemoveEnabled = onRemoveEnabled.IsActive();

            var ignoreUnits = new MenuItem("ignoreUnits", "Ignore useless units").SetValue(false);
            menu.AddItem(ignoreUnits);
            ignoreUnits.ValueChanged += (sender, args) => IgnoreUseless = args.GetNewValue<bool>();
            IgnoreUseless = ignoreUnits.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Properties

        public bool IgnoreUseless { get; private set; }

        public bool OnAddEnabled { get; private set; }

        public bool OnRemoveEnabled { get; private set; }

        #endregion
    }
}