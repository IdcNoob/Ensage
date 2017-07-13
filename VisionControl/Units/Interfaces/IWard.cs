namespace VisionControl.Units.Interfaces
{
    using SharpDX;

    internal interface IWard : IUpdatable

    {
        float CreateTime { get; }

        Vector3 Position { get; }

        bool ShowTexture { get; }
    }
}