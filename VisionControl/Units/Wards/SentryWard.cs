namespace VisionControl.Units.Wards
{
    using System.Linq;

    using Attributes;

    using Core;

    using Ensage;
    using Ensage.Common.Extensions;

    using Interfaces;

    using SharpDX;

    using Color = System.Drawing.Color;

    [Unit("item_ward_sentry", "npc_dota_sentry_wards")]
    internal class SentryWard : BaseUnit, IWard
    {
        private bool timerEnabled;

        public SentryWard(Unit unit, Settings settings)
            : base(unit)
        {
            SetData(settings);
        }

        public SentryWard(Vector3 position, Settings settings)
            : base(position)
        {
            RequiresUpdate = true;
            SetData(settings);
        }

        public Color Color => Color.Blue;

        public bool RequiresUpdate { get; private set; }

        public override bool ShowTexture => RequiresUpdate || base.ShowTexture;

        public override bool ShowTimer => timerEnabled && ShowTexture;

        public int UpdatableDistance { get; } = 400;

        public float Distance(Entity unit)
        {
            return Position.Distance2D(unit);
        }

        public void UpdateData(Unit unit)
        {
            Unit = unit;
            Handle = unit.Handle;
            Position = unit.Position;
            ParticleEffect?.SetControlPoint(0, Position);
            ParticleEffect?.FullRestart();
            RequiresUpdate = false;
        }

        private void SetData(Settings settings)
        {
            AbilityName = "item_ward_sentry";
            Duration = Unit?.FindModifier("modifier_item_buff_ward")?.RemainingTime ?? Ability.GetAbilityDataByName(AbilityName)
                           .AbilitySpecialData.First(x => x.Name == "lifetime")
                           .Value;
            EndTime = CreateTime + Duration;
            Radius = Ability.GetAbilityDataByName(AbilityName).AbilitySpecialData.First(x => x.Name == "true_sight_range").Value + 50;
            Texture = Drawing.GetTexture("materials/ensage_ui/other/item_ward_sentry");
            TextureSize = new Vector2(50, 35);
            timerEnabled = settings.TimerEnabled(AbilityName);

            if (!settings.RangeEnabled(AbilityName))
            {
                return;
            }

            ParticleEffect = new ParticleEffect("particles/ui_mouseactions/drag_selected_ring.vpcf", Position);
            ParticleEffect.SetControlPoint(1, new Vector3(30, 100, 255));
            ParticleEffect.SetControlPoint(2, new Vector3(Radius, 255, 0));
        }
    }
}