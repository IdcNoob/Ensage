namespace Timbersaw.Abilities
{
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.AbilityInfo;
    using Ensage.Common.Extensions;

    using SharpDX;

    internal class Chakram : TimberAbility
    {
        #region Fields

        private readonly Ability returnAbility;

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

        public bool CanReturn => Utils.SleepCheck("Timber." + ReturnName) && Casted;

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

        public bool ShouldReturn(Hero hero, Target enemy)
        {
            return Utils.SleepCheck("Timber." + Name) && CanReturn && enemy.IsValid()
                   && (enemy.GetDistance(Position) > Radius - 50 && enemy.GetTurnTime(Position) > 0
                       || AbilityDamage.CalculateDamage(Ability, hero, enemy.Hero) / 2 >= enemy.Health);
        }

        public void Stop(Hero hero)
        {
            hero.Stop();
            Utils.Sleep(0, "Timber." + Name);
        }

        public void UseAbility(Vector3 position, Hero enemy, bool queue = false)
        {
            Position = position;
            Ability.UseAbility(position, queue);
            Utils.Sleep(GetSleepTime + 500, "Timber." + Name);
        }

        #endregion
    }
}