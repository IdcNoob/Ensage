namespace VisionControl.Units.Mines
{
    using System.Linq;

    using Attributes;

    using Core;

    using Ensage;
    using Ensage.Common.Extensions;

    using Interfaces;

    using SharpDX;

    [Unit("techies_remote_mines", "npc_dota_techies_remote_mine")]
    internal class RemoteMine : BaseUnit, IUpdatable
    {
        private bool timerEnabled;

        public RemoteMine(Unit unit, Settings settings)
            : base(unit)
        {
            SetData(settings);
        }

        public RemoteMine(Vector3 position, Settings settings)
            : base(position)
        {
            RequiresUpdate = true;
            SetData(settings);
        }

        public bool RequiresUpdate { get; private set; }

        public override bool ShowTexture => RequiresUpdate || base.ShowTexture;

        public override bool ShowTimer => timerEnabled && ShowTexture;

        public int UpdatableDistance { get; } = 10;

        public float Distance(Entity unit)
        {
            return Position.Distance2D(unit);
        }

        public float Distance(RemoteMine unit)
        {
            return Position.Distance2D(unit.Position);
        }

        public void UpdateData(Unit unit)
        {
            Unit = unit;
            Handle = unit.Handle;
            RequiresUpdate = false;
        }

        private void SetData(Settings settings)
        {
            AbilityName = "techies_remote_mines";
            Duration = Unit?.FindModifier("modifier_techies_remote_mine")?.RemainingTime ?? Ability.GetAbilityDataByName(AbilityName)
                           .AbilitySpecialData.First(x => x.Name == "duration")
                           .Value;
            EndTime = Game.RawGameTime + Duration;
            Radius = Ability.GetAbilityDataByName(AbilityName).AbilitySpecialData.First(x => x.Name == "radius").Value + 25;
            Texture = Drawing.GetTexture("materials/ensage_ui/other/npc_dota_techies_remote_mine");
            timerEnabled = settings.TimerEnabled(AbilityName);

            if (!settings.RangeEnabled(AbilityName))
            {
                return;
            }

            ParticleEffect = new ParticleEffect("particles/ui_mouseactions/drag_selected_ring.vpcf", Position);
            ParticleEffect.SetControlPoint(1, new Vector3(0, 255, 0));
            ParticleEffect.SetControlPoint(2, new Vector3(Radius, 255, 0));
        }
    }
}