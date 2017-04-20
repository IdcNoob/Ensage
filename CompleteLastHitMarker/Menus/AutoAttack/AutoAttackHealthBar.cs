namespace CompleteLastHitMarker.Menus.AutoAttack
{
    using Ensage.Common.Menu;

    using SharpDX;

    using Units.Base;

    using Utils;

    internal class AutoAttackHealthBar
    {
        public AutoAttackHealthBar(Menu rootMenu)
        {
            var menu = new Menu("Health bar", "autoAttackHpBar");

            var creepMenu = new Menu("Creep", "autoAttackHpBarCreep");

            var xCreep = new MenuItem("hpBarXCreep", "X coordinate").SetValue(new Slider(0, -40, 40));
            creepMenu.AddItem(xCreep);
            xCreep.ValueChanged += (sender, args) => CreepX = args.GetNewValue<Slider>().Value;
            CreepX = xCreep.GetValue<Slider>().Value;

            var yCreep = new MenuItem("hpBarYCreep", "Y coordinate").SetValue(new Slider(0, -40, 40));
            creepMenu.AddItem(yCreep);
            yCreep.ValueChanged += (sender, args) => CreepY = args.GetNewValue<Slider>().Value;
            CreepY = yCreep.GetValue<Slider>().Value;

            var xSizeCreep = new MenuItem("hpBarSizeXCreep", "X size").SetValue(new Slider(0, 0, 60));
            creepMenu.AddItem(xSizeCreep);
            xSizeCreep.ValueChanged += (sender, args) => CreepSizeX = args.GetNewValue<Slider>().Value;
            CreepSizeX = xSizeCreep.GetValue<Slider>().Value;

            var ySizeCreep = new MenuItem("hpBarSizeYCreep", "Y size").SetValue(new Slider(0, 0, 10));
            creepMenu.AddItem(ySizeCreep);
            ySizeCreep.ValueChanged += (sender, args) => CreepSizeY = args.GetNewValue<Slider>().Value;
            CreepSizeY = ySizeCreep.GetValue<Slider>().Value;

            var towerMenu = new Menu("Tower", "autoAttackHpBarTower");

            var xTower = new MenuItem("hpBarXTower", "X coordinate").SetValue(new Slider(0, -40, 40));
            towerMenu.AddItem(xTower);
            xTower.ValueChanged += (sender, args) => TowerX = args.GetNewValue<Slider>().Value;
            TowerX = xTower.GetValue<Slider>().Value;

            var yTower = new MenuItem("hpBarYTower", "Y coordinate").SetValue(new Slider(0, -40, 40));
            towerMenu.AddItem(yTower);
            yTower.ValueChanged += (sender, args) => TowerY = args.GetNewValue<Slider>().Value;
            TowerY = yTower.GetValue<Slider>().Value;

            var xSizeTower = new MenuItem("hpBarSizeXTower", "X size").SetValue(new Slider(0, 0, 60));
            towerMenu.AddItem(xSizeTower);
            xSizeTower.ValueChanged += (sender, args) => TowerSizeX = args.GetNewValue<Slider>().Value;
            TowerSizeX = xSizeTower.GetValue<Slider>().Value;

            var ySizeTower = new MenuItem("hpBarSizeYTower", "Y size").SetValue(new Slider(0, 0, 10));
            towerMenu.AddItem(ySizeTower);
            ySizeTower.ValueChanged += (sender, args) => TowerSizeY = args.GetNewValue<Slider>().Value;
            TowerSizeY = ySizeTower.GetValue<Slider>().Value;

            menu.AddSubMenu(creepMenu);
            menu.AddSubMenu(towerMenu);

            rootMenu.AddSubMenu(menu);
        }

        public int CreepSizeX { get; private set; }

        public int CreepSizeY { get; private set; }

        public int CreepX { get; private set; }

        public int CreepY { get; private set; }

        public int TowerSizeX { get; private set; }

        public int TowerSizeY { get; private set; }

        public int TowerX { get; private set; }

        public int TowerY { get; private set; }

        public Vector2 GetHealthBarPosition(KillableUnit unit)
        {
            switch (unit.UnitType)
            {
                case UnitType.Creep:
                {
                    return unit.HpBarPosition + new Vector2(CreepX, CreepY);
                }
                case UnitType.Tower:
                {
                    return unit.HpBarPosition + new Vector2(TowerX, TowerY);
                }
                default:
                {
                    return unit.HpBarPosition;
                }
            }
        }

        public Vector2 GetHealthBarSize(KillableUnit unit)
        {
            switch (unit.UnitType)
            {
                case UnitType.Creep:
                {
                    return unit.HpBarSize + new Vector2(CreepSizeX, CreepSizeY);
                }
                case UnitType.Tower:
                {
                    return unit.HpBarSize + new Vector2(TowerSizeX, TowerSizeY);
                }
                default:
                {
                    return unit.HpBarSize;
                }
            }
        }
    }
}