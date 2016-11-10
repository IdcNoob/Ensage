namespace Evader.EvadableAbilities.Heroes.Alchemist
{
    using System.Linq;

    using Base.Interfaces;

    using Common;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using SharpDX;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class UnstableConcoction : Projectile, IModifier
    {
        #region Fields

        private readonly float stunRadius;

        private readonly Ability unstableConcoction;

        private Modifier abilityModifier;

        private Vector3 lastProjectilePosition;

        private Hero modifierSource;

        private bool obstacleToAOE;

        #endregion

        #region Constructors and Destructors

        public UnstableConcoction(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(Lotus);

            ModifierAllyCounter.AddRange(AllyShields);
            ModifierAllyCounter.AddRange(Invul);
            ModifierAllyCounter.AddRange(VsPhys);

            IsDisjointable = false;
            stunRadius = Ability.AbilitySpecialData.First(x => x.Name == "midair_explosion_radius").Value + 100;
            unstableConcoction = AbilityOwner.FindSpell("alchemist_unstable_concoction");
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

            ModifierHandle = modifier.Handle;
            abilityModifier = modifier;
            modifierSource = hero;
        }

        public bool CanBeCountered()
        {
            return abilityModifier != null && abilityModifier.IsValid;
        }

        public override bool CanBeStopped()
        {
            return !Ability.IsHidden && base.CanBeStopped();
        }

        public override void Check()
        {
            if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + CastPoint + GetCastRange() / GetProjectileSpeed();
                StartPosition = AbilityOwner.NetworkPosition;
                EndPosition = AbilityOwner.InFront(GetCastRange());
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
            }
            else if (ProjectileTarget != null && Obstacle == null && !FowCast)
            {
                FowCast = true;
                StartCast = Game.RawGameTime;
                EndCast = StartCast + GetCastRange() / GetProjectileSpeed();
                StartPosition = AbilityOwner.NetworkPosition;
                EndPosition = StartPosition.Extend(ProjectileTarget.Position, GetCastRange());
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
            else if (Obstacle != null)
            {
                if (!ProjectileLaunched())
                {
                    EndPosition = AbilityOwner.InFront(GetCastRange());
                    Pathfinder.UpdateObstacle(Obstacle.Value, StartPosition, EndPosition);
                    AbilityDrawer.UpdateRectaglePosition(StartPosition, EndPosition, GetRadius());
                }
                else if (ProjectileTarget != null)
                {
                    var projectilePosition = GetProjectilePosition(FowCast);

                    if (projectilePosition == lastProjectilePosition)
                    {
                        End();
                        return;
                    }

                    lastProjectilePosition = projectilePosition;

                    AbilityDrawer.Dispose(AbilityDrawer.Type.Rectangle);
                    EndCast = Game.RawGameTime
                              + (ProjectileTarget.Distance2D(projectilePosition) - 50) / GetProjectileSpeed();

                    if (!obstacleToAOE)
                    {
                        Obstacle = Pathfinder.AddObstacle(ProjectileTarget.Position, stunRadius, Obstacle);
                        obstacleToAOE = true;
                    }
                    else
                    {
                        Pathfinder.UpdateObstacle(Obstacle.Value, ProjectileTarget.Position, stunRadius);
                    }
                }
            }
        }

        public override void End()
        {
            if (Obstacle == null)
            {
                return;
            }

            base.End();

            obstacleToAOE = false;
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

            var position = ProjectileTarget?.NetworkPosition ?? hero.NetworkPosition;

            if (position.Distance2D(AbilityOwner) < 150)
            {
                return StartCast + CastPoint - Game.RawGameTime;
            }

            return EndCast - Game.RawGameTime;
        }

        public void RemoveModifier(Modifier modifier)
        {
            abilityModifier = null;
            modifierSource = null;
        }

        public override float TimeSinceCast()
        {
            if (unstableConcoction.Level <= 0 || !AbilityOwner.IsVisible)
            {
                return float.MaxValue;
            }

            var cooldownLength = unstableConcoction.CooldownLength;
            return cooldownLength <= 0 ? float.MaxValue : cooldownLength - unstableConcoction.Cooldown - 5;
        }

        #endregion
    }
}