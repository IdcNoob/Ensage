namespace VisionControl.Units
{
    using System.Linq;

    using Attributes;

    using Core;

    using Ensage;

    using SharpDX;

    [Unit(
        "undying_tombstone",
        "npc_dota_unit_tombstone1",
        "npc_dota_unit_tombstone2",
        "npc_dota_unit_tombstone3",
        "npc_dota_unit_tombstone4")]
    internal class Tombstone : BaseUnit
    {
        public Tombstone(Unit unit, Settings settings)
            : base(unit)
        {
            var level = (uint)char.GetNumericValue(unit.Name.Last()) - 1;
            AbilityName = "undying_tombstone";
            Radius = Ability.GetAbilityDataByName(AbilityName)
                .AbilitySpecialData.First(x => x.Name == "radius")
                .GetValue(level);
            Duration = Ability.GetAbilityDataByName(AbilityName)
                .AbilitySpecialData.First(x => x.Name == "duration")
                .GetValue(level);
            Texture = Drawing.GetTexture("materials/ensage_ui/other/tombstone");
            EndTime = Game.RawGameTime + Duration;
            ShowTimer = settings.RangeEnabled(AbilityName);

            if (!settings.RangeEnabled(AbilityName))
            {
                return;
            }

            ParticleEffect = new ParticleEffect("particles/ui_mouseactions/drag_selected_ring.vpcf", Position);
            ParticleEffect.SetControlPoint(1, new Vector3(128, 128, 128));
            ParticleEffect.SetControlPoint(2, new Vector3(Radius, 255, 0));
        }

        public override bool ShowTimer { get; }
    }
}