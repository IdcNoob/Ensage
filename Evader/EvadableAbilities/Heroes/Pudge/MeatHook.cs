namespace Evader.EvadableAbilities.Heroes.Pudge
{
    using System.Linq;

    using Base.Interfaces;

    using Common;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using SharpDX;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class MeatHook : LinearProjectile, IParticle, IModifier
    {
        #region Fields

        private Modifier abilityModifier;

        private Hero modifierSource;

        private bool particleAdded;

        private ParticleEffect particleEffect;

        #endregion

        #region Constructors and Destructors

        public MeatHook(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(TricksOfTheTrade);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(Invul);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.AddRange(Invis);

            ModifierAllyCounter.AddRange(AllyShields);
            ModifierAllyCounter.AddRange(Invul);
            ModifierAllyCounter.Add(Lotus);
            ModifierAllyCounter.AddRange(VsMagic);
        }

        #endregion

        #region Public Properties

        public uint ModifierHandle { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void AddModifer(Modifier modifier, Hero hero)
        {
            if (hero.Team != HeroTeam)
            {
                return;
            }

            abilityModifier = modifier;
            modifierSource = hero;
            ModifierHandle = modifier.Handle;
        }

        public void AddParticle(ParticleEffect particle)
        {
            particleEffect = particle;
        }

        public bool CanBeCountered()
        {
            return abilityModifier != null && abilityModifier.IsValid;
        }

        public override void Check()
        {
            if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + CastPoint + GetCastRange() / GetProjectileSpeed();
            }
            else if (StartCast > 0 && Obstacle == null && CanBeStopped() && !AbilityOwner.IsTurning())
            {
                StartPosition = AbilityOwner.NetworkPosition;
                EndPosition = AbilityOwner.InFront(GetCastRange() + GetRadius() / 2);
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
            }
            else if (Obstacle == null && particleEffect != null && !particleAdded)
            {
                particleAdded = true;
                StartPosition = particleEffect.GetControlPoint(0);
                EndPosition = StartPosition.Extend(particleEffect.GetControlPoint(1), GetCastRange() + GetRadius() / 2);
                StartCast = Game.RawGameTime;
                EndCast = StartCast + GetCastRange() / GetProjectileSpeed();
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast)
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
            if (Obstacle == null)
            {
                return;
            }

            base.End();

            particleEffect = null;
            particleAdded = false;
        }

        public float GetModiferRemainingTime()
        {
            return AbilityOwner.Distance2D(modifierSource) / GetProjectileSpeed();
        }

        public Hero GetModifierHero(ParallelQuery<Hero> allies)
        {
            return allies.FirstOrDefault(x => x.Equals(modifierSource));
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            var position = hero.NetworkPosition;

            if (particleAdded)
            {
                return StartCast + (position.Distance2D(StartPosition) - GetRadius() - 60) / GetProjectileSpeed()
                       - Game.RawGameTime;
            }

            if (IsInPhase && position.Distance2D(StartPosition) <= GetRadius())
            {
                return StartCast + CastPoint - Game.RawGameTime;
            }

            return StartCast + CastPoint
                   + (position.Distance2D(StartPosition) - GetRadius() - 60) / GetProjectileSpeed() - Game.RawGameTime;
        }

        public override bool IsStopped()
        {
            return !particleAdded && base.IsStopped();
        }

        public void RemoveModifier(Modifier modifier)
        {
            abilityModifier = null;
            modifierSource = null;
        }

        #endregion

        #region Methods

        protected override Vector3 GetProjectilePosition(bool ignoreCastPoint = false)
        {
            // only for drawings
            return base.GetProjectilePosition(particleAdded);
        }

        #endregion
    }
}