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

        #endregion

        #region Constructors and Destructors

        public SoulRing(Menu mainMenu)
        {
            var menu = new Menu("Auto Soul Ring", "soulringAbuse", false, "item_soul_ring", true);

            menu.AddItem(enabled = new MenuItem("enabledSR", "Enabled").SetValue(true));
            menu.AddItem(enabledAbilities = new MenuItem("enabledSRAbilities", "Enabled for"));
            menu.AddItem(
                manaThreshold =
                new MenuItem("soulringMPThreshold", "MP% threshold").SetValue(new Slider(90))
                    .SetTooltip("Don't use soul ring if you have more MP%"));
            menu.AddItem(
                healthThreshold =
                new MenuItem("soulringHPThreshold", "HP% threshold").SetValue(new Slider(30))
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
            return enabledAbilities.GetValue<AbilityToggler>().IsEnabled(name);
        }

        public void AddAbility(string name)
        {
            abilities.Add(name, true);
        }

        public void ReloadAbilityMenu()
        {
            enabledAbilities.SetValue(new AbilityToggler(abilities));
        }

        #endregion
    }
}