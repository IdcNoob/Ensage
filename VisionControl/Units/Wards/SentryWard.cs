namespace VisionControl.Units.Wards
{
    using System.Linq;

    using Ensage;

    using SharpDX;

    internal class SentryWard : Ward
    {
        private const string AbilityName = "item_ward_sentry";

        private bool showTimer;

        public SentryWard(Unit unit)
            : base(unit, AbilityName)
        {
            Initialize();
        }

        public SentryWard(Vector3 position)
            : base(position, AbilityName)
        {
            Initialize();
            Duration = Ability.GetAbilityDataByName(AbilityName)
                .AbilitySpecialData.First(x => x.Name == "lifetime")
                .Value;
            EndTime = Game.RawGameTime + Duration;
        }

        public override bool ShowTimer => showTimer && base.ShowTimer;

        public override void UpdateData(Unit unit)
        {
            base.UpdateData(unit);
            ParticleEffect?.Dispose();
            DrawRange();
        }

        private void DrawRange()
        {
            if (ParticleEffect == null)
            {
                return;
            }

            ParticleEffect = new ParticleEffect("particles/ui_mouseactions/drag_selected_ring.vpcf", Position);
            ParticleEffect.SetControlPoint(1, new Vector3(30, 100, 255));
            ParticleEffect.SetControlPoint(2, new Vector3(Radius, 255, 0));
        }

        private void Initialize()
        {
            showTimer = Menu.TimerEnabled(AbilityName);
            PositionCorrection = new Vector2(25);
            Radius = Ability.GetAbilityDataByName(AbilityName)
                .AbilitySpecialData.First(x => x.Name == "true_sight_range")
                .Value;
            DrawRange();
            Texture = Drawing.GetTexture("materials/ensage_ui/other/item_ward_sentry");
        }
    }
}