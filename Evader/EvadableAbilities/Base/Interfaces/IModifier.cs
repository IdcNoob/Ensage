namespace Evader.EvadableAbilities.Base.Interfaces
{
    using System.Linq;

    using Ensage;

    internal interface IModifier
    {
        #region Public Properties

        uint ModifierHandle { get; }

        #endregion

        #region Public Methods and Operators

        void AddModifer(Modifier modifier, Hero hero);

        bool CanBeCountered();

        float GetModiferRemainingTime();

        Hero GetModifierHero(ParallelQuery<Hero> allies);

        void RemoveModifier(Modifier modifier);

        #endregion
    }
}