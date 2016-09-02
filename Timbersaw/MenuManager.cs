namespace Timbersaw
{
    using System.Collections.Generic;

    using Ensage.Common.Menu;

    internal class MenuManager
    {
        #region Fields

        private readonly MenuItem centerHero;

        private readonly MenuItem enabled;

        private readonly MenuItem items;

        private readonly Menu menu;

        private readonly MenuItem safeChain;

        #endregion

        #region Constructors and Destructors

        public MenuManager(string heroName)
        {
            menu = new Menu("Timbersaw ?", "timbersawQuestionMark", true, heroName, true);

            menu.AddItem(enabled = new MenuItem("enabled", "Enabled").SetValue(true));
            menu.AddItem(safeChain = new MenuItem("safeChain", "Safecast chain").SetValue(true))
                .SetTooltip("Will prevent chain cast if it wont hit tree when used manually");
            menu.AddItem(centerHero = new MenuItem("centerHero", "Center camera").SetValue(false))
                .SetTooltip("Center camera on timbersaw when chase enabled");
            menu.AddItem(
                items = new MenuItem("itemToUse", "Use:").SetValue(
                    new AbilityToggler(
                            new Dictionary<string, bool>
                                {
                                    { "item_blink", true },
                                    { "item_shivas_guard", true },
                                    { "item_soul_ring", true },
                                })));
            menu.AddItem(new MenuItem("comboKey", "Chase").SetValue(new KeyBind('F', KeyBindType.Press)))
                .SetTooltip("Chase/Kill enemy using abilities and items")
                .ValueChanged += (sender, arg) => { ChaseEnabled = arg.GetNewValue<KeyBind>().Active; };
            menu.AddItem(new MenuItem("moveKey", "Move").SetValue(new KeyBind('G', KeyBindType.Press)))
                .SetTooltip("Move to mouse position using Chain and Blink")
                .ValueChanged += (sender, arg) => { MoveEnabled = arg.GetNewValue<KeyBind>().Active; };

            menu.AddToMainMenu();
        }

        #endregion

        #region Public Properties

        public bool ChaseEnabled { get; private set; }

        public bool IsCenterCameraEnabled => centerHero.GetValue<bool>();

        public bool IsEnabled => enabled.GetValue<bool>();

        public bool IsSafeChainEnabled => safeChain.GetValue<bool>();

        public bool MoveEnabled { get; private set; }

        #endregion

        #region Public Methods and Operators

        public bool IsItemEnabled(string itemName)
        {
            return items.GetValue<AbilityToggler>().IsEnabled(itemName);
        }

        public void OnClose()
        {
            menu.RemoveFromMainMenu();
        }

        #endregion
    }
}