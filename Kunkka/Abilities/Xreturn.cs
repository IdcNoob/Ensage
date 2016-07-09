namespace Kunkka.Abilities
{
    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    internal class Xreturn : IAbility
    {
        #region Fields

        private readonly Sleeper sleeper = new Sleeper();

        #endregion

        #region Constructors and Destructors

        public Xreturn(Ability ability)
        {
            Ability = ability;
            CastPoint = (float)ability.FindCastPoint();
        }

        #endregion

        #region Public Properties

        public Ability Ability { get; }

        public bool CanBeCasted => !sleeper.Sleeping && !Ability.IsHidden && Ability.CanBeCasted();

        public bool Casted => Ability.IsHidden;

        public float CastPoint { get; }

        public float GetSleepTime => CastPoint * 1000 + Game.Ping;

        public uint ManaCost { get; } = 0;

        #endregion

        #region Public Methods and Operators

        public void UseAbility()
        {
            Ability.UseAbility();
            sleeper.Sleep(GetSleepTime + 300);
        }

        #endregion
    }
}