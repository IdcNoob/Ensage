namespace ItemManager.Menus.Modules.Snatcher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Menu;

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

            var otherUnits = new MenuItem("otherUnits", "Use other units").SetValue(false)
                .SetTooltip("Like Spirit Bear, Meepo clones");
            menu.AddItem(otherUnits);
            otherUnits.ValueChanged += (sender, args) =>
                {
                    UseOtherUnits = args.GetNewValue<bool>();
                    OnUseOtherUnitsChange?.Invoke(this, EventArgs.Empty);
                };
            UseOtherUnits = otherUnits.IsActive();

            var delay = new MenuItem("sleep", "Check delay").SetValue(new Slider(200, 0, 500));
            menu.AddItem(delay);
            delay.ValueChanged += (sender, args) => Delay = args.GetNewValue<Slider>().Value;
            Delay = delay.GetValue<Slider>().Value;

            SetEnabledItems(holdItems.GetValue<AbilityToggler>().Dictionary, EnabledHoldItems);
            SetEnabledItems(toggleItems.GetValue<AbilityToggler>().Dictionary, EnabledToggleItems);

            mainMenu.AddSubMenu(menu);
        }

        public event EventHandler OnUseOtherUnitsChange;

        public int Delay { get; private set; }

        public List<AbilityId> EnabledHoldItems { get; } = new List<AbilityId>();

        public List<AbilityId> EnabledToggleItems { get; } = new List<AbilityId>();

        public bool HoldKey { get; private set; }

        public bool ToggleKey { get; private set; }

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