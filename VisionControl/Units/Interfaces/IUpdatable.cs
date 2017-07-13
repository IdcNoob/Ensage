namespace VisionControl.Units.Interfaces
{
    using Ensage;

    using SharpDX;

    internal interface IUpdatable
    {
        string AbilityName { get; }

        bool RequiresUpdate { get; }

        int UpdatableDistance { get; }

        float Distance(Vector3 position);

        void UpdateData(Unit unit);
    }
}