namespace Evader.EvadableAbilities.Heroes
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;

    using SharpDX;

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

            radius = Ability.AbilitySpecialData.First(x => x.Name == "max_range").Value + 50;
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
            if (Obstacle != null || !Owner.IsVisible)
            {
                return;
            }

            StartCast = Game.RawGameTime;
            Position = Owner.NetworkPosition;
            EndCast = StartCast + duration;
            Obstacle = Pathfinder.AddObstacle(Position, GetRadius(), Obstacle);
        }

        public override bool CanBeStopped()
        {
            return false;
        }

        public override void Check()
        {
            var time = Game.RawGameTime;

            if (StartCast > 0 && time > EndCast)
            {
                End();
            }
            else if (Obstacle != null)
            {
                Pathfinder.UpdateObstacle(Obstacle.Value, Owner.NetworkPosition, GetRadius());
            }
        }

        public override void Draw()
        {
            if (Obstacle == null)
            {
                return;
            }

            Vector2 textPosition;
            Drawing.WorldToScreen(Owner.NetworkPosition, out textPosition);
            Drawing.DrawText(
                GetRemainingTime().ToString("0.00"),
                "Arial",
                textPosition,
                new Vector2(20),
                Color.White,
                FontFlags.None);

            if (Particle == null)
            {
                Particle = new ParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf", Position);
                Particle.SetControlPoint(1, new Vector3(255, 0, 0));
                Particle.SetControlPoint(2, new Vector3(GetRadius() * -1, 255, 0));
            }

            Particle?.SetControlPoint(0, Owner.NetworkPosition);
        }

        public override bool IgnoreRemainingTime(float remainingTime = 0)
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