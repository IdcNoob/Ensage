namespace CompleteLastHitMarker.Units
{
    using Base;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions.Damage;

    using SharpDX;

    using Utils;

    internal class KillableTower : KillableUnit
    {
        public KillableTower(Unit unit)
            : base(unit)
        {
            HpBarSize = new Vector2(HUDInfo.GetHPBarSizeX(Unit) + 5, HUDInfo.GetHpBarSizeY(Unit) / 2);
            DefaultTextureY = -50;
            UnitType = UnitType.Tower;
        }

        public override Vector2 HpBarPositionFix { get; } = new Vector2(-2, -31);

        public Unit Target => ((Tower)Unit).AttackTarget;

        public float CalculateAverageDamageOn(KillableUnit unit)
        {
            return unit.Unit.DamageTaken(Unit.DamageAverage, DamageType.Physical, Unit)
                   * Damage.Multiplier(AttackDamageType.Siege, unit.ArmorType);
        }
    }
}