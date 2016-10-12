namespace Evader.EvadableAbilities.Heroes
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;

    using UsableAbilities.Base;

    using static Core.Abilities;

    internal class WhirlingAxesMelee : AOE, IParticle
    {
        #region Fields

        private readonly float duration;

        private readonly float radius;

        #endregion

        #region Constructors and Destructors

        public WhirlingAxesMelee(Ability ability)
            : base(ability)
        {
            ModifierName = "modifier_troll_warlord_whirling_axes_blind";

            radius = Ability.AbilitySpecialData.First(x => x.Name == "max_range").Value + 60;
            duration = Ability.AbilitySpecialData.First(x => x.Name == "whirl_duration").Value;

            IgnorePathfinder = true;

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.Add(SnowBall);
        }

        #endregion

        #region Public Methods and Operators

        public void AddParticle(ParticleEffect particle)
        {
            if (Obstacle != null || !AbilityOwner.IsVisible)
            {
                return;
            }

            StartCast = Game.RawGameTime;
            StartPosition = AbilityOwner.NetworkPosition;
            EndCast = StartCast + duration;
            Obstacle = Pathfinder.AddObstacle(StartPosition, GetRadius(), Obstacle);
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
            else if (Obstacle != null)
            {
                Pathfinder.UpdateObstacle(Obstacle.Value, AbilityOwner.NetworkPosition, GetRadius());
            }
        }

        public override void Draw()
        {
            if (Obstacle == null)
            {
                return;
            }

            AbilityDrawer.DrawCircle(StartPosition, GetRadius());
            AbilityDrawer.UpdateCirclePosition(AbilityOwner.NetworkPosition);
        }

        public override bool IgnoreRemainingTime(UsableAbility ability, float remainingTime = 0)
        {
            return Obstacle != null;
        }

        #endregion

        #region Methods

        protected override float GetRadius()
        {
            return radius;
        }

        #endregion
    }
}