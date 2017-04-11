namespace Debugger.Menus.OnExecuteOrder
{
    using Ensage.Common.Menu;

    internal class AttackMove
    {
        public AttackMove(Menu mainMenu)
        {
            var menu = new Menu("Attack/Move ", "executeMove");

            var enabled = new MenuItem("executeMoveEnabled", "Enabled").SetValue(false)
                .SetTooltip("Player.OnExecuteOrder");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) =>
                {
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

        public bool Enabled { get; private set; }
    }
}