namespace Debugger.Menus.OnAddRemove
{
    using Ensage.Common.Menu;

    internal class Units
    {
        public Units(Menu mainMenu)
        {
            var menu = new Menu("Units ", "units");

            var onAddEnabled = new MenuItem("onUnitsAddEnabled", "On add enabled").SetValue(false)
                .SetTooltip("ObjectManager.OnAddEntity");
            menu.AddItem(onAddEnabled);
            onAddEnabled.ValueChanged += (sender, args) =>
                {
                    OnAddEnabled = args.GetNewValue<bool>();
                    if (OnAddEnabled && !menu.DisplayName.EndsWith("*"))
                    {
                        menu.DisplayName = menu.DisplayName += "*";
                    }
                    else if (!OnRemoveEnabled)
                    {
                        menu.DisplayName = menu.DisplayName.TrimEnd('*');
                    }
                };
            OnAddEnabled = onAddEnabled.IsActive();
            if (OnAddEnabled)
            {
                menu.DisplayName = menu.DisplayName += "*";
            }

            var onRemoveEnabled = new MenuItem("onUnitsRemoveEnabled", "On remove enabled").SetValue(false)
                .SetTooltip("ObjectManager.OnRemoveEntity");
            menu.AddItem(onRemoveEnabled);
            onRemoveEnabled.ValueChanged += (sender, args) =>
                {
                    OnRemoveEnabled = args.GetNewValue<bool>();
                    if (OnRemoveEnabled && !menu.DisplayName.EndsWith("*"))
                    {
                        menu.DisplayName = menu.DisplayName += "*";
                    }
                    else if (!OnAddEnabled)
                    {
                        menu.DisplayName = menu.DisplayName.TrimEnd('*');
                    }
                };
            OnRemoveEnabled = onRemoveEnabled.IsActive();
            if (OnRemoveEnabled)
            {
                menu.DisplayName = menu.DisplayName += "*";
            }

            var ignoreUnits = new MenuItem("ignoreUnits", "Ignore useless units").SetValue(false);
            menu.AddItem(ignoreUnits);
            ignoreUnits.ValueChanged += (sender, args) => IgnoreUseless = args.GetNewValue<bool>();
            IgnoreUseless = ignoreUnits.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public bool IgnoreUseless { get; private set; }

        public bool OnAddEnabled { get; private set; }

        public bool OnRemoveEnabled { get; private set; }
    }
}