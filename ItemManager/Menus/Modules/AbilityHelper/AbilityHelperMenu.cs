namespace ItemManager.Menus.Modules.AbilityHelper
{
    using Ensage.Common.Menu;

    internal class AbilityHelperMenu
    {
        public AbilityHelperMenu(Menu mainMenu)
        {
            var menu = new Menu("Ability helper", "abilityHelper");

            Tranquil = new Tranquil(menu);
            Blink = new Blink(menu);

            mainMenu.AddSubMenu(menu);
        }

        public Blink Blink { get; }

        public Tranquil Tranquil { get; }
    }
}