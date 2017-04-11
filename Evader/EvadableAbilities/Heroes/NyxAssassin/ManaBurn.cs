namespace Evader.EvadableAbilities.Heroes.NyxAssassin
{
    using Base;

    using Common;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    internal class ManaBurn : LinearTarget
    {
        public ManaBurn(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Add(Lotus);
        }

        public override void Check()
        {
            if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible
                && !AbilityOwner.HasModifier("modifier_nyx_assassin_burrow"))
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + CastPoint + AdditionalDelay;
            }
            else if (StartCast > 0 && Obstacle == null && CanBeStopped() && !AbilityOwner.IsTurning())
            {
                StartPosition = AbilityOwner.NetworkPosition;
                EndPosition = AbilityOwner.InFront(GetCastRange() + 150);
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
        }
    }
}