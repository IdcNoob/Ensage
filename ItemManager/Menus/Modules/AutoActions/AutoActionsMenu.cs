namespace ItemManager.Menus.Modules.AutoActions
{
    using Actions;

    using Ensage.Common.Menu;

    internal class AutoActionsMenu
    {
        public AutoActionsMenu(Menu mainMenu)
        {
            var menu = new Menu("Auto actions", "autoUsage");

            AutoHealsMenu = new AutoHealsMenu(menu);
            SoulRingMenu = new SoulRingMenu(menu);
            PowerTreadsMenu = new PowerTreadsMenu(menu);
            PhaseBootsMenu = new PhaseBootsMenu(menu);
            MidasMenu = new MidasMenu(menu);
            DewardingMenu = new DewardingMenu(menu);
            TechiesMinesDestroyerMenu = new TechiesMinesDestroyerMenu(menu);
            DustMenu = new DustMenu(menu);

            mainMenu.AddSubMenu(menu);
        }

        public AutoHealsMenu AutoHealsMenu { get; }

        public DewardingMenu DewardingMenu { get; }

        public DustMenu DustMenu { get; }

        public MidasMenu MidasMenu { get; }

        public PhaseBootsMenu PhaseBootsMenu { get; }

        public PowerTreadsMenu PowerTreadsMenu { get; }

        public SoulRingMenu SoulRingMenu { get; }

        public TechiesMinesDestroyerMenu TechiesMinesDestroyerMenu { get; }
    }
}