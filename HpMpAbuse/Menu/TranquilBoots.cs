namespace HpMpAbuse.Menu
{
    using Ensage.Common.Menu;

    internal class TranquilBoots
    {
        #region Fields

        private readonly MenuItem combineActive;

        private readonly MenuItem combineEnabled;

        #endregion

        #region Constructors and Destructors

        public TranquilBoots(Menu mainMenu)
        {
            var menu = new Menu("Tranquil Abuse", "tranquilAbuse", false, "item_tranquil_boots", true);

            var drop = new Menu("Drop abuse", "dropAbuse");
            drop.AddItem(new MenuItem("dropTranquils", "Drop key").SetValue(new KeyBind('M', KeyBindType.Press)))
                .ValueChanged += (sender, args) => DropActive = args.GetNewValue<KeyBind>().Active;

            var combine = new Menu("Combine abuse", "combineAbuse");
            combine.AddItem(combineEnabled = new MenuItem("enabledCombine", "Enabled").SetValue(false))
                .SetTooltip("Don't enable it if you dont know how this abuse works");
            combine.AddItem(
                combineActive = new MenuItem("activeCombine", "Assemble").SetValue(new KeyBind('N', KeyBindType.Toggle)));

            menu.AddSubMenu(drop);
            menu.AddSubMenu(combine);

            mainMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Properties

        public bool CombineActive => combineActive.IsActive();

        public bool CombineEnabled => combineEnabled.IsActive();

        public bool DropActive { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void DisableCombine()
        {
            combineEnabled.SetValue(false).DontSave();
        }

        #endregion
    }
}