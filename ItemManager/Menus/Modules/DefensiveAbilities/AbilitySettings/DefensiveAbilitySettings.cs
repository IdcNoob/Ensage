namespace ItemManager.Menus.Modules.DefensiveAbilities.AbilitySettings
{
    using Ensage.Common.Menu;

    internal class DefensiveAbilitySettings
    {
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

            mainMenu.AddSubMenu(menu);
        }

        public bool BladeMailStack { get; private set; }

        public int Delay { get; protected set; }

        public int EnemyCount { get; protected set; }

        public bool LotusOrbStack { get; private set; }

        public bool MagicImmunityStack { get; private set; }

        public int Range { get; protected set; }
    }
}