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

        private AbilityToggler abilityToggler;

        #endregion

        #region Constructors and Destructors

        public ManaChecker(Menu mainMenu)
        {
            var menu = new Menu("Mana Combo Checker", "manaChecker", false, "item_energy_booster", true);
            var heroName = Variables.Hero.Name;

            menu.AddItem(enabled = new MenuItem(heroName + "enabledMC", "Enabled").SetValue(false))
                .SetTooltip("Don't forget to change text position");
            menu.AddItem(enabledAbilities = new MenuItem(heroName + "enabledMCAbilities", "Enabled for"))
                .SetValue(abilityToggler = new AbilityToggler(abilities));
            menu.AddItem(
                manaInfo =
                    new MenuItem("mcManaInfo", "Show mana info").SetValue(true)
                        .SetTooltip("Will show how much mana left/needed after/before casting combo"));
            menu.AddItem(
                ptMana =
                    new MenuItem("mcPTcalculations", "Include PT calculations").SetValue(true)
                        .SetTooltip("Will include in calculations mana gained from Power Treads switching"));
            menu.AddItem(textSize = new MenuItem("mcSize", "Text size").SetValue(new Slider(25, 20, 40)));

            var x = (int)HUDInfo.ScreenSizeX();
            var y = (int)HUDInfo.ScreenSizeY();

            menu.AddItem(xPos = new MenuItem("mcX", "Position X").SetValue(new Slider((int)(x * 0.65), 0, x)));
            menu.AddItem(yPos = new MenuItem("mcY", "Position Y").SetValue(new Slider((int)(y * 0.8), 0, y)));

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
            return abilityToggler.IsEnabled(name);
        }

        public void AddAbility(string name)
        {
            abilityToggler.Add(name, false);
            enabledAbilities.SetValue(abilityToggler);
        }

        #endregion
    }
}