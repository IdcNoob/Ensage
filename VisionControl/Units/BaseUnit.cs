namespace VisionControl.Units
{
    using Ensage;
    using Ensage.Common.Extensions;

    using Interfaces;

    using SharpDX;

    internal abstract class BaseUnit : IUnit
    {
        protected BaseUnit(Vector3 position)
        {
            CreateTime = Game.RawGameTime;
            Position = position;
            TextureSize = new Vector2(40);
            PositionCorrection = new Vector2(25);
        }

        protected BaseUnit(Unit unit)
            : this(unit.Position)
        {
            Unit = unit;
            Handle = unit.Handle;
        }

        public string AbilityName { get; protected set; }

        public float CreateTime { get; }

        public float Duration { get; protected set; }

        public float EndTime { get; protected set; }

        public uint Handle { get; protected set; }

        public ParticleEffect ParticleEffect { get; protected set; }

        public Vector3 Position { get; protected set; }

        public Vector2 PositionCorrection { get; protected set; }

        public float Radius { get; protected set; }

        public virtual bool ShowTexture => !Unit.IsVisible;

        public abstract bool ShowTimer { get; }

        public DotaTexture Texture { get; protected set; }

        public Vector2 TextureSize { get; protected set; }

        protected Unit Unit { get; set; }

        public float Distance(Vector3 position)
        {
            return Position.Distance2D(position);
        }
    }
}