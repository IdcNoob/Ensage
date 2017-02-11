namespace Debugger.Menus.OnAddRemove
{
    using Ensage.Common.Menu;

    internal class OnAddRemoveMenu
    {
        #region Constructors and Destructors

        public OnAddRemoveMenu(Menu mainMenu)
        {
            var menu = new Menu("On add/remove", "onAddRemove");

            Units = new Units(menu);
            Modifiers = new Modifiers(menu);
            Particles = new Particles(menu);
            Abilities = new Abilities(menu);
            Projectiles = new Projectiles(menu);

            mainMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Properties

        public Abilities Abilities { get; }

        public Modifiers Modifiers { get; }

        public Particles Particles { get; }

        public Projectiles Projectiles { get; }

        public Units Units { get; }

        #endregion
    }
}