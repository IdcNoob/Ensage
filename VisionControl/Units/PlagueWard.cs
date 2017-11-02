namespace VisionControl.Units
{
    using System.Linq;

    using Attributes;

    using Core;

    using Ensage;

    using SharpDX;

    [Unit(
        "venomancer_plague_ward",
        "npc_dota_venomancer_plague_ward_1",
        "npc_dota_venomancer_plague_ward_2",
        "npc_dota_venomancer_plague_ward_3",
        "npc_dota_venomancer_plague_ward_4")]
    internal class PlagueWard : BaseUnit
    {
        public PlagueWard(Unit unit, Settings settings)
            : base(unit)
        {
            AbilityName = "venomancer_plague_ward";
            Radius = Game.FindKeyValues(unit.Name + "/AttackRange", KeyValueSource.Unit).IntValue + 50;
            Duration = Ability.GetAbilityDataByName(AbilityName).AbilitySpecialData.First(x => x.Name == "duration").Value;
            Texture = Drawing.GetTexture("materials/ensage_ui/other/plague_ward");
            EndTime = Game.RawGameTime + Duration;
            ShowTimer = settings.RangeEnabled(AbilityName);

            if (!settings.RangeEnabled(AbilityName))
            {
                return;
            }

            ParticleEffect = new ParticleEffect("particles/ui_mouseactions/drag_selected_ring.vpcf", Position);
            ParticleEffect.SetControlPoint(1, new Vector3(153, 153, 0));
            ParticleEffect.SetControlPoint(2, new Vector3(Radius, 255, 0));
        }

        public override bool ShowTimer { get; }
    }
}