namespace ItemManager.Menus.Modules.AbilityHelper
{
    using Ensage.Common.Menu;

    internal class AbilityHelperMenu
    {
        public AbilityHelperMenu(Menu mainMenu)
        {
            var menu = new Menu("Ability helper", "abilityHelper");

            TranquilMenu = new TranquilMenu(menu);
            BlinkMenu = new BlinkMenu(menu);

            mainMenu.AddSubMenu(menu);
        }

        public BlinkMenu BlinkMenu { get; }

        public TranquilMenu TranquilMenu { get; }
    }
}