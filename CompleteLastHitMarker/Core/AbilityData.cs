namespace CompleteLastHitMarker.Core
{
    using System.Collections.Generic;

    using Ensage;

    internal static class AbilityData
    {
        public static List<AbilityId> ForceExclude { get; } = new List<AbilityId>
        {
            AbilityId.antimage_mana_void,
            AbilityId.chaos_knight_reality_rift,
            AbilityId.earthshaker_echo_slam,
            AbilityId.lich_chain_frost,
            AbilityId.lina_laguna_blade,
            AbilityId.lion_finger_of_death,
            AbilityId.monkey_king_wukongs_command,
            AbilityId.necrolyte_reapers_scythe,
            AbilityId.obsidian_destroyer_sanity_eclipse,
            AbilityId.greevil_phantom_strike,
            AbilityId.riki_tricks_of_the_trade,
            AbilityId.riki_blink_strike,
            AbilityId.slark_pounce,
            AbilityId.sniper_assassinate,
            AbilityId.spirit_breaker_nether_strike,
            AbilityId.tidehunter_ravage,
            AbilityId.tinker_heat_seeking_missile,
            AbilityId.warlock_rain_of_chaos,
            AbilityId.zuus_cloud,
            AbilityId.zuus_thundergods_wrath,
            AbilityId.nyx_assassin_mana_burn
        };

        public static List<AbilityId> ForceInclude { get; } = new List<AbilityId>
        {
            AbilityId.ancient_apparition_ice_blast,
            AbilityId.bloodseeker_blood_bath,
            AbilityId.keeper_of_the_light_illuminate,
            AbilityId.kunkka_torrent,
            AbilityId.kunkka_ghostship,
            AbilityId.lina_light_strike_array,
            AbilityId.slardar_slithereen_crush,
            AbilityId.techies_land_mines,
            AbilityId.techies_suicide,
            AbilityId.ursa_earthshock,
            AbilityId.weaver_shukuchi
        };
    }
}