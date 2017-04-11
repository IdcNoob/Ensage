namespace Evader.EvadableAbilities.Heroes.EarthSpirit
{
    using System.Linq;

    using Base.Interfaces;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions.SharpDX;

    using SharpDX;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class RollingBoulder : LinearProjectile, IParticle
    {
        private float noStoneSpeed;

        public RollingBoulder(Ability ability)
            : base(ability)
        {
            AdditionalDelay = Ability.AbilitySpecialData.First(x => x.Name == "delay").Value;
            noStoneSpeed = Ability.AbilitySpecialData.First(x => x.Name == "speed").Value;

            CounterAbilities.Add(PhaseShift);
        }

        public void AddParticle(ParticleEffectAddedEventArgs particleArgs)
        {
            if (particleArgs.Name.Contains("_stone."))
            {
                return;
            }

            StartCast = Game.RawGameTime + AdditionalDelay;

            DelayAction.Add(
                1,
                () =>
                    {
                        StartPosition = particleArgs.ParticleEffect.GetControlPoint(0);
                        EndPosition = StartPosition.Extend(
                            particleArgs.ParticleEffect.GetControlPoint(1),
                            GetCastRange());
                        EndCast = StartCast + GetCastRange() / GetProjectileSpeed();
                        Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
                    });
        }

        public override bool CanBeStopped()
        {
            return false;
        }

        public override void Check()
        {
            if (StartCast > 0 && Obstacle != null && Game.RawGameTime > EndCast)
            {
                End();
            }
            else if (Obstacle != null)
            {
                Pathfinder.UpdateObstacle(Obstacle.Value, GetProjectilePosition(), GetRadius(), GetEndRadius());
            }
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return base.GetRemainingTime(hero) - AdditionalDelay;
        }

        protected override Vector3 GetProjectilePosition(bool ignoreCastPoint = false)
        {
            return StartCast > Game.RawGameTime
                       ? StartPosition
                       : StartPosition.Extend(EndPosition, (Game.RawGameTime - StartCast) * GetProjectileSpeed());
        }
    }
}