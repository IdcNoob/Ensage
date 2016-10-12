namespace Evader.EvadableAbilities.Heroes
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;

    using Utils;

    using static Core.Abilities;

    internal class LightStrikeArray : LinearAOE, IModifierThinker
    {
        #region Fields

        private bool fowCast;

        private bool modifierAdded;

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

            AdditionalDelay = Ability.AbilitySpecialData.First(x => x.Name == "light_strike_array_delay_time").Value;
        }

        #endregion

        #region Public Methods and Operators

        public void AddModifierThinker(Modifier mod, Unit unit)
        {
            var position = unit.Position;
            modifierAdded = true;

            AbilityDrawer.Dispose(AbilityDrawer.Type.Rectangle);
            AbilityDrawer.DrawCircle(position, GetRadius());

            if (Obstacle == null)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + AdditionalDelay;
                fowCast = true;
            }

            Obstacle = Pathfinder.AddObstacle(position, GetRadius(), Obstacle);
        }

        public override bool CanBeStopped()
        {
            return !modifierAdded && base.CanBeStopped();
        }

        public override void Draw()
        {
            if (Obstacle == null)
            {
                return;
            }

            AbilityDrawer.DrawTime(GetRemainingTime(), StartPosition);

            if (!modifierAdded)
            {
                AbilityDrawer.DrawDoubleArcRectangle(StartPosition, EndPosition, GetRadius());
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
            fowCast = false;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return StartCast + (fowCast ? 0 : CastPoint) + AdditionalDelay - Game.RawGameTime;
        }

        #endregion
    }
}