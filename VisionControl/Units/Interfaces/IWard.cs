namespace VisionControl.Units.Interfaces
{
    using SharpDX;

    using Color = System.Drawing.Color;

    internal interface IWard : IUpdatable

    {
        Color Color { get; }

        float CreateTime { get; }

        Vector3 Position { get; }

        bool ShowTexture { get; }
    }
}