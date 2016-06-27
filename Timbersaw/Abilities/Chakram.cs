namespace Timbersaw.Abilities
{
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    using SharpDX;

    internal class Chakram : TimberAbility
    {
        #region Fields

        private readonly Ability returnAbility;

        private Hero target;

        #endregion

        #region Constructors and Destructors

        public Chakram(Ability chakramAbility, Ability returnAbility)
            : base(chakramAbility)
        {
            Speed = chakramAbility.GetProjectileSpeed();
            Radius = chakramAbility.GetRadius();
            this.returnAbility = returnAbility;
            ReturnName = returnAbility.Name;
        }

        #endregion

        #region Public Properties

        public bool Casted => Ability.IsHidden;

        public bool IsInPhase => Ability.IsInAbilityPhase;

        public Vector3 Position { get; set; }

        public float Radius { get; }

        public float Speed { get; }

        #endregion

        #region Properties

        private string ReturnName { get; }

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted()
        {
            return base.CanBeCasted() && !Ability.IsHidden;
        }

        public void Return()
        {
            returnAbility.UseAbility();
            Utils.Sleep(500, "Timber." + ReturnName);
        }

        public bool ShouldReturn(Vector3 targetPosition)
        {
            if (target == null)
            {
                return true;
            }

            return Utils.SleepCheck("Timber." + Name) && Utils.SleepCheck("Timber." + ReturnName) && Casted
                   && targetPosition.Distance2D(Position) > Radius - 50 && target.GetTurnTime(Position) > 0;
        }

        public void Stop(Hero hero)
        {
            hero.Stop();
            Utils.Sleep(0, "Timber." + Name);
        }

        public void UseAbility(Vector3 position, Hero enemy)
        {
            Position = position;
            target = enemy;
            Ability.UseAbility(position);
            Utils.Sleep(GetSleepTime + 500, "Timber." + Name);
        }

        #endregion
    }
}