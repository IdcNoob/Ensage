namespace ItemManager.Menus.Modules.AutoActions.HpMpRestore
{
    using Ensage.Common.Menu;

    internal class LivingArmorMenu
    {
        public LivingArmorMenu(Menu rootMenu)
        {
            var menu = new Menu("Living armor", "livingArmor");

            var enabled = new MenuItem("livingArmorEnabled", "Enabled").SetValue(true);
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) => IsEnabled = args.GetNewValue<bool>();
            IsEnabled = enabled.IsActive();

            var notification = new MenuItem("livingArmorNotification", "Show notification").SetValue(true);
            notification.SetTooltip("Print in chat message on who living armor was used (visible only to you)");
            menu.AddItem(notification);
            notification.ValueChanged += (sender, args) => IsEnabledNotification = args.GetNewValue<bool>();
            IsEnabledNotification = notification.IsActive();

            var heroMenu = new Menu("Hero", "livingArmorHero");

            var heroEnabled = new MenuItem("livingArmorHeroEnabled", "Enabled").SetValue(true);
            heroMenu.AddItem(heroEnabled);
            heroEnabled.ValueChanged += (sender, args) => IsEnabledHero = args.GetNewValue<bool>();
            IsEnabledHero = heroEnabled.IsActive();

            var heroHpThreshold = new MenuItem("livingArmorHeroHp", "Hero HP% threshold").SetValue(new Slider(70));
            heroHpThreshold.SetTooltip("Use living armor if your ally has less hp%");
            heroMenu.AddItem(heroHpThreshold);
            heroHpThreshold.ValueChanged += (sender, args) => HeroHpThreshold = args.GetNewValue<Slider>().Value;
            HeroHpThreshold = heroHpThreshold.GetValue<Slider>().Value;

            var enemyCheckRange =
                new MenuItem("livingArmorHeroEnemyRange", "Enemy search range").SetValue(new Slider(700, 0, 2000));
            enemyCheckRange.SetTooltip(
                "Use living armor only if there is enemy hero in range (if set to 0 range won't be checked)");
            heroMenu.AddItem(enemyCheckRange);
            enemyCheckRange.ValueChanged += (sender, args) => HeroEnemySearchRange = args.GetNewValue<Slider>().Value;
            HeroEnemySearchRange = enemyCheckRange.GetValue<Slider>().Value;

            var towerMenu = new Menu("Tower", "livingArmorTower");

            var towerEnabled = new MenuItem("livingArmorTowerEnabled", "Enabled").SetValue(true);
            towerMenu.AddItem(towerEnabled);
            towerEnabled.ValueChanged += (sender, args) => IsEnabledTower = args.GetNewValue<bool>();
            IsEnabledTower = towerEnabled.IsActive();

            var towerHpThreshold = new MenuItem("livingArmorTowerHp", "Tower HP% threshold").SetValue(new Slider(60));
            towerHpThreshold.SetTooltip("Use living armor if ally tower has less hp%");
            towerMenu.AddItem(towerHpThreshold);
            towerHpThreshold.ValueChanged += (sender, args) => TowerHpThreshold = args.GetNewValue<Slider>().Value;
            TowerHpThreshold = towerHpThreshold.GetValue<Slider>().Value;

            var creepMenu = new Menu("Creep", "livingArmorCreep");

            var creepEnabled = new MenuItem("livingArmorCreepEnabled", "Enabled").SetValue(false);
            creepMenu.AddItem(creepEnabled);
            creepEnabled.ValueChanged += (sender, args) => IsEnabledCreep = args.GetNewValue<bool>();
            IsEnabledCreep = creepEnabled.IsActive();

            var useOnCreepsUnderTower = new MenuItem("livingArmorCreepForce", "Force use under tower").SetValue(true);
            useOnCreepsUnderTower.SetTooltip("Always use living armor on creep which are attacked by tower");
            creepMenu.AddItem(useOnCreepsUnderTower);
            useOnCreepsUnderTower.ValueChanged += (sender, args) => IsEnabledCreepUnderTower = args.GetNewValue<bool>();
            IsEnabledCreepUnderTower = useOnCreepsUnderTower.IsActive();

            var creepHpThreshold = new MenuItem("livingArmorCreepHp", "Creep HP% threshold").SetValue(new Slider(40));
            creepHpThreshold.SetTooltip("Use living armor if ally creep has less hp%");
            creepMenu.AddItem(creepHpThreshold);
            creepHpThreshold.ValueChanged += (sender, args) => CreepHpThreshold = args.GetNewValue<Slider>().Value;
            CreepHpThreshold = creepHpThreshold.GetValue<Slider>().Value;

            menu.AddSubMenu(heroMenu);
            menu.AddSubMenu(towerMenu);
            menu.AddSubMenu(creepMenu);

            rootMenu.AddSubMenu(menu);
        }

        public int CreepHpThreshold { get; private set; }

        public int HeroEnemySearchRange { get; private set; }

        public int HeroHpThreshold { get; private set; }

        public bool IsEnabled { get; private set; }

        public bool IsEnabledCreep { get; private set; }

        public bool IsEnabledCreepUnderTower { get; private set; }

        public bool IsEnabledHero { get; private set; }

        public bool IsEnabledNotification { get; private set; }

        public bool IsEnabledTower { get; private set; }

        public int TowerHpThreshold { get; private set; }
    }
}