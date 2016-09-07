namespace VisionControl.Units.Mines
{
    using Ensage;

    using SharpDX;

    internal abstract class Mine : IUnit
    {
        #region Constructors and Destructors

        protected Mine(Unit unit, string abilityName)
        {
            Unit = unit;
            Position = unit.Position;
            Texture = Drawing.GetTexture("materials/ensage_ui/other/" + unit.Name);
            Handle = unit.Handle;
            TextureSize = new Vector2(40);

            if (Menu.RangeEnabled(abilityName))
            {
                ParticleEffect = new ParticleEffect("particles/ui_mouseactions/drag_selected_ring.vpcf", Position);
            }
        }

        protected Mine(Vector3 position)
        {
            TextureSize = new Vector2(40);
            Position = position;
            Texture = Drawing.GetTexture("materials/ensage_ui/other/npc_dota_techies_remote_mine");
        }

        #endregion

        #region Public Properties

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

        public Vector2 TextureSize { get; set; }

        #endregion

        #region Properties

        protected static MenuManager Menu => Variables.Menu;

        protected Unit Unit { get; set; }

        #endregion
    }
}