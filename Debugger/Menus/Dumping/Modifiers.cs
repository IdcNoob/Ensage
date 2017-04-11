namespace Debugger.Menus.Dumping
{
    using System;

    using Ensage.Common.Menu;

    using SharpDX;

    internal class Modifiers
    {
        public Modifiers(Menu mainMenu)
        {
            var menu = new Menu("Modifiers", "modifiersDumpMenu");

            var dump = new MenuItem("modifiers", "Get unit modifiers").SetValue(false);
            menu.AddItem(dump);
            dump.ValueChanged += (sender, args) => { OnDump?.Invoke(this, EventArgs.Empty); };

            menu.AddItem(new MenuItem("modifiersInfo", "Settings")).SetFontColor(Color.Yellow);

            var showHidden = new MenuItem("hiddenModifiers", "Show hidden").SetValue(false);
            menu.AddItem(showHidden);
            showHidden.ValueChanged += (sender, args) => ShowHidden = args.GetNewValue<bool>();
            ShowHidden = showHidden.IsActive();

            var showTextureName = new MenuItem("modifierTexture", "Show texture name").SetValue(false);
            menu.AddItem(showTextureName);
            showTextureName.ValueChanged += (sender, args) => ShowTextureName = args.GetNewValue<bool>();
            ShowTextureName = showTextureName.IsActive();

            var showRemainingTime = new MenuItem("modifierRemainingTime", "Show remaining time").SetValue(false);
            menu.AddItem(showRemainingTime);
            showRemainingTime.ValueChanged += (sender, args) => ShowRemainingTime = args.GetNewValue<bool>();
            ShowRemainingTime = showRemainingTime.IsActive();

            var showElapsedTime = new MenuItem("modifierShowElapsedTime", "Show elapsed time").SetValue(false);
            menu.AddItem(showElapsedTime);
            showElapsedTime.ValueChanged += (sender, args) => ShowElapsedTime = args.GetNewValue<bool>();
            ShowElapsedTime = showElapsedTime.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public bool ShowElapsedTime { get; private set; }

        public bool ShowHidden { get; private set; }

        public bool ShowRemainingTime { get; private set; }

        public bool ShowTextureName { get; private set; }

        public event EventHandler OnDump;
    }
}