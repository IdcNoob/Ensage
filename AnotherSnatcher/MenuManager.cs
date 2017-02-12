namespace AnotherSnatcher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage.Common.Menu;

    internal class MenuManager
    {
        #region Fields

        private readonly Dictionary<string, uint> items = new Dictionary<string, uint>
        {
            { "item_gem", 30 },
            { "item_cheese", 33 },
            { "item_rapier", 133 },
            { "item_aegis", 117 },
            { "rune_doubledamage", 0 },
        };

        private readonly Menu menu;

        #endregion

        #region Constructors and Destructors

        public MenuManager()
        {
            menu = new Menu("Another Snatcher", "anotherSnatcher", true, "rune_doubledamage", true);

            var holdKey = new MenuItem("holdSnatchKey", "Hold key").SetValue(new KeyBind('O', KeyBindType.Press));
            menu.AddItem(holdKey);
            holdKey.ValueChanged += (sender, args) => HoldKey = args.GetNewValue<KeyBind>().Active;
            HoldKey = holdKey.IsActive();

            var holdItems =
                new MenuItem("enabledStealHold", "Hold steal:").SetValue(
                    new AbilityToggler(items.ToDictionary(x => x.Key, x => true)));
            menu.AddItem(holdItems);
            holdItems.ValueChanged +=
                (sender, args) => { SetEnabledItems(args.GetNewValue<AbilityToggler>().Dictionary, EnabledHoldItems); };

            var toggleKey = new MenuItem("pressSnatchKey", "Toggle key").SetValue(new KeyBind('P', KeyBindType.Toggle));
            menu.AddItem(toggleKey);
            toggleKey.ValueChanged += (sender, args) => ToggleKey = args.GetNewValue<KeyBind>().Active;
            ToggleKey = toggleKey.IsActive();

            var toggleItems =
                new MenuItem("enabledStealToggle", "Toggle steal:").SetValue(
                    new AbilityToggler(items.ToDictionary(x => x.Key, x => true)));
            menu.AddItem(toggleItems);
            toggleItems.ValueChanged +=
                (sender, args) => SetEnabledItems(args.GetNewValue<AbilityToggler>().Dictionary, EnabledToggleItems);

            var otherUnits =
                new MenuItem("otherUnits", "Use other units").SetValue(false)
                    .SetTooltip("Like Spirit Bear, Meepo clones");
            menu.AddItem(otherUnits);
            otherUnits.ValueChanged += (sender, args) => {
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

            menu.AddToMainMenu();
        }

        #endregion

        #region Public Events

        public event EventHandler OnUseOtherUnitsChange;

        #endregion

        #region Public Properties

        public int Delay { get; private set; }

        public List<uint> EnabledHoldItems { get; } = new List<uint>();

        public List<uint> EnabledToggleItems { get; } = new List<uint>();

        public bool HoldKey { get; private set; }

        public bool ToggleKey { get; private set; }

        public bool UseOtherUnits { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            menu.RemoveFromMainMenu();
        }

        #endregion

        #region Methods

        private void SetEnabledItems(IDictionary<string, bool> newValues, ICollection<uint> enabledItems)
        {
            enabledItems.Clear();

            foreach (var item in newValues.Where(x => x.Value).Select(x => x.Key))
            {
                enabledItems.Add(items.First(x => x.Key == item).Value);
            }
        }

        #endregion
    }
}