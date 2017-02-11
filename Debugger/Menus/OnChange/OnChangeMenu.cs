namespace Debugger.Menus.OnChange
{
    using Ensage.Common.Menu;

    internal class OnChangeMenu
    {
        #region Constructors and Destructors

        public OnChangeMenu(Menu mainMenu)
        {
            var menu = new Menu("On change", "onChange");

            Animations = new Animations(menu);
            Bools = new Bools(menu);
            Floats = new Floats(menu);
            Handles = new Handles(menu);
            Int32 = new Int32(menu);
            Int64 = new Int64(menu);
            Strings = new Strings(menu);

            mainMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Properties

        public Animations Animations { get; }

        public Bools Bools { get; }

        public Floats Floats { get; }

        public Handles Handles { get; }

        public Int32 Int32 { get; }

        public Int64 Int64 { get; }

        public Strings Strings { get; }

        #endregion
    }
}