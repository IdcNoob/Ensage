namespace VisionControl.Units
{
    using System.Linq;

    using Attributes;

    using Core;

    using Ensage;

    using SharpDX;

    [Unit(
        "pugna_nether_ward",
        "npc_dota_pugna_nether_ward_1",
        "npc_dota_pugna_nether_ward_2",
        "npc_dota_pugna_nether_ward_3",
        "npc_dota_pugna_nether_ward_4")]
    internal class NetherWard : BaseUnit
    {
        public NetherWard(Unit unit, Settings settings)
            : base(unit)
        {
            AbilityName = "pugna_nether_ward";
            Radius = Ability.GetAbilityDataByName(AbilityName).AbilitySpecialData.First(x => x.Name == "radius").Value;
            Duration = Ability.GetAbilityDataByName(AbilityName).AbilitySpecialData.First(x => x.Name == "ward_duration_tooltip").Value;
            Texture = Drawing.GetTexture("materials/ensage_ui/other/nether_ward");
            EndTime = Game.RawGameTime + Duration;
            ShowTimer = settings.RangeEnabled(AbilityName);

            if (!settings.RangeEnabled(AbilityName))
            {
                return;
            }

            ParticleEffect = new ParticleEffect("particles/ui_mouseactions/drag_selected_ring.vpcf", Position);
            ParticleEffect.SetControlPoint(1, new Vector3(124, 252, 0));
            ParticleEffect.SetControlPoint(2, new Vector3(Radius, 255, 0));
        }

        public override bool ShowTimer { get; }
    }
}