namespace HpMpAbuse.Menu
{
    using System.Collections.Generic;

    using Ensage.Common.Menu;

    internal class SoulRing
    {
        #region Fields

        private readonly Dictionary<string, bool> abilities = new Dictionary<string, bool>();

        private readonly MenuItem enabled;

        private readonly MenuItem enabledAbilities;

        private readonly MenuItem healthThreshold;

        private readonly MenuItem manaThreshold;

        private AbilityToggler abilityToggler;

        #endregion

        #region Constructors and Destructors

        public SoulRing(Menu mainMenu)
        {
            var menu = new Menu("Auto Soul Ring", "soulringAbuse", false, "item_soul_ring", true);
            var heroName = Variables.Hero.Name;

            menu.AddItem(enabled = new MenuItem(heroName + "enabledSR", "Enabled").SetValue(true));
            menu.AddItem(enabledAbilities = new MenuItem(heroName + "enabledSRAbilities", "Enabled for"))
                .SetValue(abilityToggler = new AbilityToggler(abilities));
            menu.AddItem(
                manaThreshold =
                    new MenuItem(heroName + "soulringMPThreshold", "MP% threshold").SetValue(new Slider(90))
                        .SetTooltip("Don't use soul ring if you have more MP%"));
            menu.AddItem(
                healthThreshold =
                    new MenuItem(heroName + "soulringHPThreshold", "HP% threshold").SetValue(new Slider(30))
                        .SetTooltip("Don't use soul ring if you have less HP%"));

            mainMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Properties

        public bool Enabled => enabled.IsActive();

        public int HealthThreshold => healthThreshold.GetValue<Slider>().Value;

        public int ManaThreshold => manaThreshold.GetValue<Slider>().Value;

        #endregion

        #region Public Methods and Operators

        public bool AbilityEnabled(string name)
        {
            return abilityToggler.IsEnabled(name);
        }

        public void AddAbility(string name)
        {
            abilityToggler.Add(name);
            enabledAbilities.SetValue(abilityToggler);
        }

        #endregion
    }
}