namespace VisionControl.Units
{
    using System.Linq;

    using Attributes;

    using Core;

    using Ensage;

    using SharpDX;

    [Unit("templar_assassin_trap", "npc_dota_templar_assassin_psionic_trap")]
    internal class PsionicTrap : BaseUnit
    {
        public PsionicTrap(Unit unit, Settings settings)
            : base(unit)
        {
            AbilityName = "templar_assassin_trap";
            Radius = Ability.GetAbilityDataByName(AbilityName).AbilitySpecialData.First(x => x.Name == "trap_radius").Value + 25;
            Texture = Drawing.GetTexture("materials/ensage_ui/other/trap");
            ShowTimer = false;

            if (!settings.RangeEnabled(AbilityName))
            {
                return;
            }

            ParticleEffect = new ParticleEffect("particles/ui_mouseactions/drag_selected_ring.vpcf", Position);
            ParticleEffect.SetControlPoint(1, new Vector3(255, 105, 180));
            ParticleEffect.SetControlPoint(2, new Vector3(Radius, 255, 0));
        }

        public override bool ShowTimer { get; }
    }
}