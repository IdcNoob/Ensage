namespace CompleteLastHitMarker.Menus.AutoAttack
{
    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    using SharpDX;

    using Units.Base;

    using Utils;

    internal class AutoAttackHealthBar
    {
        private readonly MenuItem<Slider> creepSizeX;

        private readonly MenuItem<Slider> creepSizeY;

        private readonly MenuItem<Slider> creepX;

        private readonly MenuItem<Slider> creepY;

        private readonly MenuItem<Slider> towerSizeX;

        private readonly MenuItem<Slider> towerSizeY;

        private readonly MenuItem<Slider> towerX;

        private readonly MenuItem<Slider> towerY;

        public AutoAttackHealthBar(MenuFactory factory)
        {
            var subFactory = factory.Menu("Health bar");

            var creepFactory = subFactory.Menu("Creep");
            creepX = creepFactory.Item("X coordinate", new Slider(0, -40, 40));
            creepY = creepFactory.Item("Y coordinate", new Slider(0, -40, 40));
            creepSizeX = creepFactory.Item("X size", new Slider(0, 0, 60));
            creepSizeY = creepFactory.Item("Y size", new Slider(0, 0, 10));

            var towerFactory = subFactory.Menu("Tower");
            towerX = towerFactory.Item("X coordinate", new Slider(0, -40, 40));
            towerY = towerFactory.Item("Y coordinate", new Slider(0, -40, 40));
            towerSizeX = towerFactory.Item("X size", new Slider(0, 0, 60));
            towerSizeY = towerFactory.Item("Y size", new Slider(0, 0, 10));
        }

        public Vector2 GetHealthBarPosition(KillableUnit unit)
        {
            var position = unit.HpBarPosition;
            if (position.IsZero)
            {
                return Vector2.Zero;
            }

            switch (unit.UnitType)
            {
                case UnitType.Creep:
                case UnitType.Courier:
                {
                    return position + new Vector2(creepX, creepY);
                }
                case UnitType.Tower:
                {
                    return position + new Vector2(towerX, towerY);
                }
                default:
                {
                    return position;
                }
            }
        }

        public Vector2 GetHealthBarSize(KillableUnit unit)
        {
            switch (unit.UnitType)
            {
                case UnitType.Creep:
                case UnitType.Courier:
                {
                    return unit.HpBarSize + new Vector2(creepSizeX, creepSizeY);
                }
                case UnitType.Tower:
                {
                    return unit.HpBarSize + new Vector2(towerSizeX, towerSizeY);
                }
                default:
                {
                    return unit.HpBarSize;
                }
            }
        }
    }
}