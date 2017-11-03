namespace ItemManager.Menus.Modules.Recovery.ItemSettings
{
    using Ensage.Common.Menu;

    internal class SettingsMenu
    {
        public SettingsMenu(Menu mainMenu)
        {
            var menu = new Menu("Item settings", "recoveryAbuseItemSettings");

            BottleSettings = new BottleSettings(menu, "Bottle", 90, 60);
            SoulRing = new SoulRingSettings(menu, "Soul ring", null, 50);
            ArcaneBootsSettings = new ItemSettingsMenu(menu, "Arcane boots", null, 100);
            Mekansm = new ItemSettingsMenu(menu, "Mekansm", 150);
            GuardianGreaves = new ItemSettingsMenu(menu, "Guardian greaves", 150, 150);
            MagicStick = new ItemSettingsMenu(menu, "Magic stick", 100, 100);
            UrnOfShadows = new ItemSettingsMenu(menu, "Urn of shadows", 300);
            SpiritVessel = new ItemSettingsMenu(menu, "Spirit vessel", 300);
            PowerTreads = new PowerTreadsSettings(menu);

            mainMenu.AddSubMenu(menu);
        }

        public ItemSettingsMenu ArcaneBootsSettings { get; }

        public BottleSettings BottleSettings { get; }

        public ItemSettingsMenu GuardianGreaves { get; }

        public ItemSettingsMenu MagicStick { get; }

        public ItemSettingsMenu Mekansm { get; }

        public PowerTreadsSettings PowerTreads { get; }

        public SoulRingSettings SoulRing { get; }

        public ItemSettingsMenu SpiritVessel { get; }

        public ItemSettingsMenu UrnOfShadows { get; }
    }
}