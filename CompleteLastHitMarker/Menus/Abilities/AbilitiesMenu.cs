namespace CompleteLastHitMarker.Menus.Abilities
{
    using System.Collections.Generic;

    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    internal class AbilitiesMenu
    {
        private AbilityToggler abilityToggler;

        private PriorityChanger priorityChanger;

        public AbilitiesMenu(MenuFactory factory)
        {
            var subFactory = factory.Menu("Abilities");

            AbilitiesColor = new AbilitiesColor(subFactory);
            Texture = new Texture(subFactory);

            IsEnabled = subFactory.Item("Enabled", true);
            SumDamage = subFactory.Item("Sum damage", true);
            SumDamage.Item.SetTooltip("If enabled it will sum damage from all abilities otherwise only from one");
            ShowBorder = subFactory.Item("Show border", true);
            ShowBorder.Item.SetTooltip("Show border when unit can be killed");
            ShowWarningBorder = subFactory.Item("Show warning border", true);
            ShowWarningBorder.Item.SetTooltip(
                "Show texture and border when unit requires one more auto attack hit before he can be killed");

            subFactory.Item("Abilities:", abilityToggler = new AbilityToggler(new Dictionary<string, bool>()));
            subFactory.Item("Order:", priorityChanger = new PriorityChanger(new List<string>()));
        }

        public AbilitiesColor AbilitiesColor { get; }

        public MenuItem<bool> IsEnabled { get; }

        public MenuItem<bool> ShowBorder { get; }

        public MenuItem<bool> ShowWarningBorder { get; }

        public MenuItem<bool> SumDamage { get; }

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