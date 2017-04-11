namespace Evader.EvadableAbilities.Heroes.Phoenix
{
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using Modifiers;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class FireSpirits : LinearProjectile, IParticle, IModifier
    {
        public FireSpirits(Ability ability)
            : base(ability)
        {
            DisableTimeSinceCastCheck = true;

            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            CounterAbilities.Add(PhaseShift);

            Modifier.AllyCounterAbilities.Add(Lotus);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(VsMagic);
        }

        public EvadableModifier Modifier { get; }

        public void AddParticle(ParticleEffectAddedEventArgs particleArgs)
        {
            StartCast = Game.RawGameTime;

            DelayAction.Add(
                1,
                () =>
                    {
                        StartPosition = particleArgs.ParticleEffect.GetControlPoint(0);
                        EndPosition = StartPosition.Extend(
                            StartPosition + particleArgs.ParticleEffect.GetControlPoint(1),
                            GetCastRange());
                        EndCast = StartCast + StartPosition.Distance2D(EndPosition) / GetProjectileSpeed();
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

        public override float GetRemainingDisableTime()
        {
            return 0;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            var position = hero.NetworkPosition;

            return StartCast + (position.Distance2D(StartPosition) - GetProjectileRadius(position))
                   / GetProjectileSpeed() - Game.RawGameTime + 0.05f;
        }
    }
}