namespace Evader.Core.Menus
{
    using Ensage.Common.Menu;

    internal class HotkeysMenu
    {
        #region Constructors and Destructors

        public HotkeysMenu(Menu rootMenu)
        {
            var menu = new Menu("Hotkeys", "hotkeys");

            var enabledEvader =
                new MenuItem("enable", "Enable evader").SetValue(new KeyBind(36, KeyBindType.Toggle, true));
            menu.AddItem(enabledEvader);
            enabledEvader.ValueChanged += (sender, args) => EnabledEvader = args.GetNewValue<KeyBind>().Active;
            EnabledEvader = enabledEvader.IsActive();

            var enabledPathfinder =
                new MenuItem("enablePathfinder", "Enable pathfinder").SetValue(
                    new KeyBind(35, KeyBindType.Toggle, true));
            menu.AddItem(enabledPathfinder);
            enabledPathfinder.ValueChanged += (sender, args) => EnabledPathfinder = args.GetNewValue<KeyBind>().Active;
            EnabledPathfinder = enabledPathfinder.IsActive();

            menu.AddItem(
                    new MenuItem("forceBlink", "Force blink").SetValue(new KeyBind(46, KeyBindType.Press))
                        .SetTooltip("Blink in front of your hero as soon as possible")).ValueChanged +=
                (sender, args) => ForceBlink = args.GetNewValue<KeyBind>().Active;

            rootMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Properties

        public bool EnabledEvader { get; private set; }

        public bool EnabledPathfinder { get; private set; }

        public bool ForceBlink { get; private set; }

        #endregion
    }
}