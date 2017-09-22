namespace Evader.Data
{
    using System;
    using System.Collections.Generic;

    using Ensage;

    using UsableAbilities.Abilities;
    using UsableAbilities.Base;
    using UsableAbilities.Items;

    internal class AllyAbilities
    {
        public Dictionary<string, Func<Ability, UsableAbility>> BlinkAbilities { get; } =
            new Dictionary<string, Func<Ability, UsableAbility>>
            {
                { "ember_spirit_activate_fire_remnant", ability => new FireRemnant(ability, AbilityType.Blink) },
                { "morphling_morph_replicate", ability => new Replicate(ability, AbilityType.Blink) },
                { "item_blink", ability => new BlinkDagger(ability, AbilityType.Blink) },
                { "antimage_blink", ability => new BlinkAbility(ability, AbilityType.Blink) },
                { "queenofpain_blink", ability => new BlinkAbility(ability, AbilityType.Blink) },
                { "faceless_void_time_walk", ability => new TimeWalk(ability, AbilityType.Blink) },
                { "magnataur_skewer", ability => new BlinkAbility(ability, AbilityType.Blink) },
                { "morphling_waveform", ability => new BlinkAbility(ability, AbilityType.Blink) },
                { "phantom_lancer_doppelwalk", ability => new BlinkAbility(ability, AbilityType.Blink) },
                { "phantom_assassin_phantom_strike", ability => new TargetBlink(ability, AbilityType.Blink) },
                { "riki_blink_strike", ability => new TargetBlink(ability, AbilityType.Blink) },
                { "sandking_burrowstrike", ability => new BlinkAbility(ability, AbilityType.Blink) },
                { "earth_spirit_rolling_boulder", ability => new BlinkAbility(ability, AbilityType.Blink) },
                { "slark_pounce", ability => new Leap(ability, AbilityType.Blink) },
                { "mirana_leap", ability => new Leap(ability, AbilityType.Blink) },
                { "item_hurricane_pike", ability => new ForceStaff(ability, AbilityType.Blink) },
                { "item_force_staff", ability => new ForceStaff(ability, AbilityType.Blink) },
                { "monkey_king_tree_dance", ability => new TreeDance(ability, AbilityType.Blink) }
            };

        public Dictionary<string, Func<Ability, UsableAbility>> CounterAbilities { get; } =
            new Dictionary<string, Func<Ability, UsableAbility>>
            {
                { "storm_spirit_ball_lightning", ability => new BallLightning(ability, AbilityType.Counter) },
                { "ursa_enrage", ability => new Enrage(ability, AbilityType.Counter) },
                { "clinkz_searing_arrows", ability => new OrbAbility(ability, AbilityType.Counter) },
                { "obsidian_destroyer_arcane_orb", ability => new OrbAbility(ability, AbilityType.Counter) },
                { "enchantress_impetus", ability => new OrbAbility(ability, AbilityType.Counter) },
                { "huskar_burning_spear", ability => new OrbAbility(ability, AbilityType.Counter) },
                { "silencer_glaives_of_wisdom", ability => new OrbAbility(ability, AbilityType.Counter) },
                { "viper_poison_attack", ability => new OrbAbility(ability, AbilityType.Counter) },
                {
                    "ember_spirit_sleight_of_fist",
                    ability => new SleightOfFist(ability, AbilityType.Counter, AbilityCastTarget.Enemy)
                },
                {
                    "oracle_fortunes_end",
                    ability => new FortunesEnd(
                        ability,
                        AbilityType.Counter,
                        AbilityCastTarget.Ally | AbilityCastTarget.Enemy)
                },
                { "phoenix_supernova", ability => new Supernova(ability, AbilityType.Counter) },
                { "tusk_snowball", ability => new Snowball(ability, AbilityType.Counter, AbilityCastTarget.Ally) },
                {
                    "abyssal_underlord_dark_rift",
                    ability => new DarkRift(ability, AbilityType.Counter, AbilityCastTarget.Ally)
                },
                { "item_sphere", ability => new LinkensSphere(ability, AbilityType.Counter, AbilityCastTarget.Ally) },
                {
                    "item_solar_crest",
                    ability => new MedallionOfCourage(ability, AbilityType.Counter, AbilityCastTarget.Ally)
                },
                {
                    "item_medallion_of_courage",
                    ability => new MedallionOfCourage(ability, AbilityType.Counter, AbilityCastTarget.Ally)
                },
                { "item_bloodstone", ability => new Bloodstone(ability, AbilityType.Counter) },
                { "item_armlet", ability => new ArmletOfMordiggian(ability, AbilityType.Counter) },
                { "puck_phase_shift", ability => new PhaseShift(ability, AbilityType.Counter) },
                { "templar_assassin_meld", ability => new Meld(ability, AbilityType.Counter) },
                { "alchemist_chemical_rage", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "riki_tricks_of_the_trade", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "item_cyclone", ability => new Targetable(ability, AbilityType.Counter) },
                { "clinkz_wind_walk", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "ember_spirit_flame_guard", ability => new NotTargetable(ability, AbilityType.Counter) },
                // todo: invoke invoker abilities
                { "invoker_ghost_walk", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "juggernaut_blade_fury", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "life_stealer_rage", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "medusa_stone_gaze", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "nyx_assassin_spiked_carapace", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "nyx_assassin_vendetta", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "sandking_sand_storm", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "slark_dark_pact", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "slark_shadow_dance", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "templar_assassin_refraction", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "weaver_shukuchi", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "windrunner_windrun", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "item_ghost", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "item_invis_sword", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "item_silver_edge", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "item_hood_of_defiance", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "item_blade_mail", ability => new BladeMail(ability, AbilityType.Counter) },
                { "item_black_king_bar", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "item_manta", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "invoker_deafening_blast", ability => new Targetable(ability, AbilityType.Counter) },
                {
                    "keeper_of_the_light_blinding_light",
                    ability => new BlindingLight(ability, AbilityType.Counter, AbilityCastTarget.Enemy)
                },
                { "phantom_lancer_doppelwalk", ability => new Doppelganger(ability, AbilityType.Counter) },
                { "pugna_nether_ward", ability => new Targetable(ability, AbilityType.Counter) },
                { "razor_static_link", ability => new Targetable(ability, AbilityType.Counter) },
                {
                    "item_ethereal_blade",
                    ability => new Targetable(
                        ability,
                        AbilityType.Counter,
                        AbilityCastTarget.Ally | AbilityCastTarget.Enemy)
                },
                {
                    "pugna_decrepify",
                    ability => new Targetable(
                        ability,
                        AbilityType.Counter,
                        AbilityCastTarget.Ally | AbilityCastTarget.Enemy)
                },
                {
                    "obsidian_destroyer_astral_imprisonment",
                    ability => new Targetable(
                        ability,
                        AbilityType.Counter,
                        AbilityCastTarget.Ally | AbilityCastTarget.Enemy)
                },
                {
                    "shadow_demon_disruption",
                    ability => new Targetable(
                        ability,
                        AbilityType.Counter,
                        AbilityCastTarget.Ally | AbilityCastTarget.Enemy)
                },
                {
                    "bane_nightmare",
                    ability => new Targetable(
                        ability,
                        AbilityType.Counter,
                        AbilityCastTarget.Ally | AbilityCastTarget.Enemy)
                },
                {
                    "oracle_fates_edict",
                    ability => new Targetable(
                        ability,
                        AbilityType.Counter,
                        AbilityCastTarget.Ally | AbilityCastTarget.Enemy)
                },
                {
                    "item_hurricane_pike",
                    ability => new HurricanePike(ability, AbilityType.Counter, AbilityCastTarget.Enemy)
                },
                {
                    "item_diffusal_blade",
                    ability => new Targetable(ability, AbilityType.Counter, AbilityCastTarget.Enemy)
                },
                {
                    "item_diffusal_blade_2",
                    ability => new Targetable(ability, AbilityType.Counter, AbilityCastTarget.Enemy)
                },
                {
                    "brewmaster_drunken_haze",
                    ability => new Targetable(ability, AbilityType.Counter, AbilityCastTarget.Enemy)
                },
                { "necrolyte_sadist", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "tinker_laser", ability => new Targetable(ability, AbilityType.Counter, AbilityCastTarget.Enemy) },
                {
                    "item_heavens_halberd",
                    ability => new Targetable(ability, AbilityType.Counter, AbilityCastTarget.Enemy)
                },
                { "bane_enfeeble", ability => new Targetable(ability, AbilityType.Counter, AbilityCastTarget.Enemy) },
                {
                    "shadow_demon_demonic_purge",
                    ability => new Targetable(ability, AbilityType.Counter, AbilityCastTarget.Enemy)
                },
                {
                    "naga_siren_song_of_the_siren",
                    ability => new NotTargetable(ability, AbilityType.Counter, AbilityCastTarget.Ally)
                },
                {
                    "oracle_false_promise",
                    ability => new Targetable(ability, AbilityType.Counter, AbilityCastTarget.Ally)
                },
                { "lich_frost_armor", ability => new Targetable(ability, AbilityType.Counter, AbilityCastTarget.Ally) },
                { "sven_warcry", ability => new NotTargetable(ability, AbilityType.Counter, AbilityCastTarget.Ally) },
                {
                    "treant_living_armor",
                    ability => new Targetable(ability, AbilityType.Counter, AbilityCastTarget.Ally)
                },
                {
                    "tusk_frozen_sigil",
                    ability => new NotTargetable(ability, AbilityType.Counter, AbilityCastTarget.Ally)
                },
                { "item_buckler", ability => new NotTargetable(ability, AbilityType.Counter, AbilityCastTarget.Ally) },
                { "item_pipe", ability => new NotTargetable(ability, AbilityType.Counter, AbilityCastTarget.Ally) },
                {
                    "item_crimson_guard",
                    ability => new NotTargetable(ability, AbilityType.Counter, AbilityCastTarget.Ally)
                },
                {
                    "item_shivas_guard",
                    ability => new NotTargetable(ability, AbilityType.Counter, AbilityCastTarget.Ally)
                },
                {
                    "abaddon_aphotic_shield",
                    ability => new Targetable(ability, AbilityType.Counter, AbilityCastTarget.Ally)
                },
                {
                    "legion_commander_press_the_attack",
                    ability => new Targetable(ability, AbilityType.Counter, AbilityCastTarget.Ally)
                },
                { "omniknight_repel", ability => new Targetable(ability, AbilityType.Counter, AbilityCastTarget.Ally) },
                {
                    "omniknight_guardian_angel",
                    ability => new NotTargetable(ability, AbilityType.Counter, AbilityCastTarget.Ally)
                },
                {
                    "winter_wyvern_cold_embrace",
                    ability => new Targetable(ability, AbilityType.Counter, AbilityCastTarget.Ally)
                },
                {
                    "item_glimmer_cape", ability => new Targetable(ability, AbilityType.Counter, AbilityCastTarget.Ally)
                },
                { "item_lotus_orb", ability => new Targetable(ability, AbilityType.Counter, AbilityCastTarget.Ally) },
                {
                    "item_quelling_blade",
                    ability => new TargetTree(ability, AbilityType.Counter, AbilityCastTarget.Ally)
                },
                { "item_iron_talon", ability => new TargetTree(ability, AbilityType.Counter, AbilityCastTarget.Ally) },
                { "item_tango", ability => new TargetTree(ability, AbilityType.Counter, AbilityCastTarget.Ally) },
                {
                    "item_tango_single", ability => new TargetTree(ability, AbilityType.Counter, AbilityCastTarget.Ally)
                },
                { "item_bfury", ability => new TargetTree(ability, AbilityType.Counter, AbilityCastTarget.Ally) }
            };

        public Dictionary<string, Func<Ability, UsableAbility>> DisableAbilities { get; } =
            new Dictionary<string, Func<Ability, UsableAbility>>
            {
                { "earthshaker_echo_slam", ability => new EchoSlam(ability, AbilityType.Disable) },
                { "ember_spirit_searing_chains", ability => new SearingChains(ability, AbilityType.Disable) },
                { "item_orchid", ability => new Targetable(ability, AbilityType.Disable) },
                { "item_bloodthorn", ability => new Targetable(ability, AbilityType.Disable) },
                { "item_cyclone", ability => new Targetable(ability, AbilityType.Disable) },
                { "obsidian_destroyer_astral_imprisonment", ability => new Targetable(ability, AbilityType.Disable) },
                { "dragon_knight_dragon_tail", ability => new Targetable(ability, AbilityType.Disable) },
                { "bane_fiends_grip", ability => new Targetable(ability, AbilityType.Disable) },
                { "bane_nightmare", ability => new Targetable(ability, AbilityType.Disable) },
                { "keeper_of_the_light_mana_leak", ability => new Targetable(ability, AbilityType.Disable) },
                { "batrider_flaming_lasso", ability => new Targetable(ability, AbilityType.Disable) },
                { "beastmaster_primal_roar", ability => new Targetable(ability, AbilityType.Disable) },
                { "crystal_maiden_frostbite", ability => new Targetable(ability, AbilityType.Disable) },
                { "death_prophet_silence", ability => new Targetable(ability, AbilityType.Disable) },
                { "disruptor_static_storm", ability => new Targetable(ability, AbilityType.Disable) },
                { "doom_bringer_doom", ability => new Targetable(ability, AbilityType.Disable) },
                { "drow_ranger_wave_of_silence", ability => new Targetable(ability, AbilityType.Disable) },
                { "earthshaker_fissure", ability => new Targetable(ability, AbilityType.Disable) },
                { "enigma_malefice", ability => new Targetable(ability, AbilityType.Disable) },
                { "invoker_cold_snap", ability => new Targetable(ability, AbilityType.Disable) },
                { "invoker_tornado", ability => new Targetable(ability, AbilityType.Disable) },
                { "lion_impale", ability => new Targetable(ability, AbilityType.Disable) },
                { "lion_voodoo", ability => new Targetable(ability, AbilityType.Disable) },
                { "morphling_adaptive_strike", ability => new Targetable(ability, AbilityType.Disable) },
                { "naga_siren_ensnare", ability => new Targetable(ability, AbilityType.Disable) },
                { "night_stalker_crippling_fear", ability => new Targetable(ability, AbilityType.Disable) },
                { "nyx_assassin_impale", ability => new Targetable(ability, AbilityType.Disable) },
                { "ogre_magi_fireblast", ability => new Targetable(ability, AbilityType.Disable) },
                { "ogre_magi_unrefined_fireblast", ability => new Targetable(ability, AbilityType.Disable) },
                { "puck_dream_coil", ability => new Targetable(ability, AbilityType.Disable) },
                { "pudge_dismember", ability => new Targetable(ability, AbilityType.Disable) },
                { "riki_smoke_screen", ability => new Targetable(ability, AbilityType.Disable) },
                { "rubick_telekinesis", ability => new Targetable(ability, AbilityType.Disable) },
                { "sandking_burrowstrike", ability => new Targetable(ability, AbilityType.Disable) },
                { "shadow_demon_disruption", ability => new Targetable(ability, AbilityType.Disable) },
                { "shadow_shaman_voodoo", ability => new Targetable(ability, AbilityType.Disable) },
                { "shadow_shaman_shackles", ability => new Targetable(ability, AbilityType.Disable) },
                { "skywrath_mage_ancient_seal", ability => new Targetable(ability, AbilityType.Disable) },
                { "storm_spirit_electric_vortex", ability => new ElectricVortex(ability, AbilityType.Disable) },
                { "sven_storm_bolt", ability => new Targetable(ability, AbilityType.Disable) },
                { "tiny_avalanche", ability => new Targetable(ability, AbilityType.Disable) },
                { "tusk_walrus_punch", ability => new Targetable(ability, AbilityType.Disable) },
                { "tusk_walrus_kick", ability => new Targetable(ability, AbilityType.Disable) },
                { "vengefulspirit_magic_missile", ability => new Targetable(ability, AbilityType.Disable) },
                { "chaos_knight_chaos_bolt", ability => new Targetable(ability, AbilityType.Disable) },
                { "warlock_rain_of_chaos", ability => new Targetable(ability, AbilityType.Disable) },
                { "windrunner_shackleshot", ability => new Targetable(ability, AbilityType.Disable) },
                { "witch_doctor_paralyzing_cask", ability => new Targetable(ability, AbilityType.Disable) },
                { "skeleton_king_hellfire_blast", ability => new Targetable(ability, AbilityType.Disable) },
                { "item_sheepstick", ability => new Targetable(ability, AbilityType.Disable) },
                { "item_abyssal_blade", ability => new Targetable(ability, AbilityType.Disable) },
                // todo: check brewmaster_earth_hurl_boulder
                { "brewmaster_earth_hurl_boulder", ability => new Targetable(ability, AbilityType.Disable) },
                { "axe_berserkers_call", ability => new NotTargetable(ability, AbilityType.Disable) },
                { "centaur_hoof_stomp", ability => new NotTargetable(ability, AbilityType.Disable) },
                { "puck_waning_rift", ability => new NotTargetable(ability, AbilityType.Disable) },
                { "lone_druid_savage_roar", ability => new NotTargetable(ability, AbilityType.Disable) },
                { "lone_druid_savage_roar_bear", ability => new NotTargetable(ability, AbilityType.Disable) },
                { "silencer_global_silence", ability => new NotTargetable(ability, AbilityType.Disable) },
                { "slardar_slithereen_crush", ability => new NotTargetable(ability, AbilityType.Disable) }
            };
    }
}