namespace Debugger.Menus.OnAddRemove
{
    using Ensage.Common.Menu;

    internal class OnAddRemoveMenu
    {
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

        public Abilities Abilities { get; }

        public Modifiers Modifiers { get; }

        public Particles Particles { get; }

        public Projectiles Projectiles { get; }

        public Units Units { get; }
    }
}