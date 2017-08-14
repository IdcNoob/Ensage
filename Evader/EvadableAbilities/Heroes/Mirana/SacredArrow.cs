namespace Evader.EvadableAbilities.Heroes.Mirana
{
    using Base.Interfaces;

    using Common;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using Modifiers;

    using SharpDX;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class SacredArrow : LinearProjectile, IModifier, IUnit
    {
        private readonly Ability talent;

        private Unit abilityUnit;

        private bool fowCast;

        public SacredArrow(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.ModifierSource);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Remove("abaddon_aphotic_shield");

            Modifier.AllyCounterAbilities.Add(Enrage);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(Invul);
            Modifier.AllyCounterAbilities.AddRange(VsMagic);

            talent = AbilityOwner.FindSpell("special_bonus_unique_mirana_2");
        }

        public EvadableModifier Modifier { get; }

        public void AddUnit(Unit unit)
        {
            abilityUnit = unit;

            if (AbilityOwner.IsVisible)
            {
                return;
            }

            StartCast = Game.RawGameTime;
            EndCast = StartCast + GetCastRange() / GetProjectileSpeed();
            StartPosition = unit.Position.SetZ(350);
            fowCast = true;
        }

        public override bool CanBeStopped()
        {
            return !fowCast && base.CanBeStopped();
        }

        public override void Check()
        {
            if (fowCast && Obstacle == null)
            {
                if (!IsAbilityUnitValid() || !abilityUnit.IsVisible)
                {
                    return;
                }

                var position = StartPosition.Extend(abilityUnit.Position.SetZ(350), GetCastRange());

                if (position.Distance2D(StartPosition) < 50)
                {
                    return;
                }

                EndPosition = position.SetZ(350);
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), GetEndRadius(), Obstacle);
            }
            else if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + CastPoint + GetCastRange() / GetProjectileSpeed();
            }
            else if (StartCast > 0 && Obstacle == null && CanBeStopped() && !AbilityOwner.IsTurning())
            {
                StartPosition = AbilityOwner.NetworkPosition;
                EndPosition = AbilityOwner.InFront(GetCastRange() + GetRadius() / 2);
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), GetEndRadius(), Obstacle);
            }
            else if (StartCast > 0 && (Game.RawGameTime > EndCast
                                       || Game.RawGameTime > StartCast + (fowCast ? 0 : CastPoint) + 0.1f
                                       && !IsAbilityUnitValid()))
            {
                End();
            }
            else if (Obstacle != null && !IsInPhase)
            {
                Pathfinder.UpdateObstacle(Obstacle.Value, GetProjectilePosition(), GetRadius(), GetEndRadius());
            }
        }

        public override void Draw()
        {
            if (Obstacle == null || !IsAbilityUnitValid() && fowCast)
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

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            var position = hero.NetworkPosition;

            if (IsInPhase && AbilityOwner.IsVisible && position.Distance2D(StartPosition) < GetRadius())
            {
                return StartCast + CastPoint - Game.RawGameTime;
            }

            return StartCast + (fowCast ? 0 : CastPoint)
                   + (position.Distance2D(StartPosition) - GetRadius()) / GetProjectileSpeed() - Game.RawGameTime;
        }

        public override bool IsStopped()
        {
            return StartCast > 0 && !IsInPhase && CanBeStopped();
        }

        protected override float GetEndRadius()
        {
            return talent?.Level > 0 ? 1500 : base.GetEndRadius();
        }

        protected override Vector3 GetProjectilePosition(bool ignoreCastPoint = false)
        {
            return base.GetProjectilePosition(fowCast);
        }

        private bool IsAbilityUnitValid()
        {
            return abilityUnit != null && abilityUnit.IsValid;
        }
    }
}