namespace ItemManager.Menus.Modules.AutoActions.Actions
{
    using System;
    using System.Collections.Generic;

    using Ensage.Common.Menu;

    using EventArgs;

    internal class SoulRingMenu
    {
        private AbilityToggler abilityToggler;

        public SoulRingMenu(Menu mainMenu)
        {
            var menu = new Menu("Soul ring", "soulRing");

            var enabled = new MenuItem("autoSoulRing", "Enabled").SetValue(true);
            enabled.SetTooltip("Auto soul ring when using abilities");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) =>
                {
                    IsEnabled = args.GetNewValue<bool>();
                    OnEnabledChange?.Invoke(null, new BoolEventArgs(IsEnabled));
                };
            IsEnabled = enabled.IsActive();

            var universal = new MenuItem("srUniversalUse", "Universal use").SetValue(true);
            universal.SetTooltip("If enabled soul ring will work with all other assemblies otherwise only when player used ability");
            menu.AddItem(universal);
            universal.ValueChanged += (sender, args) => UniversalUseEnabled = args.GetNewValue<bool>();
            UniversalUseEnabled = universal.IsActive();

            var invis = new MenuItem("srUseWhenInvis", "Use when invisible").SetValue(false);
            invis.SetTooltip("Use soul ring if your hero is invisible");
            menu.AddItem(invis);
            invis.ValueChanged += (sender, args) => UseWhenInvisible = args.GetNewValue<bool>();
            UseWhenInvisible = invis.IsActive();

            var hpThreshold = new MenuItem("soulRingHpThreshold", "HP% threshold").SetValue(new Slider(70));
            hpThreshold.SetTooltip("Use soul ring if you have more hp%");
            menu.AddItem(hpThreshold);
            hpThreshold.ValueChanged += (sender, args) => HpThreshold = args.GetNewValue<Slider>().Value;
            HpThreshold = hpThreshold.GetValue<Slider>().Value;

            var mpThreshold = new MenuItem("soulRingMpThreshold", "MP% threshold").SetValue(new Slider(100));
            mpThreshold.SetTooltip("Use soul ring if you have less mp%");
            menu.AddItem(mpThreshold);
            mpThreshold.ValueChanged += (sender, args) => MpThreshold = args.GetNewValue<Slider>().Value;
            MpThreshold = mpThreshold.GetValue<Slider>().Value;

            var mpAbilityThreshold = new MenuItem("srMpAbilityThreshold", "MP ability threshold").SetValue(new Slider(25));
            mpAbilityThreshold.SetTooltip("Use soul ring when ability costs more mp");
            menu.AddItem(mpAbilityThreshold);
            mpAbilityThreshold.ValueChanged += (sender, args) => MpAbilityThreshold = args.GetNewValue<Slider>().Value;
            MpAbilityThreshold = mpAbilityThreshold.GetValue<Slider>().Value;

            menu.AddItem(
                new MenuItem("soulRingAbilities", "Enabled:").SetValue(
                    abilityToggler = new AbilityToggler(new Dictionary<string, bool>())));

            mainMenu.AddSubMenu(menu);
        }

        public event EventHandler<BoolEventArgs> OnEnabledChange;

        public int HpThreshold { get; private set; }

        public bool IsEnabled { get; private set; }

        public int MpAbilityThreshold { get; private set; }

        public int MpThreshold { get; private set; }

        public bool UniversalUseEnabled { get; private set; }

        public bool UseWhenInvisible { get; private set; }

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