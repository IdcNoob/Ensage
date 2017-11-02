namespace ItemManager.Menus.Modules.DefensiveAbilities.AbilitySettings
{
    using System.Collections.Generic;

    using Ensage.Common.Menu;
    using Ensage.Common.Menu.MenuItems;

    internal class DefensiveAbilitySettings
    {
        private readonly Dictionary<string, bool> heroToggler = new Dictionary<string, bool>();

        public DefensiveAbilitySettings(Menu mainMenu, string name, string texture = null)
        {
            var simpleName = name.ToLower().Replace(" ", string.Empty);
            var menu = new Menu(" " + name, simpleName + "DefensiveSettings", false, texture, true);

            var range = new MenuItem(simpleName + "DefRange", "Enemies in range").SetValue(new Slider(450, 100, 1000));
            menu.AddItem(range);
            range.ValueChanged += (sender, args) => Range = args.GetNewValue<Slider>().Value;
            Range = range.GetValue<Slider>().Value;

            var count = new MenuItem(simpleName + "DefEnemyCount", "Enemy count").SetValue(new Slider(2, 1, 5));
            menu.AddItem(count);
            count.ValueChanged += (sender, args) => EnemyCount = args.GetNewValue<Slider>().Value;
            EnemyCount = count.GetValue<Slider>().Value;

            var delay = new MenuItem(simpleName + "DefDelay", "Delay (ms)").SetValue(new Slider(300, 0, 1000));
            delay.SetTooltip("Delay before use");
            menu.AddItem(delay);
            delay.ValueChanged += (sender, args) => Delay = args.GetNewValue<Slider>().Value;
            Delay = delay.GetValue<Slider>().Value;

            var magicStack = new MenuItem(simpleName + "DefMagicStack", "Stack with magic immunity").SetValue(false);
            menu.AddItem(magicStack);
            magicStack.ValueChanged += (sender, args) => MagicImmunityStack = args.GetNewValue<bool>();
            MagicImmunityStack = magicStack.IsActive();

            var bladeMailStack = new MenuItem(simpleName + "defBmStack", "Stack with blade mail").SetValue(false);
            menu.AddItem(bladeMailStack);
            bladeMailStack.ValueChanged += (sender, args) => BladeMailStack = args.GetNewValue<bool>();
            BladeMailStack = bladeMailStack.IsActive();

            var lotusStack = new MenuItem(simpleName + "DefLotusStack", "Stack with lotus orb").SetValue(false);
            menu.AddItem(lotusStack);
            lotusStack.ValueChanged += (sender, args) => LotusOrbStack = args.GetNewValue<bool>();
            LotusOrbStack = lotusStack.IsActive();

            var alwaysUse = new MenuItem(simpleName + "DefAlwaysUse", "Always use").SetValue(false);
            alwaysUse.SetTooltip("Will use item when selected enemy is range");
            menu.AddItem(alwaysUse);
            alwaysUse.ValueChanged += (sender, args) => AlwaysUse = args.GetNewValue<bool>();
            AlwaysUse = alwaysUse.IsActive();

            menu.AddItem(new EnemyHeroesToggler(simpleName + "defAlwaysOn", "When:", heroToggler, false));

            var alwayseUseRange = new MenuItem(simpleName + "defAlwaysUseRange", "Is in range").SetValue(new Slider(600, 100, 1500));
            menu.AddItem(alwayseUseRange);
            alwayseUseRange.ValueChanged += (sender, args) => AlwaysUseRange = args.GetNewValue<Slider>().Value;
            AlwaysUseRange = alwayseUseRange.GetValue<Slider>().Value;

            mainMenu.AddSubMenu(menu);
        }

        public bool AlwaysUse { get; private set; }

        public int AlwaysUseRange { get; protected set; }

        public bool BladeMailStack { get; private set; }

        public int Delay { get; protected set; }

        public int EnemyCount { get; protected set; }

        public bool LotusOrbStack { get; private set; }

        public bool MagicImmunityStack { get; private set; }

        public int Range { get; protected set; }

        public bool IsEnabled(string heroName)
        {
            bool enabled;
            heroToggler.TryGetValue(heroName, out enabled);
            return enabled;
        }
    }
}