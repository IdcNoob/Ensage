namespace Evader.EvadableAbilities.Base.Interfaces
{
    using Ensage;

    internal interface IModifierThinker
    {
        #region Public Methods and Operators

        void AddModifierThinker(Modifier modifier, Unit unit);

        #endregion
    }
}