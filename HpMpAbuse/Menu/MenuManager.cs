namespace HpMpAbuse.Menu
{
    using Ensage.Common.Menu;

    internal class MenuManager
    {
        #region Fields

        private readonly Menu menu;

        #endregion

        #region Constructors and Destructors

        public MenuManager()
        {
            menu = new Menu("Smart HP/MP Abuse", "smartAbuse", true);

            Recovery = new Recovery(menu);
            PowerTreads = new PowerTreads(menu);
            SoulRing = new SoulRing(menu);
            ManaChecker = new ManaChecker(menu);
            TranquilBoots = new TranquilBoots(menu);

            menu.AddToMainMenu();
        }

        #endregion

        #region Public Properties

        public ManaChecker ManaChecker { get; }

        public PowerTreads PowerTreads { get; }

        public Recovery Recovery { get; }

        public SoulRing SoulRing { get; }

        public TranquilBoots TranquilBoots { get; }

        #endregion

        #region Public Methods and Operators

        public void AddAbility(string name)
        {
            PowerTreads.AddAbility(name);
            SoulRing.AddAbility(name);
            ManaChecker.AddAbility(name);
        }

        public void OnClose()
        {
            menu.RemoveFromMainMenu();
        }

        public void ReloadAbilityMenu()
        {
            PowerTreads.ReloadAbilityMenu();
            SoulRing.ReloadAbilityMenu();
            ManaChecker.ReloadAbilityMenu();
        }

        #endregion
    }
}