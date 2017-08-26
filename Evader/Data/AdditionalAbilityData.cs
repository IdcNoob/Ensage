namespace Evader.Data
{
    using System.Collections.Generic;

    internal static class AdditionalAbilityData
    {
        public static Dictionary<string, string> Modifiers { get; } = new Dictionary<string, string>
        {
            { "modifier_invoker_sun_strike", "invoker_sun_strike" },
            { "modifier_kunkka_torrent_thinker", "kunkka_torrent" },
            { "modifier_leshrac_split_earth_thinker", "leshrac_split_earth" },
            { "modifier_lina_light_strike_array", "lina_light_strike_array" },
            { "modifier_disruptor_static_storm_thinker", "disruptor_static_storm" },
            { "modifier_faceless_void_chronosphere", "faceless_void_chronosphere" },
            { "modifier_enigma_black_hole_thinker", "enigma_black_hole" },
            { "modifier_bloodseeker_bloodbath_thinker", "bloodseeker_blood_bath" },
            { "modifier_slark_dark_pact", "slark_dark_pact" },
            { "modifier_storm_spirit_static_remnant_thinker", "storm_spirit_static_remnant" },
            { "modifier_shredder_chakram_thinker", "shredder_chakram" },
            { "modifier_monkey_king_spring_thinker", "monkey_king_primal_spring" },
            { "modifier_morphling_waveform", "morphling_waveform" },
        };

        public static Dictionary<string, string> Particles { get; } = new Dictionary<string, string>
        {
            { "meathook", "pudge_meat_hook" },
            { "pounce_trail", "slark_pounce" },
            { "powershot_channel", "windrunner_powershot" },
            { "venomous_gale", "venomancer_venomous_gale" },
            { "poison_nova", "venomancer_poison_nova" },
            { "whirling_axe_melee", "troll_warlord_whirling_axes_melee" },
            { "ice_blast_final", "ancient_apparition_ice_blast" },
            { "ice_blast_explode", "ancient_apparition_ice_blast" },
            { "hookshot", "rattletrap_hookshot" },
            { "rocket_flare", "rattletrap_rocket_flare" },
            { "glimpse_travel", "disruptor_glimpse" },
            { "_emp.vpcf", "invoker_emp" },
            { "chaos_meteor_fly", "invoker_chaos_meteor" },
            { "bouldersmash_caster", "earth_spirit_boulder_smash" },
            { "bouldersmash_target", "earth_spirit_boulder_smash" },
            { "rollingboulder", "earth_spirit_rolling_boulder" },
            { "geomagentic_grip_target", "earth_spirit_geomagnetic_grip" },
            { "sleight_of_fist_cast", "ember_spirit_sleight_of_fist" },
            { "calldown_first", "gyrocopter_call_down" },
            { "_illuminate.vpcf", "keeper_of_the_light_illuminate" },
            { "_illuminate_charge.vpcf", "keeper_of_the_light_illuminate" },
            { "fire_spirit_launch", "phoenix_launch_fire_spirit" },
            { "supernova_start", "phoenix_supernova" },
            { "spring_channel", "monkey_king_primal_spring" },
            { "arcana_death_channel", "monkey_king_primal_spring" },
            { "jump_treelaunch_ring", "monkey_king_primal_spring" },
            { "_cloud.vpcf", "zuus_cloud" },
            { "furion_sprout", "furion_sprout" },
            { "dreamcoil", "puck_dream_coil" }
        };

        public static Dictionary<string, uint> Vision { get; } = new Dictionary<string, uint>
        {
            { "shredder_timber_chain", 100 },
            { "rattletrap_hookshot", 100 },
            { "weaver_shukuchi", 100 },
            { "invoker_tornado", 200 },
            { "tusk_ice_shards", 200 },
            { "sven_storm_bolt", 225 },
            { "batrider_flamebreak", 300 },
            { "skywrath_mage_arcane_bolt", 325 },
            { "kunkka_ghostship", 400 },
            { "venomancer_venomous_gale", 400 },
            { "shadow_demon_shadow_poison", 400 },
            { "windrunner_powershot", 400 },
            { "skywrath_mage_concussive_shot", 400 },
            { "disruptor_glimpse", 400 },
            { "phantom_assassin_stifling_dagger", 450 },
            { "vengefulspirit_wave_of_terror", 500 },
            { "mirana_arrow", 500 },
            { "ancient_apparition_ice_blast", 550 },
            { "puck_illusory_orb", 450 },
            { "rattletrap_rocket_flare", 600 },
            { "lich_chain_frost", 800 },
            { "razor_plasma_field", 800 }, // unit has name
        };
    }
}