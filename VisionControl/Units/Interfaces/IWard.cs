namespace VisionControl.Units.Interfaces
{
    using System.Drawing;

    internal interface IWard : IUnit, IUpdatable

    {
        Color Color { get; }

        float CreateTime { get; }
    }
}