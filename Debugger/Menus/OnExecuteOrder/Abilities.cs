namespace Debugger.Menus.OnExecuteOrder
{
    using Ensage.Common.Menu;

    internal class Abilities
    {
        public Abilities(Menu mainMenu)
        {
            var menu = new Menu("Abilities ", "executeAbilities");

            var enabled = new MenuItem("executeAbilitiesEnabled", "Enabled").SetValue(false)
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