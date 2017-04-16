namespace CompleteLastHitMarker.Units
{
    using Base;

    using Core;

    using Ensage;
    using Ensage.Common;

    using Menus.Abilities;

    using SharpDX;

    internal class KillableTower : KillableUnit
    {
        private readonly Vector2 hpBarPositionFix;

        public KillableTower(Unit unit)
            : base(unit)
        {
            hpBarPositionFix = new Vector2(-2, -31);
            HpBarSize = new Vector2(HUDInfo.GetHPBarSizeX(Unit) + 5, HUDInfo.GetHpBarSizeY(Unit) / 2);
            DefaultTextureY = -50;
        }

        public override Vector2 HpBarPosition => HUDInfo.GetHPbarPosition(Unit) + hpBarPositionFix;

        public override void CalculateAbilityDamageTaken(MyHero hero, AbilitiesMenu menu)
        {
            // add tower damage abilities ?
        }
    }
}