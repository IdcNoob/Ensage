namespace Evader.EvadableAbilities.Heroes
{
    using Base;

    using Ensage;
    using Ensage.Common.Extensions;

    using SharpDX;

    using static Core.Abilities;

    internal class Eclipse : AOE
    {
        #region Fields

        private readonly float[] eclipseTimings = { 2.4f, 4.2f, 6f, 1.8f, 3.6f, 5.4f };

        #endregion

        #region Constructors and Destructors

        public Eclipse(Ability ability)
            : base(ability)
        {
            BlinkAbilities.AddRange(BlinkAbilityNames);
            DisableAbilities.AddRange(DisableAbilityNames);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(TricksOfTheTrade);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(Invul);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.AddRange(Invis);

            IgnorePathfinder = true;
        }

        #endregion

        #region Public Methods and Operators

        public override void Check()
        {
            var time = Game.RawGameTime;

            if (IsInPhase && StartCast + CastPoint <= time)
            {
                StartCast = time;
                Position = Owner.NetworkPosition;
                EndCast = StartCast + GetEclipseTime() + CastPoint;
                Obstacle = Pathfinder.AddObstacle(Position, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && time > EndCast)
            {
                End();
            }
            else if (Obstacle != null && !IsInPhase)
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

        public override float GetRemainingTime(Hero hero = null)
        {
            return EndCast - Game.RawGameTime;
        }

        public override bool IgnoreRemainingTime(float remainingTime = 0)
        {
            return Owner.HasModifier("modifier_luna_eclipse");
        }

        #endregion

        #region Methods

        private float GetEclipseTime()
        {
            return eclipseTimings[Ability.Level + (Owner.AghanimState() ? 2 : -1)];
        }

        #endregion
    }
}