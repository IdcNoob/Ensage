namespace Debugger.Menus.OnExecuteOrder
{
    using Ensage.Common.Menu;

    internal class Other
    {
        #region Constructors and Destructors

        public Other(Menu mainMenu)
        {
            var menu = new Menu("All other orders ", "executeOther");

            var enabled =
                new MenuItem("executeOtherEnabled", "Enabled").SetValue(false).SetTooltip("Player.OnExecuteOrder");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) => {
                Enabled = args.GetNewValue<bool>();
                if (Enabled)
                {
                    menu.DisplayName = menu.DisplayName += "*";
                }
                else
                {
                    menu.DisplayName = menu.DisplayName.TrimEnd('*');
                }
            };
            Enabled = enabled.IsActive();
            if (Enabled)
            {
                menu.DisplayName = menu.DisplayName += "*";
            }

            mainMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Properties

        public bool Enabled { get; private set; }

        #endregion
    }
}