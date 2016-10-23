namespace Evader.EvadableAbilities.Base.Interfaces
{
    using Ensage;

    internal interface IModifierObstacle
    {
        #region Public Methods and Operators

        void AddModifierObstacle(Modifier modifier, Unit unit);

        #endregion
    }
}