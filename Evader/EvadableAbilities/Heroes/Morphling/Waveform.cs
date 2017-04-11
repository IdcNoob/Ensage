namespace Evader.EvadableAbilities.Heroes.Morphling
{
    using Base.Interfaces;

    using Common;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class Waveform : LinearProjectile, IModifierObstacle
    {
        private Modifier waveModifier;

        public Waveform(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }

        public void AddModifierObstacle(Modifier modifier, Unit unit)
        {
            waveModifier = modifier;
        }

        public override void Check()
        {
            if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + CastPoint + AdditionalDelay + GetCastRange() / GetProjectileSpeed();
            }
            else if (StartCast > 0 && Obstacle == null && CanBeStopped() && !AbilityOwner.IsTurning())
            {
                StartPosition = AbilityOwner.NetworkPosition;
                EndPosition = AbilityOwner.InFront(GetCastRange() + GetRadius() / 2);
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), GetEndRadius(), Obstacle);
            }
            else if (StartCast > 0 && (Game.RawGameTime > EndCast || waveModifier != null && !waveModifier.IsValid))
            {
                End();
            }
            else if (Obstacle != null && !CanBeStopped())
            {
                Pathfinder.UpdateObstacle(Obstacle.Value, GetProjectilePosition(), GetRadius(), GetEndRadius());
            }
        }

        public override void End()
        {
            base.End();
            waveModifier = null;
        }
    }
}