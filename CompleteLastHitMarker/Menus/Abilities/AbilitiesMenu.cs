namespace CompleteLastHitMarker.Menus.Abilities
{
    using System.Collections.Generic;

    using Ensage.Common.Menu;

    internal class AbilitiesMenu
    {
        private AbilityToggler abilityToggler;

        private PriorityChanger priorityChanger;

        public AbilitiesMenu(Menu rootMenu)
        {
            var menu = new Menu("Abilities", "abilities");

            AbilitiesColor = new AbilitiesColor(menu);
            Texture = new Texture(menu);

            var enabled = new MenuItem("abilitiesEnabled", "Enabled").SetValue(true);
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) => IsEnabled = args.GetNewValue<bool>();
            IsEnabled = enabled.IsActive();

            var sumDamage = new MenuItem("abilitiesSumDamage", "Sum damage").SetValue(true);
            sumDamage.SetTooltip("If enabled it will sum damage from all abilities otherwise only from one");
            menu.AddItem(sumDamage);
            sumDamage.ValueChanged += (sender, args) => SumDamage = args.GetNewValue<bool>();
            SumDamage = sumDamage.IsActive();

            var showBorder = new MenuItem("abilitiesBorder", "Show border").SetValue(true);
            showBorder.SetTooltip("Show border when unit can be killed");
            menu.AddItem(showBorder);
            showBorder.ValueChanged += (sender, args) => ShowBorder = args.GetNewValue<bool>();
            ShowBorder = showBorder.IsActive();

            var showWarningBorder = new MenuItem("abilitiesWarningBorder", "Show warning border").SetValue(true);
            showWarningBorder.SetTooltip(
                "Show texture and border when unit requires one more auto attack hit before he can be killed");
            menu.AddItem(showWarningBorder);
            showWarningBorder.ValueChanged += (sender, args) => ShowWarningBorder = args.GetNewValue<bool>();
            ShowWarningBorder = showWarningBorder.IsActive();

            menu.AddItem(
                new MenuItem("abilitiesToggler", "Abilities:").SetValue(
                    abilityToggler = new AbilityToggler(new Dictionary<string, bool>())));

            menu.AddItem(
                new MenuItem("abilitiesPriority", "Order:").SetValue(
                    priorityChanger = new PriorityChanger(new List<string>())));

            rootMenu.AddSubMenu(menu);
        }

        public AbilitiesColor AbilitiesColor { get; }

        public bool IsEnabled { get; private set; }

        public bool ShowBorder { get; private set; }

        public bool ShowWarningBorder { get; private set; }

        public bool SumDamage { get; private set; }

        public Texture Texture { get; }

        public void AddAbility(string abilityName)
        {
            abilityToggler.Add(abilityName);
            priorityChanger.Add(abilityName);
        }

        public uint GetAbilityPriority(string itemName)
        {
            return priorityChanger.GetPriority(itemName);
        }

        public bool IsAbilityEnabled(string itemName)
        {
            return abilityToggler.IsEnabled(itemName);
        }
    }
}