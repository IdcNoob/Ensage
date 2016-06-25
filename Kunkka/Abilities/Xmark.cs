namespace Kunkka.Abilities
{
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    using SharpDX;

    internal class Xmark : IAbility
    {
        #region Fields

        private Vector3 position;

        #endregion

        #region Constructors and Destructors

        public Xmark(Ability ability)
        {
            Ability = ability;
            CastPoint = ability.FindCastPoint();
        }

        #endregion

        #region Public Properties

        public Ability Ability { get; }

        public bool CanBeCasted => Utils.SleepCheck("Kunkka.Xmark") && !Ability.IsHidden && Ability.CanBeCasted();

        public bool Casted => Ability.AbilityState == AbilityState.OnCooldown;

        public double CastPoint { get; }

        public float CastRange => Ability.Level > 0 ? Ability.GetCastRange() + 100 : 0;

        public double GetSleepTime => CastPoint * 1000 + Game.Ping;

        public bool IsInPhase => Ability.IsInAbilityPhase;

        public uint ManaCost => Ability.ManaCost;

        public bool PhaseStarted { get; set; }

        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                PositionUpdated = false;
            }
        }

        public bool PositionUpdated { get; set; }

        public float TimeCasted { get; set; }

        #endregion

        #region Public Methods and Operators

        public void UseAbility(Hero target)
        {
            TimeCasted = Game.RawGameTime + Game.Ping / 1000;
            Ability.UseAbility(target);
            Utils.Sleep(GetSleepTime + 300, "Kunkka.Xmark");
        }

        #endregion
    }
}