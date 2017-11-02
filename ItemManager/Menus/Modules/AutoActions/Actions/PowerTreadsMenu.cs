namespace ItemManager.Menus.Modules.AutoActions.Actions
{
    using System;
    using System.Collections.Generic;

    using Ensage.Common.Menu;

    using EventArgs;

    internal class PowerTreadsMenu
    {
        private AbilityToggler abilityToggler;

        public PowerTreadsMenu(Menu mainMenu)
        {
            var menu = new Menu("Power treads", "ptSwitcherMenu");

            var enabled = new MenuItem("ptSwitcherEnabled", "Enabled").SetValue(true);
            enabled.SetTooltip("Auto switch power treads when using abilities");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) =>
                {
                    IsEnabled = args.GetNewValue<bool>();
                    OnEnabledChange?.Invoke(null, new BoolEventArgs(IsEnabled));
                };
            IsEnabled = enabled.IsActive();

            var universal = new MenuItem("ptUniversalUse", "Universal use").SetValue(true);
            universal.SetTooltip("If enabled power treads will work with all other assemblies otherwise only when player used ability");
            menu.AddItem(universal);
            universal.ValueChanged += (sender, args) => UniversalUseEnabled = args.GetNewValue<bool>();
            UniversalUseEnabled = universal.IsActive();

            var switchOnMove = new MenuItem("ptSwitchOnMove", "Switch on move").SetValue(
                new StringList("None", "Hero attribute", "Strength", "Agility", "Intelligence"));
            menu.AddItem(switchOnMove);
            switchOnMove.ValueChanged += (sender, args) => SwitchOnMoveAttribute = args.GetNewValue<StringList>().SelectedIndex;
            SwitchOnMoveAttribute = switchOnMove.GetValue<StringList>().SelectedIndex;

            var switchOnAttack = new MenuItem("ptSwitchOnAttack", "Switch on attack").SetValue(
                new StringList("None", "Hero attribute", "Strength", "Agility", "Intelligence"));
            menu.AddItem(switchOnAttack);
            switchOnAttack.ValueChanged += (sender, args) => SwitchOnAttackAttribute = args.GetNewValue<StringList>().SelectedIndex;
            SwitchOnAttackAttribute = switchOnAttack.GetValue<StringList>().SelectedIndex;

            var switchOnHeal = new MenuItem("ptSwitchOnHeal", "Switch when healing").SetValue(true);
            switchOnHeal.SetTooltip("AutoBottleMenu, flask, shrine");
            menu.AddItem(switchOnHeal);
            switchOnHeal.ValueChanged += (sender, args) => SwitchOnHeal = args.GetNewValue<bool>();
            SwitchOnHeal = switchOnHeal.IsActive();

            var mpAbilityThreshold = new MenuItem("ptMpAbilityThreshold", "MP ability threshold").SetValue(new Slider(25));
            mpAbilityThreshold.SetTooltip("Use soul ring when ability costs more mp");
            menu.AddItem(mpAbilityThreshold);
            mpAbilityThreshold.ValueChanged += (sender, args) => MpAbilityThreshold = args.GetNewValue<Slider>().Value;
            MpAbilityThreshold = mpAbilityThreshold.GetValue<Slider>().Value;

            menu.AddItem(
                new MenuItem("ptAbilities", "Enabled:").SetValue(abilityToggler = new AbilityToggler(new Dictionary<string, bool>())));

            mainMenu.AddSubMenu(menu);
        }

        public event EventHandler<BoolEventArgs> OnEnabledChange;

        public bool IsEnabled { get; private set; }

        public int MpAbilityThreshold { get; private set; }

        public int SwitchOnAttackAttribute { get; private set; }

        public bool SwitchOnHeal { get; private set; }

        public int SwitchOnMoveAttribute { get; private set; }

        public bool UniversalUseEnabled { get; private set; }

        public void AddAbility(string abilityName, bool enabled)
        {
            abilityToggler.Add(abilityName, enabled);
        }

        public bool IsAbilityEnabled(string abilityName)
        {
            return abilityToggler.IsEnabled(abilityName);
        }

        public void RemoveAbility(string abilityName)
        {
            abilityToggler.Remove(abilityName);
        }
    }
}