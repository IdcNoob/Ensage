namespace Evader.EvadableAbilities.Base.Interfaces
{
    using Ensage;

    internal interface IUnit
    {
        #region Public Properties

        //todo remove name after tests
        string Name { get; }

        #endregion

        #region Public Methods and Operators

        void AddUnit(Unit unit);

        #endregion
    }
}