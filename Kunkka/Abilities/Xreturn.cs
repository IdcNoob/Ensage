namespace Kunkka.Abilities
{
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    internal class Xreturn : IAbility
    {
        #region Constructors and Destructors

        public Xreturn(Ability ability)
        {
            Ability = ability;
            CastPoint = ability.FindCastPoint();
        }

        #endregion

        #region Public Properties

        public Ability Ability { get; }

        public bool CanBeCasted => Utils.SleepCheck("Kunkka.Xreturn") && !Ability.IsHidden && Ability.CanBeCasted();

        public bool Casted => Ability.IsHidden;

        public double CastPoint { get; }

        public double GetSleepTime => CastPoint * 1000 + Game.Ping;

        public uint ManaCost { get; } = 0;

        #endregion

        #region Public Methods and Operators

        public void UseAbility()
        {
            Ability.UseAbility();
            Utils.Sleep(GetSleepTime + 300, "Kunkka.Xreturn");
        }

        #endregion
    }
}