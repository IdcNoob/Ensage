namespace VisionControl.Units
{
    using System.Linq;

    using Attributes;

    using Core;

    using Ensage;

    using SharpDX;

    [Unit("treant_eyes_in_the_forest", "npc_dota_treant_eyes")]
    internal class TreantEye : BaseUnit
    {
        public TreantEye(Unit unit, Settings settings)
            : base(unit)
        {
            AbilityName = "treant_eyes_in_the_forest";
            Radius = Ability.GetAbilityDataByName(AbilityName).AbilitySpecialData.First(x => x.Name == "vision_aoe").Value;
            Texture = Drawing.GetTexture("materials/ensage_ui/other/eyes_in_the_forest");
            ShowTimer = false;

            if (!settings.RangeEnabled(AbilityName))
            {
                return;
            }

            ParticleEffect = new ParticleEffect("particles/ui_mouseactions/drag_selected_ring.vpcf", Position);
            ParticleEffect.SetControlPoint(1, new Vector3(50, 205, 50));
            ParticleEffect.SetControlPoint(2, new Vector3(Radius, 255, 0));
        }

        public override bool ShowTimer { get; }
    }
}