namespace Evader.EvadableAbilities.Base.Interfaces
{
    using Ensage;

    internal interface IModifier
    {
        #region Public Methods and Operators

        void AddModifier(Modifier modifier, Unit unit);

        #endregion
    }
}