namespace HpMpAbuse.Menu
{
    using System.Collections.Generic;

    using Ensage.Common;
    using Ensage.Common.Menu;

    internal class ManaChecker
    {
        #region Fields

        private readonly Dictionary<string, bool> abilities = new Dictionary<string, bool>();

        private readonly MenuItem enabled;

        private readonly MenuItem enabledAbilities;

        private readonly MenuItem manaInfo;

        private readonly MenuItem ptMana;

        private readonly MenuItem textSize;

        private readonly MenuItem xPos;

        private readonly MenuItem yPos;

        #endregion

        #region Constructors and Destructors

        public ManaChecker(Menu mainMenu)
        {
            var menu = new Menu("Mana Combo Checker", "manaChecker", false, "item_energy_booster", true);

            menu.AddItem(enabled = new MenuItem("enabledMC", "Enabled").SetValue(false))
                .SetTooltip("Don't forget to change text position");
            menu.AddItem(enabledAbilities = new MenuItem("enabledMCAbilities", "Enabled for"));
            menu.AddItem(
                manaInfo =
                new MenuItem("mcManaInfo", "Show mana info").SetValue(true)
                    .SetTooltip("Will show how much mana left/needed after/before casting combo"));
            menu.AddItem(
                ptMana =
                new MenuItem("mcPTcalculations", "Include PT calculations").SetValue(true)
                    .SetTooltip("Will include in calculations mana gained from Power Treads switching"));
            menu.AddItem(textSize = new MenuItem("mcSize", "Text size").SetValue(new Slider(25, 20, 40)));
            menu.AddItem(
                xPos = new MenuItem("mcX", "Position X").SetValue(new Slider(0, 0, (int)HUDInfo.ScreenSizeX())));
            menu.AddItem(
                yPos = new MenuItem("mcY", "Position Y").SetValue(new Slider(0, 0, (int)HUDInfo.ScreenSizeY())));

            mainMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Properties

        public bool Enabled => enabled.IsActive();

        public bool IncludePtCalcualtions => ptMana.IsActive();

        public bool ShowManaInfo => manaInfo.IsActive();

        public int TextPositionX => xPos.GetValue<Slider>().Value;

        public int TextPositionY => yPos.GetValue<Slider>().Value;

        public int TextSize => textSize.GetValue<Slider>().Value;

        #endregion

        #region Public Methods and Operators

        public bool AbilityEnabled(string name)
        {
            return enabledAbilities.GetValue<AbilityToggler>().IsEnabled(name);
        }

        public void AddAbility(string name)
        {
            abilities.Add(name, false);
        }

        public void ReloadAbilityMenu()
        {
            enabledAbilities.SetValue(new AbilityToggler(abilities));
        }

        #endregion
    }
}