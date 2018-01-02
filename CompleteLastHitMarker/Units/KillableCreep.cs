namespace CompleteLastHitMarker.Units
{
    using Base;

    using Ensage;
    using Ensage.Common;

    using SharpDX;

    using Utils;

    internal class KillableCreep : KillableUnit
    {
        public KillableCreep(Unit unit)
            : base(unit)
        {
            HpBarSize = new Vector2(HUDInfo.GetHPBarSizeX(Unit) - 26, HUDInfo.GetHpBarSizeY(Unit) / 2);
            DefaultTextureY = -50;
            UnitType = UnitType.Creep;
        }

        public override Vector2 HpBarPositionFix { get; } = new Vector2(13, 21);
    }
}