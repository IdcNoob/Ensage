namespace VisionControl.Units
{
    using Ensage;

    internal interface IUpdatable
    {
        #region Public Properties

        bool RequiresUpdate { get; }

        #endregion

        #region Public Methods and Operators

        float Distance(Entity unit);

        void UpdateData(Unit unit);

        #endregion
    }
}