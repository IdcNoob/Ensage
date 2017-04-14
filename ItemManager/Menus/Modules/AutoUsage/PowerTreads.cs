namespace ItemManager.Menus.Modules.AutoUsage
{
    using System.Collections.Generic;

    using Ensage.Common.Menu;

    internal class PowerTreads
    {
        private AbilityToggler abilityToggler;

        public PowerTreads(Menu mainMenu)
        {
            var menu = new Menu("Power treads", "ptSwitcherMenu");

            var enabled = new MenuItem("ptSwitcherEnabled", "Enabled").SetValue(true);
            enabled.SetTooltip("Auto switch power treads when using abilities");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) => IsEnabled = args.GetNewValue<bool>();
            IsEnabled = enabled.IsActive();

            var universal = new MenuItem("ptUniversalUse", "Universal use").SetValue(true);
            universal.SetTooltip(
                "If enabled power treads will work with all other assemblies otherwise only when player used ability");
            menu.AddItem(universal);
            universal.ValueChanged += (sender, args) => UniversalUseEnabled = args.GetNewValue<bool>();
            UniversalUseEnabled = universal.IsActive();

            var switchOnHeal = new MenuItem("ptSwitchOnHeal", "Switch when healing").SetValue(true);
            switchOnHeal.SetTooltip("Bottle, flask, shrine");
            menu.AddItem(switchOnHeal);
            switchOnHeal.ValueChanged += (sender, args) => SwitchOnHeal = args.GetNewValue<bool>();
            SwitchOnHeal = switchOnHeal.IsActive();

            var mpAbilityThreshold =
                new MenuItem("ptMpAbilityThreshold", "MP ability threshold").SetValue(new Slider(25));
            mpAbilityThreshold.SetTooltip("Use soul ring when ability costs more mp");
            menu.AddItem(mpAbilityThreshold);
            mpAbilityThreshold.ValueChanged += (sender, args) => MpAbilityThreshold = args.GetNewValue<Slider>().Value;
            MpAbilityThreshold = mpAbilityThreshold.GetValue<Slider>().Value;

            menu.AddItem(
                new MenuItem("ptAbilities", "Enabled:").SetValue(
                    abilityToggler = new AbilityToggler(new Dictionary<string, bool>())));

            mainMenu.AddSubMenu(menu);
        }

        public bool IsEnabled { get; private set; }

        public int MpAbilityThreshold { get; private set; }

        public bool SwitchOnHeal { get; private set; }

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