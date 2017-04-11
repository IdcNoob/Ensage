namespace Debugger.Menus.Dumping
{
    using Ensage.Common.Menu;

    internal class DumpingMenu
    {
        public DumpingMenu(Menu mainMenu)
        {
            var menu = new Menu("Info dumping", "infoDumping");

            Spells = new Spells(menu);
            Items = new Items(menu);
            Modifiers = new Modifiers(menu);
            Units = new Units(menu);

            mainMenu.AddSubMenu(menu);
        }

        public Items Items { get; }

        public Modifiers Modifiers { get; }

        public Spells Spells { get; }

        public Units Units { get; }
    }
}