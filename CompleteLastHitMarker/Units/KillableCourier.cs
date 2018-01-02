namespace CompleteLastHitMarker.Units
{
    using System.Collections.Generic;
    using System.Linq;

    using Base;

    using Core;

    using Ensage;
    using Ensage.Common;

    using Menus.Abilities;

    using SharpDX;

    using Utils;

    internal class KillableCourier : KillableUnit
    {
        private readonly List<AbilityId> courierDamageAbilities = new List<AbilityId>
        {
            AbilityId.clinkz_searing_arrows,
            AbilityId.templar_assassin_meld,
            AbilityId.tusk_walrus_punch,
            AbilityId.axe_culling_blade,
            AbilityId.queenofpain_sonic_wave,
            AbilityId.lina_laguna_blade,
            AbilityId.bristleback_quill_spray
        };


        public KillableCourier(Unit unit)
            : base(unit)
        {
            HpBarSize = new Vector2(HUDInfo.GetHPBarSizeX(Unit) - 12, HUDInfo.GetHpBarSizeY(Unit) / 2);
            DefaultTextureY = -50;
            UnitType = UnitType.Courier;
        }

        public override Vector2 HpBarPositionFix { get; } = new Vector2(15, 19);

        public override void CalculateAbilityDamageTaken(MyHero hero, AbilitiesMenu menu)
        {
            ActiveAbilities.Clear();

            foreach (var ability in hero.GetValidAbilities(this).Where(x => courierDamageAbilities.Contains(x.AbilityId)))
            {
                ActiveAbilities[ability] = ability.CalculateDamage(hero.Hero, Unit);
            }

            if (ActiveAbilities.Any())
            {
                AbilityDamageCalculated = true;
            }
        }
    }
}