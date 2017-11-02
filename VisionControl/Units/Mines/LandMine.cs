namespace VisionControl.Units.Mines
{
    using System.Linq;

    using Attributes;

    using Core;

    using Ensage;

    using SharpDX;

    [Unit("techies_land_mines", "npc_dota_techies_land_mine")]
    internal class LandMine : BaseUnit
    {
        public LandMine(Unit unit, Settings settings)
            : base(unit)
        {
            AbilityName = "techies_land_mines";
            Radius = Ability.GetAbilityDataByName(AbilityName).AbilitySpecialData.First(x => x.Name == "radius").Value + 25;
            Texture = Drawing.GetTexture("materials/ensage_ui/other/npc_dota_techies_land_mine");
            ShowTimer = false;

            if (!settings.RangeEnabled(AbilityName))
            {
                return;
            }

            ParticleEffect = new ParticleEffect("particles/ui_mouseactions/drag_selected_ring.vpcf", Position);
            ParticleEffect.SetControlPoint(1, new Vector3(255, 0, 0));
            ParticleEffect.SetControlPoint(2, new Vector3(Radius, 255, 0));
        }

        public override bool ShowTimer { get; }
    }
}