namespace ItemManager.Menus.Modules.Snatcher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Menu;

    using EventArgs;

    internal class SnatcherMenu
    {
        private readonly Dictionary<string, AbilityId> items = new Dictionary<string, AbilityId>
        {
            { "item_gem", AbilityId.item_gem },
            { "item_cheese", AbilityId.item_cheese },
            { "item_rapier", AbilityId.item_rapier },
            { "item_aegis", AbilityId.item_aegis },
            { "rune_doubledamage", 0 }
        };

        public SnatcherMenu(Menu mainMenu)
        {
            var menu = new Menu("Snatcher", "snatcher");

            var enabled = new MenuItem("snatcherEnabled", "Enabled").SetValue(true);
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) =>
                {
                    IsEnabled = args.GetNewValue<bool>();
                    OnEnabledChange?.Invoke(null, new BoolEventArgs(IsEnabled));
                };
            IsEnabled = enabled.IsActive();

            var holdKey = new MenuItem("holdSnatchKey", "Hold key").SetValue(new KeyBind('O', KeyBindType.Press));
            menu.AddItem(holdKey);
            holdKey.ValueChanged += (sender, args) => HoldKey = args.GetNewValue<KeyBind>().Active;
            HoldKey = holdKey.IsActive();

            var holdItems =
                new MenuItem("enabledStealHold", "Hold steal:").SetValue(
                    new AbilityToggler(items.ToDictionary(x => x.Key, x => true)));
            menu.AddItem(holdItems);
            holdItems.ValueChanged += (sender, args) =>
                {
                    SetEnabledItems(args.GetNewValue<AbilityToggler>().Dictionary, EnabledHoldItems);
                };

            var toggleKey = new MenuItem("pressSnatchKey", "Toggle key").SetValue(new KeyBind('P', KeyBindType.Toggle));
            menu.AddItem(toggleKey);
            toggleKey.ValueChanged += (sender, args) => ToggleKey = args.GetNewValue<KeyBind>().Active;
            ToggleKey = toggleKey.IsActive();

            var toggleItems =
                new MenuItem("enabledStealToggle", "Toggle steal:").SetValue(
                    new AbilityToggler(items.ToDictionary(x => x.Key, x => true)));
            menu.AddItem(toggleItems);
            toggleItems.ValueChanged += (sender, args) => SetEnabledItems(
                args.GetNewValue<AbilityToggler>().Dictionary,
                EnabledToggleItems);

            var otherUnits = new MenuItem("snatcherOtherUnits", "Use other units").SetValue(false)
                .SetTooltip("Like Spirit Bear, Meepo clones");
            menu.AddItem(otherUnits);
            otherUnits.ValueChanged += (sender, args) =>
                {
                    UseOtherUnits = args.GetNewValue<bool>();
                    OnUseOtherUnitsChange?.Invoke(null, new BoolEventArgs(UseOtherUnits));
                };
            UseOtherUnits = otherUnits.IsActive();

            var itemMoveCostThreshold =
                new MenuItem("snatcherMoveItemCost", "Move item cost threshold").SetValue(new Slider(1000, 0, 5000));
            itemMoveCostThreshold.SetTooltip(
                "It will move item from inventory (when full) to backpack which costs less gold to pick up aegis/rapier/gem (disabled: 0)");
            menu.AddItem(itemMoveCostThreshold);
            itemMoveCostThreshold.ValueChanged +=
                (sender, args) => ItemMoveCostThreshold = args.GetNewValue<Slider>().Value;
            ItemMoveCostThreshold = itemMoveCostThreshold.GetValue<Slider>().Value;

            var updateRate = new MenuItem("snatcherUpdateRate", "Update rate").SetValue(new Slider(1, 1, 500));
            updateRate.SetTooltip("Lower value => faster reaction, but requires more resources");
            menu.AddItem(updateRate);
            updateRate.ValueChanged += (sender, args) =>
                {
                    UpdateRate = args.GetNewValue<Slider>().Value;
                    OnUpdateRateChange?.Invoke(null, new IntEventArgs(UpdateRate));
                };
            UpdateRate = updateRate.GetValue<Slider>().Value;

            SetEnabledItems(holdItems.GetValue<AbilityToggler>().Dictionary, EnabledHoldItems);
            SetEnabledItems(toggleItems.GetValue<AbilityToggler>().Dictionary, EnabledToggleItems);

            mainMenu.AddSubMenu(menu);
        }

        public event EventHandler<BoolEventArgs> OnEnabledChange;

        public event EventHandler<IntEventArgs> OnUpdateRateChange;

        public event EventHandler<BoolEventArgs> OnUseOtherUnitsChange;

        public List<AbilityId> EnabledHoldItems { get; } = new List<AbilityId>();

        public List<AbilityId> EnabledToggleItems { get; } = new List<AbilityId>();

        public bool HoldKey { get; private set; }

        public bool IsEnabled { get; private set; }

        public int ItemMoveCostThreshold { get; private set; }

        public bool ToggleKey { get; private set; }

        public int UpdateRate { get; private set; }

        public bool UseOtherUnits { get; private set; }

        private void SetEnabledItems(IDictionary<string, bool> newValues, ICollection<AbilityId> enabledItems)
        {
            enabledItems.Clear();

            foreach (var item in newValues.Where(x => x.Value).Select(x => x.Key))
            {
                enabledItems.Add(items.First(x => x.Key == item).Value);
            }
        }
    }
}