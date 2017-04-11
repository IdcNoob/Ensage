namespace Evader.EvadableAbilities.Heroes.Venomancer
{
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using Modifiers;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class VenomousGale : LinearProjectile, IParticle, IModifier
    {
        public VenomousGale(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(VsMagic);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
        }

        public EvadableModifier Modifier { get; }

        public void AddParticle(ParticleEffectAddedEventArgs particleArgs)
        {
            if (Obstacle != null || !AbilityOwner.IsVisible)
            {
                return;
            }

            StartCast = Game.RawGameTime;
            StartPosition = AbilityOwner.NetworkPosition;
            EndPosition = AbilityOwner.InFront(GetCastRange());
            EndCast = StartCast + GetCastRange() / GetProjectileSpeed();
            Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
        }

        public override bool CanBeStopped()
        {
            return false;
        }

        public override void Check()
        {
            if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
        }

        public override float GetProjectileSpeed()
        {
            return base.GetProjectileSpeed() + 100;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            if (hero.NetworkPosition.Distance2D(StartPosition) < GetRadius())
            {
                return 0;
            }

            return StartCast + (hero.NetworkPosition.Distance2D(StartPosition) - GetRadius()) / GetProjectileSpeed()
                   - Game.RawGameTime;
        }

        protected override float GetCastRange()
        {
            return base.GetCastRange() + 100;
        }
    }
}