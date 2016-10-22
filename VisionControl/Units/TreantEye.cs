namespace VisionControl.Units
{
    using System.Linq;

    using Ensage;

    using SharpDX;

    internal class TreantEye : IUnit
    {
        #region Constants

        private const string AbilityName = "treant_eyes_in_the_forest";

        #endregion

        #region Fields

        private readonly Unit unit;

        #endregion

        #region Constructors and Destructors

        public TreantEye(Unit unit)
        {
            this.unit = unit;
            PositionCorrection = new Vector2(25);
            Radius =
                Ability.GetAbilityDataByName(AbilityName).AbilitySpecialData.First(x => x.Name == "vision_aoe").Value;
            Position = unit.Position;
            Texture = Drawing.GetTexture("materials/ensage_ui/other/eyes_in_the_forest");
            Handle = unit.Handle;
            TextureSize = new Vector2(40);

            if (Menu.RangeEnabled(AbilityName))
            {
                ParticleEffect = new ParticleEffect("particles/ui_mouseactions/drag_selected_ring.vpcf", Position);
                ParticleEffect.SetControlPoint(1, new Vector3(50, 205, 50));
                ParticleEffect.SetControlPoint(2, new Vector3(Radius, 255, 0));
            }
        }

        #endregion

        #region Public Properties

        public float Duration { get; } = 0;

        public float EndTime { get; } = 0;

        public uint Handle { get; }

        public bool IsVisible => unit.IsVisible;

        public ParticleEffect ParticleEffect { get; }

        public Vector3 Position { get; }

        public Vector2 PositionCorrection { get; }

        public float Radius { get; }

        public bool ShowTexture => !unit.IsVisible;

        public bool ShowTimer { get; } = false;

        public DotaTexture Texture { get; }

        public Vector2 TextureSize { get; set; }

        #endregion

        #region Properties

        private static MenuManager Menu => Variables.Menu;

        #endregion
    }
}