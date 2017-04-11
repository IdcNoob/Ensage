namespace VisionControl.Units
{
    using Ensage;

    internal interface IUpdatable
    {
        bool RequiresUpdate { get; }

        float Distance(Entity unit);

        void UpdateData(Unit unit);
    }
}