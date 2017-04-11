namespace Evader.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;

    internal class AbilityModifiers
    {
        private readonly Dictionary<string, string> fixedAbilityModifiers = new Dictionary<string, string>
        {
            { "modifier_phantom_assassin_stiflingdagger", "phantom_assassin_stifling_dagger" },
            { "modifier_monkey_king_quadruple_tap_bonuses", "monkey_king_jingu_mastery" },
            { "modifier_windrunner_shackle_shot", "windrunner_shackleshot" },
            { "modifier_juggernaut_omnislash", "juggernaut_omni_slash" },
            { "modifier_kunkka_x_marks_the_spot", "kunkka_return" },
            { "modifier_warlock_golem_permanent_immolation_debuff", "warlock_rain_of_chaos" },
            { "modifier_maledict", "witch_doctor_maledict" },
            { "modifier_templar_assassin_meld_armor", "templar_assassin_meld" },
            { "modifier_spirit_breaker_charge_of_darkness_vision", "spirit_breaker_charge_of_darkness" },
            { "modifier_storm_spirit_electric_vortex_pull", "storm_spirit_electric_vortex" },
            { "modifier_slark_pounce_leash", "slark_pounce" },
            { "modifier_skywrath_mystic_flare_aura_effect", "skywrath_mage_mystic_flare" },
            { "modifier_sandking_impale", "sandking_burrowstrike" },
            { "modifier_dragonknight_breathefire_reduction", "dragon_knight_breathe_fire" },
            { "modifier_oracle_false_promise_timer", "oracle_false_promise" },
            { "modifier_lone_druid_spirit_bear_entangle_effect", "lone_druid_spirit_bear_entangle" },
            { "modifier_oracle_fortunes_end_purge", "oracle_fortunes_end" },
            { "modifier_abyssal_underlord_pit_of_malice_ensare", "abyssal_underlord_pit_of_malice" },
            { "modifier_spawnlord_master_freeze_root", "spawnlord_master_freeze" }
        };

        private readonly Dictionary<string, string> fixedItemModifiers = new Dictionary<string, string>
        {
            { "item_blade_mail_axe_pw", "item_blade_mail" },
            { "item_travel_boots_tinker", "item_travel_boots" }
        };

        private readonly List<string> trimModifierEnd = new List<string>
        {
            "_burn",
            "_slow",
            "_blind",
            "_stun",
            "_debuff",
            "_disarm"
        };

        public string GetAbilityName(Modifier modifier)
        {
            string abilityName;
            var modifierName = modifier.Name;
            var modifierTextureName = modifier.TextureName;

            switch (modifierName)
            {
                case "modifier_bashed":
                case "modifier_stunned":
                case "modifier_silence":
                    abilityName = modifierTextureName.Split('/').LastOrDefault();
                    break;
                default:
                    if (modifierTextureName.StartsWith("item_")
                        && !fixedItemModifiers.TryGetValue(modifierTextureName, out abilityName))
                    {
                        abilityName = modifierTextureName;
                    }
                    else if (!fixedAbilityModifiers.TryGetValue(modifierName, out abilityName))
                    {
                        abilityName = modifierName.Substring("modifier_".Length);

                        var trimEnd = trimModifierEnd.FirstOrDefault(x => abilityName.EndsWith(x));
                        if (!string.IsNullOrEmpty(trimEnd))
                        {
                            abilityName = abilityName.Substring(
                                0,
                                abilityName.LastIndexOf(trimEnd, StringComparison.Ordinal));
                        }
                    }
                    break;
            }

            return abilityName;
        }
    }
}