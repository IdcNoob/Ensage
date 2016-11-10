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
        #region Public Properties

        public Dictionary<string, Func<Ability, UsableAbility>> BlinkAbilities { get; } = new Dictionary
            <string, Func<Ability, UsableAbility>>
            {
                // custom
                { "ember_spirit_activate_fire_remnant", ability => new FireRemnant(ability, AbilityType.Blink) },
                { "morphling_morph_replicate", ability => new Replicate(ability, AbilityType.Blink) },

                // default
                { "item_blink", ability => new BlinkDagger(ability, AbilityType.Blink) },
                { "antimage_blink", ability => new BlinkAbility(ability, AbilityType.Blink) },
                { "queenofpain_blink", ability => new BlinkAbility(ability, AbilityType.Blink) },
                { "faceless_void_time_walk", ability => new BlinkAbility(ability, AbilityType.Blink) },
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
                { "item_force_staff", ability => new ForceStaff(ability, AbilityType.Blink) }
            };

        public Dictionary<string, Func<Ability, UsableAbility>> CounterAbilities { get; } = new Dictionary
            <string, Func<Ability, UsableAbility>>
            {
                //// custom
                { "storm_spirit_ball_lightning", ability => new BallLightning(ability, AbilityType.Counter) },
                {
                    "ember_spirit_sleight_of_fist",
                    ability => new SleightOfFist(ability, AbilityType.Counter, AbilityCastTarget.Enemy)
                },
                {
                    "oracle_fortunes_end",
                    ability =>
                        new FortunesEnd(
                            ability,
                            AbilityType.Counter,
                            AbilityCastTarget.Ally | AbilityCastTarget.Enemy)
                },
                { "phoenix_supernova", ability => new Supernova(ability, AbilityType.Counter) },
                { "tusk_snowball", ability => new Snowball(ability, AbilityType.Counter) },
                {
                    "abyssal_underlord_dark_rift",
                    ability => new DarkRift(ability, AbilityType.Counter, AbilityCastTarget.Ally)
                },
                { "item_sphere", ability => new LinkensSphere(ability, AbilityType.Counter, AbilityCastTarget.Ally) },
                { "item_bloodstone", ability => new Bloodstone(ability, AbilityType.Counter) },
                { "item_armlet", ability => new ArmletOfMordiggian(ability, AbilityType.Counter) },

                //// default
                { "puck_phase_shift", ability => new NotTargetable(ability, AbilityType.Counter) },
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
                { "templar_assassin_meld", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "templar_assassin_refraction", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "weaver_shukuchi", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "windrunner_windrun", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "item_ghost", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "item_invis_sword", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "item_silver_edge", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "item_hood_of_defiance", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "item_blade_mail", ability => new BladeMail(ability, AbilityType.Counter) },
                // todo: check forms
                { "lone_druid_true_form", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "lone_druid_true_form_druid", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "item_black_king_bar", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "item_manta", ability => new NotTargetable(ability, AbilityType.Counter) },
                { "invoker_deafening_blast", ability => new Targetable(ability, AbilityType.Counter) },
                //todo: improve keeper_of_the_light_blinding_light
                { "keeper_of_the_light_blinding_light", ability => new Targetable(ability, AbilityType.Counter) },
                //todo: doppelwalk use infront
                { "phantom_lancer_doppelwalk", ability => new Targetable(ability, AbilityType.Counter) },
                { "pugna_nether_ward", ability => new Targetable(ability, AbilityType.Counter) },
                { "razor_static_link", ability => new Targetable(ability, AbilityType.Counter) },

                // cast on all
                {
                    "item_ethereal_blade",
                    ability =>
                        new Targetable(
                            ability,
                            AbilityType.Counter,
                            AbilityCastTarget.Ally | AbilityCastTarget.Enemy)
                },
                {
                    "pugna_decrepify",
                    ability =>
                        new Targetable(
                            ability,
                            AbilityType.Counter,
                            AbilityCastTarget.Ally | AbilityCastTarget.Enemy)
                },
                {
                    "obsidian_destroyer_astral_imprisonment",
                    ability =>
                        new Targetable(
                            ability,
                            AbilityType.Counter,
                            AbilityCastTarget.Ally | AbilityCastTarget.Enemy)
                },
                {
                    "shadow_demon_disruption",
                    ability =>
                        new Targetable(
                            ability,
                            AbilityType.Counter,
                            AbilityCastTarget.Ally | AbilityCastTarget.Enemy)
                },
                //todo: sub -0.1 cast point from nightmare
                {
                    "bane_nightmare",
                    ability =>
                        new Targetable(
                            ability,
                            AbilityType.Counter,
                            AbilityCastTarget.Ally | AbilityCastTarget.Enemy)
                },
                {
                    "oracle_fates_edict",
                    ability =>
                        new Targetable(
                            ability,
                            AbilityType.Counter,
                            AbilityCastTarget.Ally | AbilityCastTarget.Enemy)
                },

                // cast on enemy
                {
                    "brewmaster_drunken_haze",
                    ability => new Targetable(ability, AbilityType.Counter, AbilityCastTarget.Enemy)
                },
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

                // cast on ally
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
                    "treant_living_armor", ability => new Targetable(ability, AbilityType.Counter, AbilityCastTarget.Ally)
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
                { "item_glimmer_cape", ability => new Targetable(ability, AbilityType.Counter, AbilityCastTarget.Ally) },
                { "item_lotus_orb", ability => new Targetable(ability, AbilityType.Counter, AbilityCastTarget.Ally) }
            };

        public Dictionary<string, Func<Ability, UsableAbility>> DisableAbilities { get; } = new Dictionary
            <string, Func<Ability, UsableAbility>>
            {
                // custom
                { "earthshaker_echo_slam", ability => new EchoSlam(ability, AbilityType.Disable) },
                { "ember_spirit_searing_chains", ability => new SearingChains(ability, AbilityType.Disable) },

                // default
                { "item_orchid", ability => new Targetable(ability, AbilityType.Disable) },
                { "item_cyclone", ability => new Targetable(ability, AbilityType.Disable) },
                { "obsidian_destroyer_astral_imprisonment", ability => new Targetable(ability, AbilityType.Disable) },
                { "bane_fiends_grip", ability => new Targetable(ability, AbilityType.Disable) },
                { "bane_nightmare", ability => new Targetable(ability, AbilityType.Disable) },
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
                { "silencer_global_silence", ability => new Targetable(ability, AbilityType.Disable) },
                { "skywrath_mage_ancient_seal", ability => new Targetable(ability, AbilityType.Disable) },
                // todo: fix aghanim vortex
                { "storm_spirit_electric_vortex", ability => new Targetable(ability, AbilityType.Disable) },
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
                { "slardar_slithereen_crush", ability => new NotTargetable(ability, AbilityType.Disable) }
            };

        #endregion
    }
}