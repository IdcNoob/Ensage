namespace Timbersaw.Abilities
{
    using Ensage;
    using Ensage.Common.AbilityInfo;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using SharpDX;

    internal class Chakram : TimberAbility
    {
        #region Fields

        private readonly Ability returnAbility;

        private readonly Sleeper returnSleeper;

        #endregion

        #region Constructors and Destructors

        public Chakram(Ability chakramAbility, Ability returnAbility)
            : base(chakramAbility)
        {
            Speed = chakramAbility.GetProjectileSpeed();
            Radius = chakramAbility.GetRadius();
            this.returnAbility = returnAbility;
            returnSleeper = new Sleeper();
        }

        #endregion

        #region Public Properties

        public bool CanReturn => !returnSleeper.Sleeping && Casted;

        public bool Casted => Ability.IsHidden;

        public bool IsInPhase => Ability.IsInAbilityPhase;

        public Vector3 Position { get; set; }

        public float Radius { get; }

        public float Speed { get; }

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted()
        {
            return base.CanBeCasted() && !Casted;
        }

        public bool Damaging(Target enemy)
        {
            return Casted && enemy.GetDistance(Position) < Radius - 60;
        }

        public void Return()
        {
            returnAbility.UseAbility();
            returnSleeper.Sleep(500);
        }

        public bool ShouldReturn(Hero hero, Target enemy, bool doubleChakramDamage)
        {
            var damage = AbilityDamage.CalculateDamage(Ability, hero, enemy.Hero);

            if (!doubleChakramDamage)
            {
                damage /= 2;
            }

            return !Sleeper.Sleeping && CanReturn && enemy.IsValid()
                   && (!Damaging(enemy) && enemy.FindAngle(Position) > 0.5 || damage >= enemy.Health);
        }

        public void Stop(Hero hero)
        {
            hero.Stop();
            Sleeper.Sleep(Game.Ping);
        }

        public void UseAbility(Vector3 position, Hero enemy, Hero hero)
        {
            Position = position;
            Ability.UseAbility(position);
            Sleeper.Sleep(GetSleepTime + hero.Distance2D(position) / Speed * 1000);
        }

        #endregion
    }
}