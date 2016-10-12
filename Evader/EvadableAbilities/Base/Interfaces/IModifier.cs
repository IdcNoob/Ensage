namespace Evader.EvadableAbilities.Base.Interfaces
{
    using Ensage;

    internal interface IModifier
    {
        #region Public Methods and Operators

        void AddModifer(Modifier modifier, Hero hero);

        bool CanBeCountered();

        float GetModiferRemainingTime();

        Hero GetModifierHero();

        void RemoveModifier();

        #endregion
    }
}