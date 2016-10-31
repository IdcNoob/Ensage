namespace Evader.EvadableAbilities.Heroes.Sven
{
    using System.Linq;

    using Base.Interfaces;

    using Common;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class StormHammer : Projectile, IModifier
    {
        #region Fields

        private readonly float modifierDuration;

        private readonly float stunRadius;

        private Modifier abilityModifier;

        private bool obstacleToAOE;

        #endregion

        #region Constructors and Destructors

        public StormHammer(Ability ability)
            : base(ability)
        {
            //todo add projectile aoe stun

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Add(Lotus);
            CounterAbilities.Remove("abaddon_aphotic_shield");

            ModifierAllyCounter.AddRange(AllyShields);
            ModifierAllyCounter.AddRange(Invul);
            ModifierAllyCounter.AddRange(VsMagic);

            modifierDuration = Ability.AbilitySpecialData.First(x => x.Name == "bolt_stun_duration").Value;
            stunRadius = Ability.AbilitySpecialData.First(x => x.Name == "bolt_aoe").Value + 100;
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
            ModifierHandle = modifier.Handle;
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
                    AbilityDrawer.Dispose(AbilityDrawer.Type.Rectangle);
                    EndCast = Game.RawGameTime
                              + (ProjectileTarget.Distance2D(GetProjectilePosition(FowCast)) - 20)
                              / GetProjectileSpeed();

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
            return modifierDuration - abilityModifier.ElapsedTime;
        }

        public Hero GetModifierHero(ParallelQuery<Hero> allies)
        {
            return
                allies.Where(x => x.HasModifier(abilityModifier.Name)).OrderByDescending(x => x.Health).FirstOrDefault();
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            var position = ProjectileTarget?.NetworkPosition ?? hero.NetworkPosition;

            if (position.Distance2D(AbilityOwner) < 250)
            {
                return StartCast + CastPoint - Game.RawGameTime;
            }

            return EndCast - Game.RawGameTime;
        }

        public void RemoveModifier(Modifier modifier)
        {
            abilityModifier = null;
        }

        #endregion
    }
}