namespace Evader.EvadableAbilities.Heroes
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Core;

    using Ensage;
    using Ensage.Common.Extensions;

    using SharpDX;

    using Utils;

    using static Core.Abilities;

    internal class Chronosphere : LinearAOE, IModifier
    {
        #region Fields

        private readonly float[] duration = new float[3];

        private bool modifierAdded;

        private Vector3 position;

        #endregion

        #region Constructors and Destructors

        public Chronosphere(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.AddRange(StrongDefUltimates);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.Add(SnowBall);

            CounterAbilities.Remove("slark_dark_pact");
            BlinkAbilities.Remove("slark_pounce");

            if (Owner.Team == Variables.HeroTeam)
            {
                // leave only blink abilities
                // if void is ally
                CounterAbilities.Clear();
                DisableAbilities.Clear();
            }

            for (var i = 0u; i < 3; i++)
            {
                duration[i] = ability.AbilitySpecialData.First(x => x.Name == "duration").GetValue(i);
            }

            ObstacleStays = true;
        }

        #endregion

        #region Public Methods and Operators

        public void AddModifier(Modifier mod, Unit unit)
        {
            position = unit.Position;
            modifierAdded = true;

            if (Particle == null)
            {
                Particle = new ParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf", position);
                Particle.SetControlPoint(1, new Vector3(255, 0, 0));
                Particle.SetControlPoint(2, new Vector3(GetRadius(), 255, 0));
                Particle.SetControlPoint(0, position);
                Obstacle = Pathfinder.AddObstacle(position, GetRadius(), Obstacle);
            }
        }

        public override bool CanBeStopped()
        {
            return !modifierAdded && base.CanBeStopped();
        }

        public override void Check()
        {
            var time = Game.RawGameTime;
            var phase = IsInPhase;

            if (phase && StartCast + CastPoint <= time)
            {
                StartCast = time;
                EndCast = StartCast + CastPoint + GetDuration();
            }
            else if (phase && Obstacle == null && (int)Owner.RotationDifference == 0)
            {
                StartPosition = Owner.NetworkPosition;
                EndPosition = Owner.InFront(GetCastRange() + GetRadius());
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && time > EndCast)
            {
                End();
            }
        }

        public override void Draw()
        {
            if (Obstacle == null)
            {
                return;
            }

            if (!modifierAdded)
            {
                Utils.DrawRectangle(StartPosition, EndPosition, GetRadius());
            }

            Vector2 textPosition;
            Drawing.WorldToScreen(StartPosition, out textPosition);
            Drawing.DrawText(
                GetRemainingTime().ToString("0.00"),
                "Arial",
                textPosition,
                new Vector2(20),
                Color.White,
                FontFlags.None);
        }

        public override void End()
        {
            if (Obstacle == null)
            {
                return;
            }

            base.End();
            modifierAdded = false;
        }

        #endregion

        #region Methods

        protected override float GetRadius()
        {
            return base.GetRadius() + 75;
        }

        private float GetDuration()
        {
            return duration[Ability.Level - 1];
        }

        #endregion
    }
}