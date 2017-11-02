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

    [Unit("item_ward_observer", "npc_dota_observer_wards")]
    internal class ObserverWard : BaseUnit, IWard
    {
        private bool timerEnabled;

        public ObserverWard(Unit unit, Settings settings)
            : base(unit)
        {
            SetData(settings);
        }

        public ObserverWard(Vector3 position, Settings settings)
            : base(position)
        {
            RequiresUpdate = true;
            SetData(settings);
        }

        public Color Color => Color.Yellow;

        public bool RequiresUpdate { get; private set; }

        public override bool ShowTexture => RequiresUpdate || base.ShowTexture;

        public override bool ShowTimer => timerEnabled && ShowTexture;

        public int UpdatableDistance { get; } = 400;

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
            AbilityName = "item_ward_observer";
            Duration = Unit?.FindModifier("modifier_item_buff_ward")?.RemainingTime ?? Ability.GetAbilityDataByName(AbilityName)
                           .AbilitySpecialData.First(x => x.Name == "lifetime")
                           .Value;
            EndTime = Game.RawGameTime + Duration;
            Radius = Ability.GetAbilityDataByName(AbilityName).AbilitySpecialData.First(x => x.Name == "vision_range").Value + 175;
            Texture = Drawing.GetTexture("materials/ensage_ui/other/item_ward_observer");
            TextureSize = new Vector2(50, 35);
            timerEnabled = settings.TimerEnabled(AbilityName);

            if (!settings.RangeEnabled(AbilityName))
            {
                return;
            }

            ParticleEffect = new ParticleEffect("particles/ui_mouseactions/drag_selected_ring.vpcf", Position);
            ParticleEffect.SetControlPoint(1, new Vector3(255, 255, 0));
            ParticleEffect.SetControlPoint(2, new Vector3(Radius, 255, 0));
        }
    }
}