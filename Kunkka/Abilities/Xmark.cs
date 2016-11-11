namespace Kunkka.Abilities
{
    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using SharpDX;

    internal class Xmark : IAbility
    {
        #region Fields

        private readonly Sleeper sleeper = new Sleeper();

        #endregion

        #region Constructors and Destructors

        public Xmark(Ability ability)
        {
            Ability = ability;
            CastPoint = (float)ability.FindCastPoint();
        }

        #endregion

        #region Public Properties

        public Ability Ability { get; }

        public bool CanBeCasted => !sleeper.Sleeping && !Ability.IsHidden && Ability.CanBeCasted();

        public bool Casted => Ability.AbilityState == AbilityState.OnCooldown;

        public float CastPoint { get; }

        public float CastRange => Ability.Level > 0 ? Ability.GetCastRange() + 200 : 0;

        public float GetSleepTime => CastPoint * 1000 + Game.Ping;

        public bool IsInPhase => Ability.IsInAbilityPhase;

        public uint ManaCost => Ability.ManaCost;

        public bool PhaseStarted { get; set; }

        public Vector3 Position { get; set; }

        public float TimeCasted { get; set; }

        #endregion

        #region Public Methods and Operators

        public void UseAbility(Hero target)
        {
            TimeCasted = Game.RawGameTime + Game.Ping / 1000;
            Ability.UseAbility(target);
            sleeper.Sleep(GetSleepTime + 300);
        }

        #endregion
    }
}