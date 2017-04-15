namespace VisionControl.Units.Mines
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;

    using SharpDX;

    internal class RemoteMine : Mine, IUpdatable
    {
        private const string AbilityName = "techies_remote_mines";

        private bool showTimer;

        public RemoteMine(Unit unit)
            : base(unit, AbilityName)
        {
            Duration = unit.FindModifier("modifier_techies_remote_mine")?.RemainingTime ?? Ability
                           .GetAbilityDataByName(AbilityName)
                           .AbilitySpecialData.First(x => x.Name == "duration")
                           .Value;
            if (Menu.RangeEnabled(AbilityName))
            {
                ParticleEffect = new ParticleEffect("particles/ui_mouseactions/drag_selected_ring.vpcf", unit.Position);
            }
            Initialize();
        }

        public RemoteMine(Vector3 position)
            : base(position)
        {
            if (Menu.RangeEnabled(AbilityName))
            {
                ParticleEffect = new ParticleEffect("particles/ui_mouseactions/drag_selected_ring.vpcf", position);
            }
            RequiresUpdate = true;
            Duration = Ability.GetAbilityDataByName(AbilityName)
                .AbilitySpecialData.First(x => x.Name == "duration")
                .Value;
            Initialize();
        }

        public bool RequiresUpdate { get; private set; }

        public override bool ShowTexture => RequiresUpdate || !Unit.IsVisible;

        public override bool ShowTimer => showTimer && (RequiresUpdate || !Unit.IsVisible);

        public float Distance(Entity unit)
        {
            return Position.Distance2D(unit);
        }

        public void UpdateData(Unit unit)
        {
            Unit = unit;
            var duration = unit.FindModifier("modifier_techies_remote_mine")?.RemainingTime;
            if (duration != null)
            {
                Duration = duration.Value;
                EndTime = Game.RawGameTime + Duration;
            }
            RequiresUpdate = false;
            Handle = unit.Handle;
        }

        private void Initialize()
        {
            PositionCorrection = new Vector2(25);
            Radius = Ability.GetAbilityDataByName(AbilityName).AbilitySpecialData.First(x => x.Name == "radius").Value
                     + 25;

            EndTime = Game.RawGameTime + Duration;
            showTimer = Menu.TimerEnabled(AbilityName);

            if (ParticleEffect != null)
            {
                ParticleEffect.SetControlPoint(1, new Vector3(0, 255, 0));
                ParticleEffect.SetControlPoint(2, new Vector3(Radius, 255, 0));
            }
        }
    }
}