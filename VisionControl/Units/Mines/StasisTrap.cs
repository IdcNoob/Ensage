namespace VisionControl.Units.Mines
{
    using System.Linq;

    using Attributes;

    using Core;

    using Ensage;

    using SharpDX;

    [Unit("techies_stasis_trap", "npc_dota_techies_stasis_trap")]
    internal class StasisTrap : BaseUnit
    {
        public StasisTrap(Unit unit, Settings settings)
            : base(unit)
        {
            AbilityName = "techies_stasis_trap";
            Radius = Ability.GetAbilityDataByName(AbilityName).AbilitySpecialData.First(x => x.Name == "activation_radius").Value;
            Texture = Drawing.GetTexture("materials/ensage_ui/other/npc_dota_techies_stasis_trap");
            ShowTimer = false;

            if (!settings.RangeEnabled(AbilityName))
            {
                return;
            }

            ParticleEffect = new ParticleEffect("particles/ui_mouseactions/drag_selected_ring.vpcf", Position);
            ParticleEffect.SetControlPoint(1, new Vector3(65, 105, 225));
            ParticleEffect.SetControlPoint(2, new Vector3(Radius, 255, 0));
        }

        public override bool ShowTimer { get; }
    }
}