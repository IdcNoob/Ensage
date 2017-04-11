namespace Debugger.Menus.Dumping
{
    using System;

    using Ensage.Common.Menu;

    using SharpDX;

    internal class Spells
    {
        public Spells(Menu mainMenu)
        {
            var menu = new Menu("Spells", "spellsDumpMenu");

            var dump = new MenuItem("spells", "Get unit spells").SetValue(false);
            menu.AddItem(dump);
            dump.ValueChanged += (sender, args) => { OnDump?.Invoke(this, EventArgs.Empty); };

            menu.AddItem(new MenuItem("spellsInfo", "Settings")).SetFontColor(Color.Yellow);

            var showHidden = new MenuItem("hiddenSpells", "Show hidden").SetValue(false);
            menu.AddItem(showHidden);
            showHidden.ValueChanged += (sender, args) => ShowHidden = args.GetNewValue<bool>();
            ShowHidden = showHidden.IsActive();

            var showTalents = new MenuItem("talentSpells", "Show talents").SetValue(false);
            menu.AddItem(showTalents);
            showTalents.ValueChanged += (sender, args) => ShowTalents = args.GetNewValue<bool>();
            ShowTalents = showTalents.IsActive();

            var showLevel = new MenuItem("levelSpells", "Show levels").SetValue(false);
            menu.AddItem(showLevel);
            showLevel.ValueChanged += (sender, args) => ShowLevels = args.GetNewValue<bool>();
            ShowLevels = showLevel.IsActive();

            var showManaCost = new MenuItem("manaSpells", "Show mana cost").SetValue(false);
            menu.AddItem(showManaCost);
            showManaCost.ValueChanged += (sender, args) => ShowManaCost = args.GetNewValue<bool>();
            ShowManaCost = showManaCost.IsActive();

            var showCastRange = new MenuItem("rangeSpells", "Show cast range").SetValue(false);
            menu.AddItem(showCastRange);
            showCastRange.ValueChanged += (sender, args) => ShowCastRange = args.GetNewValue<bool>();
            ShowCastRange = showCastRange.IsActive();

            var showBehavior = new MenuItem("behaviorSpells", "Show behavior").SetValue(false);
            menu.AddItem(showBehavior);
            showBehavior.ValueChanged += (sender, args) => ShowBehavior = args.GetNewValue<bool>();
            ShowBehavior = showBehavior.IsActive();

            var showTargetType = new MenuItem("targetTypeItems", "Show target type").SetValue(false);
            menu.AddItem(showTargetType);
            showTargetType.ValueChanged += (sender, args) => ShowTargetType = args.GetNewValue<bool>();
            ShowTargetType = showTargetType.IsActive();

            var showSpecialData = new MenuItem("specialSpells", "Show all special data").SetValue(false);
            menu.AddItem(showSpecialData);
            showSpecialData.ValueChanged += (sender, args) => ShowSpecialData = args.GetNewValue<bool>();
            ShowSpecialData = showSpecialData.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public bool ShowBehavior { get; private set; }

        public bool ShowCastRange { get; private set; }

        public bool ShowHidden { get; private set; }

        public bool ShowLevels { get; private set; }

        public bool ShowManaCost { get; private set; }

        public bool ShowSpecialData { get; private set; }

        public bool ShowTalents { get; private set; }

        public bool ShowTargetType { get; private set; }

        public event EventHandler OnDump;
    }
}