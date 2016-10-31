namespace Evader.EvadableAbilities.Heroes.Mirana
{
    using System.Linq;

    using Base.Interfaces;

    using Common;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class SacredArrow : LinearProjectile, IModifier
    {
        #region Fields

        private Modifier abilityModifier;

        private Unit arrow;

        private bool fowCast;

        private Hero modifierSource;

        #endregion

        #region Constructors and Destructors

        public SacredArrow(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Remove("abaddon_aphotic_shield");

            ModifierAllyCounter.AddRange(AllyShields);
            ModifierAllyCounter.AddRange(Invul);
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

        public void AddUnit(Unit unit)
        {
            if (AbilityOwner.IsVisible)
            {
                return;
            }

            arrow = unit;
            StartCast = Game.RawGameTime;
            EndCast = Game.RawGameTime + GetCastRange() / GetProjectileSpeed();
            StartPosition = unit.Position.SetZ(AbilityOwner.Position.Z);
            fowCast = true;
        }

        public bool CanBeCountered()
        {
            return abilityModifier != null && abilityModifier.IsValid;
        }

        public override bool CanBeStopped()
        {
            return !IsValidArrow() && base.CanBeStopped();
        }

        public override void Check()
        {
            var time = Game.RawGameTime;
            var phase = IsInPhase && AbilityOwner.IsVisible;

            if (phase && StartCast + CastPoint <= time && time > EndCast)
            {
                StartCast = time;
                EndCast = StartCast + CastPoint + GetCastRange() / GetProjectileSpeed();
            }
            else if ((phase && Obstacle == null && !AbilityOwner.IsTurning()) || (fowCast && Obstacle == null))
            {
                if (fowCast && (!IsValidArrow() || !arrow.IsVisible))
                {
                    return;
                }

                if (fowCast)
                {
                    EndPosition = StartPosition.Extend(arrow.Position.SetZ(AbilityOwner.Position.Z), GetCastRange());

                    if (EndPosition.Distance2D(StartPosition) < 10)
                    {
                        return;
                    }
                }
                else
                {
                    StartPosition = AbilityOwner.NetworkPosition;
                    EndPosition = AbilityOwner.InFront(GetCastRange() + GetRadius() / 2);
                }

                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
            }
            else if ((StartCast > 0 && time > EndCast) || (fowCast && !IsValidArrow()))
            {
                End();
            }
            else if (Obstacle != null && !phase)
            {
                Pathfinder.UpdateObstacle(Obstacle.Value, GetProjectilePosition(fowCast), GetRadius());
            }
        }

        public override void Draw()
        {
            if (Obstacle == null || (!IsValidArrow() && fowCast))
            {
                return;
            }

            base.Draw();
        }

        public override void End()
        {
            if (Obstacle == null)
            {
                return;
            }

            base.End();
            fowCast = false;
        }

        public float GetModiferRemainingTime()
        {
            return abilityModifier.RemainingTime;
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

            if (!IsValidArrow())
            {
                if (IsInPhase && position.Distance2D(StartPosition) < GetRadius())
                {
                    return StartCast + CastPoint - Game.RawGameTime;
                }

                return StartCast + CastPoint
                       + (position.Distance2D(StartPosition) - GetRadius() * 2) / GetProjectileSpeed()
                       - Game.RawGameTime;
            }

            return StartCast + (position.Distance2D(StartPosition) - GetRadius() * 2) / GetProjectileSpeed()
                   - Game.RawGameTime;
        }

        public override bool IsStopped()
        {
            var check = !IsInPhase && CanBeStopped() && !fowCast;

            if (check)
            {
                End();
            }

            return check;
        }

        public void RemoveModifier(Modifier modifier)
        {
            abilityModifier = null;
            modifierSource = null;
        }

        #endregion

        #region Methods

        private bool IsValidArrow()
        {
            return arrow != null && arrow.IsValid;
        }

        #endregion
    }
}