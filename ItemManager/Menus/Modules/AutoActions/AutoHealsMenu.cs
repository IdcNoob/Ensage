namespace ItemManager.Menus.Modules.AutoActions
{
    using Ensage;
    using Ensage.Common.Menu;

    using HpMpRestore;

    internal class AutoHealsMenu
    {
        public AutoHealsMenu(Menu rootMenu)
        {
            var menu = new Menu("HP/MP restore", "autoHealUsage");

            AutoBottleMenu = new AutoBottleMenu(menu);
            AutoArcaneBootsMenu = new AutoArcaneBootsMenu(menu);
            AutoTangoMenu = new AutoTangoMenu(menu);
            AutoMagicStickMenu = new InstantHealthRestoreItemMenu(menu, "Magic stick");
            AutoCheeseMenu = new InstantHealthRestoreItemMenu(menu, "Cheese");
            AutoFaerieFireMenu = new InstantHealthRestoreItemMenu(menu, "Faerie fire", false);
            LivingArmorMenu = new LivingArmorMenu(menu);
            rootMenu.AddSubMenu(menu);
        }

        public AutoArcaneBootsMenu AutoArcaneBootsMenu { get; }

        public AutoBottleMenu AutoBottleMenu { get; }

        public InstantHealthRestoreItemMenu AutoCheeseMenu { get; }

        public InstantHealthRestoreItemMenu AutoFaerieFireMenu { get; }

        public InstantHealthRestoreItemMenu AutoMagicStickMenu { get; }

        public AutoTangoMenu AutoTangoMenu { get; }

        public LivingArmorMenu LivingArmorMenu { get; }

        public InstantHealthRestoreItemMenu GetMenuFor(AbilityId abilityId)
        {
            switch (abilityId)
            {
                case AbilityId.item_cheese:
                    return AutoCheeseMenu;
                case AbilityId.item_magic_wand:
                case AbilityId.item_magic_stick:
                    return AutoMagicStickMenu;
                case AbilityId.item_faerie_fire:
                    return AutoFaerieFireMenu;
            }

            return null;
        }
    }
}