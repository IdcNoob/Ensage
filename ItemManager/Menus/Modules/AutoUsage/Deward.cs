namespace ItemManager.Menus.Modules.AutoUsage
{
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Menu;

    internal class Deward
    {
        private AbilityToggler abilityToggler;

        private PriorityChanger priorityChanger;

        public Deward(Menu mainMenu)
        {
            var menu = new Menu("Dewarding", "dewardMenu");

            var enabled = new MenuItem("dewardEnabled", "Enabled").SetValue(true);
            enabled.SetTooltip("Auto use quelling blade, tangos, iron talon etc. enemy on wards");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) => IsEnabled = args.GetNewValue<bool>();
            IsEnabled = enabled.IsActive();

            var destroyMines = new MenuItem("dewardMines", "Destroy techies mines").SetValue(true);
            destroyMines.SetTooltip("Auto use quelling blade, iron talon etc. on techies mines");
            menu.AddItem(destroyMines);
            destroyMines.ValueChanged += (sender, args) => DestroyMines = args.GetNewValue<bool>();
            DestroyMines = destroyMines.IsActive();

            var tangoHpThreshold =
                new MenuItem("dewardTangoHp", "Tango HP threshold").SetValue(new Slider(150, 0, 250));
            tangoHpThreshold.SetTooltip("Use tango only if you are missing more hp");
            menu.AddItem(tangoHpThreshold);
            tangoHpThreshold.ValueChanged += (sender, args) => TangoHpThreshold = args.GetNewValue<Slider>().Value;
            TangoHpThreshold = tangoHpThreshold.GetValue<Slider>().Value;

            menu.AddItem(
                new MenuItem("dewardItemsToggler", "Items:").SetValue(
                    abilityToggler = new AbilityToggler(ItemsToUse.ToDictionary(x => x.ToString(), x => true))));

            menu.AddItem(
                new MenuItem("dewardPriority", "Order:").SetValue(
                    priorityChanger = new PriorityChanger(ItemsToUse.Select(x => x.ToString()).ToList())));

            mainMenu.AddSubMenu(menu);
        }

        public bool DestroyMines { get; private set; }

        public bool IsEnabled { get; private set; }

        public List<AbilityId> ItemsToUse { get; } = new List<AbilityId>
        {
            AbilityId.item_tango,
            AbilityId.item_tango_single,
            AbilityId.item_iron_talon,
            AbilityId.item_quelling_blade,
            AbilityId.item_bfury
        };

        public int TangoHpThreshold { get; private set; }

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