namespace Evader.EvadableAbilities.Base
{
    using Common;

    using Ensage;
    using Ensage.Common.Extensions;

    using SharpDX;

    internal abstract class AOE : EvadableAbility
    {
        #region Fields

        private readonly float radius;

        #endregion

        #region Constructors and Destructors

        protected AOE(Ability ability)
            : base(ability)
        {
            radius = ability.GetRadius() + 60;
            Debugger.WriteLine("// Radius: " + radius);
        }

        #endregion

        #region Public Properties

        public Vector3 StartPosition { get; protected set; }

        #endregion

        #region Public Methods and Operators

        public override void Check()
        {
            if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                StartPosition = AbilityOwner.NetworkPosition;
                EndCast = StartCast + CastPoint;
                Obstacle = Pathfinder.AddObstacle(StartPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast)
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

            AbilityDrawer.DrawTime(GetRemainingTime(), StartPosition);
            AbilityDrawer.DrawCircle(StartPosition, GetRadius());
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return StartCast + CastPoint + AdditionalDelay - Game.RawGameTime;
        }

        #endregion

        #region Methods

        protected virtual float GetRadius()
        {
            return radius;
        }

        #endregion
    }
}