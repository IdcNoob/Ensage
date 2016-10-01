namespace Evader.EvadableAbilities.Heroes
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using SharpDX;

    using Utils;

    using static Core.Abilities;

    internal class LightStrikeArray : LinearAOE, IModifier
    {
        #region Fields

        private readonly float additionalDelay;

        private bool fowCast;

        private bool modifierAdded;

        private Vector3 position;

        #endregion

        #region Constructors and Destructors

        public LightStrikeArray(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(SnowBall);

            additionalDelay = Ability.AbilitySpecialData.First(x => x.Name == "light_strike_array_delay_time").Value;
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

                if (Obstacle == null)
                {
                    StartCast = Game.RawGameTime;
                    EndCast = StartCast + additionalDelay;
                    fowCast = true;
                }
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
                EndCast = StartCast + CastPoint + additionalDelay;
            }
            else if (phase && Obstacle == null && (int)Owner.RotationDifference == 0)
            {
                StartPosition = Owner.NetworkPosition;
                EndPosition = Owner.InFront(GetCastRange() + GetRadius() / 2);
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
            fowCast = false;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return StartCast + (fowCast ? 0 : CastPoint) + additionalDelay - Game.RawGameTime;
        }

        #endregion
    }
}