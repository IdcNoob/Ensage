namespace Debugger.Menus.OnExecuteOrder
{
    using Ensage.Common.Menu;

    internal class OnExecuteOrderMenu
    {
        public OnExecuteOrderMenu(Menu mainMenu)
        {
            var menu = new Menu("On execute order", "onExecuteOrder");

            Abilities = new Abilities(menu);
            AttackMove = new AttackMove(menu);
            Other = new Other(menu);

            mainMenu.AddSubMenu(menu);
        }

        public Abilities Abilities { get; }

        public AttackMove AttackMove { get; }

        public Other Other { get; }
    }
}