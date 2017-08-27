namespace VisionControl.Units.Interfaces
{
    using SharpDX;

    internal interface IWard : IUpdatable

    {
        System.Drawing.Color Color { get; }

        float CreateTime { get; }

        Vector3 Position { get; }

        bool ShowTexture { get; }
    }
}