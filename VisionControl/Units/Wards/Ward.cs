namespace VisionControl.Units.Wards
{
    using Ensage;
    using Ensage.Common.Extensions;

    using SharpDX;

    internal abstract class Ward : IUnit, IUpdatable
    {
        protected Ward(Unit unit, string abilityName)
        {
            Unit = unit;
            Position = unit.Position;
            Duration = unit.FindModifier("modifier_item_buff_ward").RemainingTime;
            EndTime = Game.RawGameTime + Duration;
            RequiresUpdate = false;
            Handle = unit.Handle;
            TextureSize = new Vector2(50, 35);
            if (Menu.RangeEnabled(abilityName))
            {
                ParticleEffect = new ParticleEffect("particles/ui_mouseactions/drag_selected_ring.vpcf", Position);
            }
        }

        protected Ward(Vector3 position, string abilityName)
        {
            Position = position;
            RequiresUpdate = true;
            TextureSize = new Vector2(50, 35);
            if (Menu.RangeEnabled(abilityName))
            {
                ParticleEffect = new ParticleEffect("particles/ui_mouseactions/drag_selected_ring.vpcf", Position);
            }
        }

        protected static MenuManager Menu => Variables.Menu;

        protected Unit Unit { get; set; }

        public float Duration { get; protected set; }

        public float EndTime { get; protected set; }

        public uint Handle { get; private set; }

        public ParticleEffect ParticleEffect { get; protected set; }

        public Vector3 Position { get; private set; }

        public Vector2 PositionCorrection { get; protected set; }

        public float Radius { get; protected set; }

        public bool ShowTexture => RequiresUpdate || !Unit.IsVisible;

        public virtual bool ShowTimer => RequiresUpdate || !Unit.IsVisible;

        public DotaTexture Texture { get; protected set; }

        public Vector2 TextureSize { get; set; }

        public bool RequiresUpdate { get; private set; }

        public float Distance(Entity unit)
        {
            return Position.Distance2D(unit);
        }

        public virtual void UpdateData(Unit unit)
        {
            Unit = unit;
            Position = unit.Position;
            Duration = unit.FindModifier("modifier_item_buff_ward").RemainingTime;
            EndTime = Game.RawGameTime + Duration;
            RequiresUpdate = false;
            Handle = unit.Handle;
        }

        public float Distance(Vector3 position)
        {
            return Unit?.Distance2D(position) ?? Position.Distance2D(position);
        }
    }
}