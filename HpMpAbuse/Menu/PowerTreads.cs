namespace HpMpAbuse.Menu
{
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Menu;

    internal class PowerTreads
    {
        #region Fields

        public readonly Dictionary<string, Attribute> Attributes = new Dictionary<string, Attribute>
        {
            { "Don't switch", Attribute.Invalid },
            { "Main attribute", Attribute.Invalid },
            { "Strength", Attribute.Strength },
            { "Intelligence", Attribute.Intelligence },
            { "Agility", Attribute.Agility }
        };

        private readonly Dictionary<string, bool> abilities = new Dictionary<string, bool>();

        private readonly MenuItem autoDisableTime;

        private readonly MenuItem enabled;

        private readonly MenuItem enabledAbilities;

        private readonly MenuItem manaThreshold;

        private readonly MenuItem switchOnAttack;

        private readonly MenuItem switchOnHeal;

        private readonly MenuItem switchOnMove;

        private AbilityToggler abilityToggler;

        #endregion

        #region Constructors and Destructors

        public PowerTreads(Menu mainMenu)
        {
            var menu = new Menu("Power Treads Switcher", "ptSwitcher", false, "item_power_treads", true);
            var heroName = Variables.Hero.Name;

            menu.AddItem(enabled = new MenuItem(heroName + "enabledPT", "Enabled").SetValue(true));
            menu.AddItem(enabledAbilities = new MenuItem(heroName + "enabledPTAbilities", "Enabled for"))
                .SetValue(abilityToggler = new AbilityToggler(abilities));
            menu.AddItem(
                    switchOnMove =
                        new MenuItem(heroName + "switchPTonMove", "Switch when moving").SetValue(
                            new StringList(Attributes.Keys.ToArray())))
                .SetTooltip("Switch PT to selected attribute when moving");
            menu.AddItem(
                    switchOnAttack =
                        new MenuItem(heroName + "switchPTonAttack", "Swtich when attacking").SetValue(
                            new StringList(Attributes.Keys.ToArray())))
                .SetTooltip("Switch PT to selected attribute when attacking");
            menu.AddItem(switchOnHeal = new MenuItem(heroName + "switchPTHeal", "Swtich when healing").SetValue(true))
                .SetTooltip("Bottle, flask, tango and some hero spells");
            menu.AddItem(
                manaThreshold =
                    new MenuItem("manaPTThreshold", "Mana cost threshold").SetValue(new Slider(15, 0, 50))
                        .SetTooltip("Don't switch PT if spell/Item costs less mana"));
            menu.AddItem(
                autoDisableTime =
                    new MenuItem(heroName + "autoPTdisable", "Auto disable PT switcher after (mins)").SetValue(
                        new Slider(0, 0, 60)));

            mainMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Properties

        public int AutoDisableTime => autoDisableTime.GetValue<Slider>().Value;

        public bool Enabled => enabled.IsActive();

        public int ManaThreshold => manaThreshold.GetValue<Slider>().Value;

        public int SwitchOnAttack => switchOnAttack.GetValue<StringList>().SelectedIndex;

        public bool SwitchOnHeal => switchOnHeal.IsActive();

        public int SwitchOnMove => switchOnMove.GetValue<StringList>().SelectedIndex;

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