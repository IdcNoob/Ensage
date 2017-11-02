namespace ItemManager.Menus.Modules.AutoActions.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Menu;

    using EventArgs;

    internal class DewardingMenu
    {
        private AbilityToggler abilityToggler;

        private PriorityChanger priorityChanger;

        public DewardingMenu(Menu mainMenu)
        {
            var menu = new Menu("Dewarding", "dewardMenu");

            var enabled = new MenuItem("dewardEnabled", "Enabled").SetValue(true);
            enabled.SetTooltip("Auto use quelling blade, tangos etc. on wards");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) =>
                {
                    IsEnabled = args.GetNewValue<bool>();
                    OnEnabledChange?.Invoke(null, new BoolEventArgs(IsEnabled));
                };
            IsEnabled = enabled.IsActive();

            var tangoHpThreshold = new MenuItem("dewardTangoHp", "Tango HP threshold").SetValue(new Slider(150, 0, 250));
            tangoHpThreshold.SetTooltip("Use tango only if you are missing more hp");
            menu.AddItem(tangoHpThreshold);
            tangoHpThreshold.ValueChanged += (sender, args) => TangoHpThreshold = args.GetNewValue<Slider>().Value;
            TangoHpThreshold = tangoHpThreshold.GetValue<Slider>().Value;

            var updateRate = new MenuItem("dewardUpdateRate", "Update rate").SetValue(new Slider(200, 1, 500));
            updateRate.SetTooltip("Lower value => faster reaction, but requires more resources");
            menu.AddItem(updateRate);
            updateRate.ValueChanged += (sender, args) =>
                {
                    UpdateRate = args.GetNewValue<Slider>().Value;
                    OnUpdateRateChange?.Invoke(null, new IntEventArgs(UpdateRate));
                };
            UpdateRate = updateRate.GetValue<Slider>().Value;

            menu.AddItem(
                new MenuItem("dewardItemsToggler", "Items:").SetValue(
                    abilityToggler = new AbilityToggler(ItemsToUse.ToDictionary(x => x.ToString(), x => true))));

            menu.AddItem(
                new MenuItem("dewardPriority", "Order:").SetValue(
                    priorityChanger = new PriorityChanger(ItemsToUse.Select(x => x.ToString()).ToList())));

            mainMenu.AddSubMenu(menu);
        }

        public event EventHandler<BoolEventArgs> OnEnabledChange;

        public event EventHandler<IntEventArgs> OnUpdateRateChange;

        public bool IsEnabled { get; private set; }

        public HashSet<AbilityId> ItemsToUse { get; } = new HashSet<AbilityId>
        {
            AbilityId.item_quelling_blade,
            AbilityId.item_bfury,
            AbilityId.item_tango,
            AbilityId.item_tango_single
        };

        public int TangoHpThreshold { get; private set; }

        public int UpdateRate { get; private set; }

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