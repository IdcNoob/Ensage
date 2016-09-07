namespace VisionControl.Units.Wards
{
    using System.Linq;

    using Ensage;

    using SharpDX;

    internal class ObserverWard : Ward
    {
        #region Constants

        private const string AbilityName = "item_ward_observer";

        #endregion

        #region Fields

        private bool showTimer;

        #endregion

        #region Constructors and Destructors

        public ObserverWard(Unit unit)
            : base(unit, AbilityName)
        {
            Initialize();
        }

        public ObserverWard(Vector3 position)
            : base(position, AbilityName)
        {
            Initialize();
            Duration =
                Ability.GetAbilityDataByName(AbilityName).AbilitySpecialData.First(x => x.Name == "lifetime").Value;
            EndTime = Game.RawGameTime + Duration;
        }

        #endregion

        #region Public Properties

        public override bool ShowTimer => showTimer && base.ShowTimer;

        #endregion

        #region Public Methods and Operators

        public override void UpdateData(Unit unit)
        {
            base.UpdateData(unit);
            ParticleEffect?.Dispose();
            DrawRange();
        }

        #endregion

        #region Methods

        private void DrawRange()
        {
            if (ParticleEffect == null)
            {
                return;
            }

            ParticleEffect = new ParticleEffect("particles/ui_mouseactions/drag_selected_ring.vpcf", Position);
            ParticleEffect.SetControlPoint(1, new Vector3(255, 255, 0));
            ParticleEffect.SetControlPoint(2, new Vector3(Radius, 255, 0));
        }

        private void Initialize()
        {
            showTimer = Menu.TimerEnabled(AbilityName);
            PositionCorrection = new Vector2(25);
            Radius =
                Ability.GetAbilityDataByName(AbilityName).AbilitySpecialData.First(x => x.Name == "vision_range").Value;
            Texture = Drawing.GetTexture("materials/ensage_ui/other/item_ward_observer");
            DrawRange();
        }

        #endregion
    }
}