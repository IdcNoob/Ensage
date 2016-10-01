namespace Evader.EvadableAbilities.Heroes
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;

    using SharpDX;

    using static Core.Abilities;

    internal class SunStrike : AOE, IModifier
    {
        #region Fields

        private readonly float additionalDelay;

        private Modifier modifier;

        private bool modifierAdded;

        #endregion

        #region Constructors and Destructors

        public SunStrike(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);

            additionalDelay = Ability.AbilitySpecialData.First(x => x.Name == "delay").Value;
        }

        #endregion

        #region Public Methods and Operators

        public void AddModifier(Modifier mod, Unit unit)
        {
            modifier = mod;
            Position = unit.Position;
            modifierAdded = true;
        }

        public override bool CanBeStopped()
        {
            return false;
        }

        public override void Check()
        {
            if (!modifierAdded)
            {
                return;
            }

            if (Obstacle != null)
            {
                if (GetRemainingTime() <= 0)
                {
                    End();
                }
                return;
            }

            if (modifier == null || !modifier.IsValid)
            {
                if (Obstacle != null)
                {
                    End();
                }
                return;
            }

            EndCast = Game.RawGameTime + (additionalDelay - modifier.ElapsedTime);
            Obstacle = Pathfinder.AddObstacle(Position, GetRadius(), Obstacle);
        }

        public override void Draw()
        {
            if (Obstacle == null)
            {
                return;
            }

            Vector2 textPosition;
            Drawing.WorldToScreen(Position, out textPosition);
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
        }

        public override void End()
        {
            if (Obstacle == null)
            {
                return;
            }

            base.End();
            modifierAdded = false;
            modifier = null;
        }

        #endregion
    }
}