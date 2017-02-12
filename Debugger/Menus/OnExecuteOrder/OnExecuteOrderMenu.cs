namespace Debugger.Menus.OnExecuteOrder
{
    using Ensage.Common.Menu;

    internal class OnExecuteOrderMenu
    {
        #region Constructors and Destructors

        public OnExecuteOrderMenu(Menu mainMenu)
        {
            var menu = new Menu("On execute order", "onExecuteOrder");

            Abilities = new Abilities(menu);
            AttackMove = new AttackMove(menu);
            Other = new Other(menu);

            mainMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Properties

        public Abilities Abilities { get; }

        public AttackMove AttackMove { get; }

        public Other Other { get; }

        #endregion
    }
}