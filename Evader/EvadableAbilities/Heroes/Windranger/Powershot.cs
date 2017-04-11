namespace Evader.EvadableAbilities.Heroes.Windranger
{
    using Base.Interfaces;

    using Common;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using SharpDX;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class Powershot : LinearProjectile, IParticle
    {
        private readonly float channelTime;

        private bool channelFix;

        private float channelingTime;

        private bool particleAdded;

        public Powershot(Ability ability)
            : base(ability)
        {
            channelTime = ability.GetChannelTime(0);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }

        public void AddParticle(ParticleEffectAddedEventArgs particleArgs)
        {
            if (Obstacle != null || !AbilityOwner.IsVisible)
            {
                return;
            }

            StartCast = Game.RawGameTime;
            particleAdded = true;
        }

        public override bool CanBeStopped()
        {
            return false;
        }

        public override void Check()
        {
            if (particleAdded && !AbilityOwner.IsTurning())
            {
                StartPosition = AbilityOwner.NetworkPosition;
                EndPosition = AbilityOwner.InFront(GetCastRange());
                EndCast = StartCast + channelTime + GetCastRange() / GetProjectileSpeed();
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
                particleAdded = false;
            }
            else if (Obstacle != null && !Ability.IsChanneling && !channelFix)
            {
                var time = Game.RawGameTime;
                channelFix = true;
                channelingTime = time - StartCast;
                EndCast = time + GetCastRange() / GetProjectileSpeed();
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
            else if (Obstacle != null && !Ability.IsChanneling)
            {
                Pathfinder.UpdateObstacle(Obstacle.Value, GetProjectilePosition(), EndPosition);
            }
        }

        public override void End()
        {
            if (Obstacle == null)
            {
                return;
            }

            base.End();
            channelFix = false;
            channelingTime = 0;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            var position = hero.NetworkPosition;

            if (IsInPhase && position.Distance2D(StartPosition) <= GetRadius())
            {
                return StartCast + CastPoint + channelTime - Game.RawGameTime;
            }

            return StartCast + CastPoint + (Ability.IsChanneling ? channelTime : channelingTime)
                   + (position.Distance2D(StartPosition) - GetProjectileRadius(position) - 60) / GetProjectileSpeed()
                   - Game.RawGameTime;
        }

        protected override Vector3 GetProjectilePosition(bool ignoreCastPoint = false)
        {
            return Ability.IsChanneling
                       ? StartPosition
                       : StartPosition.Extend(
                           EndPosition,
                           (Game.RawGameTime - StartCast - channelingTime) * GetProjectileSpeed());
        }
    }
}