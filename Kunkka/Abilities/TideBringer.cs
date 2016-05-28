namespace Kunkka.Abilities
{
    using Ensage;
    using Ensage.Common.Extensions;

    internal class TideBringer : IAbility
    {
        #region Constructors and Destructors

        public TideBringer(Ability ability)
        {
            Ability = ability;
        }

        #endregion

        #region Public Properties

        public Ability Ability { get; }

        public bool CanBeCasted => Ability.CanBeCasted();

        public bool Casted => Ability.AbilityState == AbilityState.OnCooldown;

        public double CastPoint { get; } = 0;

        public uint ManaCost { get; } = 0;

        #endregion
    }
}