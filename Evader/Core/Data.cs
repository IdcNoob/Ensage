namespace Evader.Core
{
    using System;
    using System.Collections.Generic;

    using Ensage;

    using EvadableAbilities.Base;
    using EvadableAbilities.Heroes;
    using EvadableAbilities.Items;
    using EvadableAbilities.Units;

    using UsableAbilities.Abilities;
    using UsableAbilities.Base;
    using UsableAbilities.Items;

    using FortunesEnd = EvadableAbilities.Heroes.FortunesEnd;
    using Void = EvadableAbilities.Heroes.Void;

    internal static class Data
    {
        #region Public Properties

        public static Dictionary<string, Func<Ability, EvadableAbility>> Abilities { get; } = new Dictionary
            <string, Func<Ability, EvadableAbility>>
            {
                //// custom
                { "pudge_meat_hook", ability => new MeatHook(ability) },
                { "invoker_sun_strike", ability => new SunStrike(ability) },
                { "leshrac_split_earth", ability => new SplitEarth(ability) },
                { "kunkka_torrent", ability => new Torrent(ability) },
                { "crystal_maiden_freezing_field", ability => new FreezingField(ability) },
                { "elder_titan_echo_stomp", ability => new EchoStomp(ability) },
                { "luna_eclipse", ability => new Eclipse(ability) },
                { "enigma_black_hole", ability => new BlackHole(ability) },
                { "venomancer_venomous_gale", ability => new VenomousGale(ability) },
                { "windrunner_powershot", ability => new Powershot(ability) },
                { "nevermore_shadowraze1", ability => new Shadowraze(ability) },
                { "nevermore_shadowraze2", ability => new Shadowraze(ability) },
                { "nevermore_shadowraze3", ability => new Shadowraze(ability) },
                { "jakiro_ice_path", ability => new IcePath(ability) },
                { "lina_light_strike_array", ability => new LightStrikeArray(ability) },
                { "disruptor_static_storm", ability => new StaticStorm(ability) },
                { "faceless_void_chronosphere", ability => new Chronosphere(ability) },

                // custom + modifier
                { "troll_warlord_whirling_axes_melee", ability => new WhirlingAxesMelee(ability) },
                { "slark_pounce", ability => new Pounce(ability) },
                { "venomancer_poison_nova", ability => new PoisonNova(ability) },


                //// default
                { "omniknight_purification", ability => new Purification(ability) },
                { "lion_finger_of_death", ability => new FingerOfDeath(ability) },
                { "lina_laguna_blade", ability => new LagunaBlade(ability) },
                { "shredder_chakram", ability => new Chakram(ability) },
                { "shredder_chakram_2", ability => new Chakram(ability) },
                { "beastmaster_wild_axes", ability => new WildAxes(ability) },
                { "death_prophet_carrion_swarm", ability => new CryptSwarm(ability) },
                { "axe_berserkers_call", ability => new BerserkersCall(ability) },
                { "queenofpain_sonic_wave", ability => new SonicWave(ability) },
                { "tidehunter_ravage", ability => new Ravage(ability) },
                { "mirana_arrow", ability => new SacredArrow(ability) },
                { "earthshaker_fissure", ability => new Fissure(ability) },
                { "beastmaster_primal_roar", ability => new PrimalRoar(ability) },
                { "sven_storm_bolt", ability => new StormHammer(ability) },
                { "phantom_lancer_spirit_lance", ability => new SpiritLance(ability) },
                { "alchemist_unstable_concoction_throw", ability => new UnstableConcoction(ability) },
                { "crystal_maiden_crystal_nova", ability => new CrystalNova(ability) },
                { "tinker_heat_seeking_missile", ability => new HeatSeekingMissile(ability) },
                { "brewmaster_thunder_clap", ability => new ThunderClap(ability) },
                { "centaur_hoof_stomp", ability => new HoofStomp(ability) },
                { "earthshaker_enchant_totem", ability => new EnchantTotem(ability) },
                { "magnataur_reverse_polarity", ability => new ReversePolarity(ability) },
                { "mirana_starfall", ability => new Starstorm(ability) },
                { "naga_siren_song_of_the_siren", ability => new SongOfTheSiren(ability) },
                { "slardar_slithereen_crush", ability => new SlithereenCrush(ability) },
                { "tidehunter_anchor_smash", ability => new AnchorSmash(ability) },
                { "axe_culling_blade", ability => new CullingBlade(ability) },
                { "troll_warlord_whirling_axes_ranged", ability => new WhirlingAxesRanged(ability) },
                { "ursa_earthshock", ability => new Earthshock(ability) },
                { "antimage_mana_void", ability => new ManaVoid(ability) },
                { "dark_seer_vacuum", ability => new Vacuum(ability) },
                { "dazzle_shadow_wave", ability => new ShadowWave(ability) },
                { "death_prophet_silence", ability => new Silence(ability) },
                { "drow_ranger_wave_of_silence", ability => new WaveOfSilence(ability) },
                { "legion_commander_overwhelming_odds", ability => new OverwhelmingOdds(ability) },
                { "obsidian_destroyer_sanity_eclipse", ability => new SanitysEclipse(ability) },
                { "shadow_shaman_ether_shock", ability => new EtherShock(ability) },
                { "silencer_curse_of_the_silent", ability => new ArcaneCurse(ability) },
                { "shredder_timber_chain", ability => new TimberChain(ability) },
                { "undying_decay", ability => new Decay(ability) },
                { "warlock_fatal_bonds", ability => new FatalBonds(ability) },
                { "warlock_rain_of_chaos", ability => new ChaoticOffering(ability) },
                { "jakiro_dual_breath", ability => new DualBreath(ability) },
                { "tusk_ice_shards", ability => new IceShards(ability) },
                { "lion_impale", ability => new EarthSpike(ability) },
                { "magnataur_shockwave", ability => new Shockwave(ability) },
                { "magnataur_skewer", ability => new Skewer(ability) },
                { "morphling_waveform", ability => new Waveform(ability) },
                { "nyx_assassin_impale", ability => new Impale(ability) },
                { "puck_illusory_orb", ability => new IllusoryOrb(ability) },
                { "abaddon_death_coil", ability => new MistCoil(ability) },
                { "bane_brain_sap", ability => new BrainSap(ability) },
                { "bane_nightmare", ability => new Nightmare(ability) },
                { "bane_fiends_grip", ability => new FiendsGrip(ability) },
                { "bloodseeker_rupture", ability => new Rupture(ability) },
                { "lina_dragon_slave", ability => new DragonSlave(ability) },
                { "bounty_hunter_shuriken_toss", ability => new ShurikenToss(ability) },
                { "centaur_double_edge", ability => new DoubleEdge(ability) },
                { "chaos_knight_reality_rift", ability => new RealityRift(ability) },
                { "rattletrap_hookshot", ability => new Hookshot(ability) },
                { "legion_commander_duel", ability => new Duel(ability) },
                { "lich_frost_nova", ability => new FrostBlast(ability) },
                { "luna_lucent_beam", ability => new LucentBeam(ability) },
                { "night_stalker_crippling_fear", ability => new CripplingFear(ability) },
                { "nyx_assassin_mana_burn", ability => new ManaBurn(ability) },
                { "ogre_magi_fireblast", ability => new Fireblast(ability) },
                { "ogre_magi_unrefined_fireblast", ability => new Fireblast(ability) },
                { "obsidian_destroyer_astral_imprisonment", ability => new AstralImprisonment(ability) },
                { "pudge_dismember", ability => new Dismember(ability) },
                { "shadow_demon_disruption", ability => new Disruption(ability) },
                { "shadow_shaman_shackles", ability => new Shackles(ability) },
                { "spirit_breaker_nether_strike", ability => new NetherStrike(ability) },
                { "storm_spirit_electric_vortex", ability => new Vortex(ability) },
                { "terrorblade_sunder", ability => new Sunder(ability) },
                { "tusk_walrus_punch", ability => new WalrusPunch(ability) },
                { "tusk_walrus_kick", ability => new WalrusKick(ability) },
                { "undying_soul_rip", ability => new SoulRip(ability) },
                { "vengefulspirit_nether_swap", ability => new NetherSwap(ability) },
                { "zuus_lightning_bolt", ability => new LightningBolt(ability) },
                { "broodmother_spawn_spiderlings", ability => new SpawnSpiderlings(ability) },
                { "chaos_knight_chaos_bolt", ability => new ChaosBolt(ability) },
                { "dazzle_poison_touch", ability => new PoisonTouch(ability) },
                { "huskar_life_break", ability => new LifeBreak(ability) },
                { "medusa_mystic_snake", ability => new MysticSnake(ability) },
                { "item_ethereal_blade", ability => new EtherealBlade(ability) },
                { "morphling_adaptive_strike", ability => new AdaptiveStrike(ability) },
                { "necrolyte_death_pulse", ability => new DeathPulse(ability) },
                { "ogre_magi_ignite", ability => new Ignite(ability) },
                { "oracle_fortunes_end", ability => new FortunesEnd(ability) },
                { "phantom_assassin_stifling_dagger", ability => new StiflingDagger(ability) },
                { "queenofpain_shadow_strike", ability => new ShadowStrike(ability) },
                { "queenofpain_scream_of_pain", ability => new ScreamOfPain(ability) },
                { "skywrath_mage_arcane_bolt", ability => new ArcaneBolt(ability) },
                { "skywrath_mage_concussive_shot", ability => new ConcussiveShot(ability) },
                { "sniper_assassinate", ability => new Assassinate(ability) },
                { "spectre_spectral_dagger", ability => new SpectralDagger(ability) },
                { "vengefulspirit_magic_missile", ability => new MagicMissile(ability) },
                { "viper_viper_strike", ability => new ViperStrike(ability) },
                { "visage_soul_assumption", ability => new SoulAssumption(ability) },
                { "windrunner_shackleshot", ability => new Shackleshot(ability) },
                { "witch_doctor_paralyzing_cask", ability => new ParalyzingCask(ability) },
                { "skeleton_king_hellfire_blast", ability => new WraithfireBlast(ability) },
                { "shadow_demon_shadow_poison", ability => new ShadowPoison(ability) },
                { "elder_titan_earth_splitter", ability => new EarthSplitter(ability) },

                // default + modifier
                { "tinker_laser", ability => new Laser(ability) },
                { "keeper_of_the_light_blinding_light", ability => new BlindingLight(ability) },
                { "witch_doctor_maledict", ability => new Maledict(ability) },
                { "vengefulspirit_wave_of_terror", ability => new WaveOfTerror(ability) },
                { "arc_warden_flux", ability => new Flux(ability) },
                { "batrider_flaming_lasso", ability => new FlamingLasso(ability) },
                { "crystal_maiden_frostbite", ability => new Frostbite(ability) },
                { "disruptor_thunder_strike", ability => new ThunderStrike(ability) },
                { "doom_bringer_doom", ability => new Doom(ability) },
                { "enigma_malefice", ability => new Malefice(ability) },
                { "naga_siren_ensnare", ability => new Ensnare(ability) },
                { "necrolyte_reapers_scythe", ability => new ReapersScythe(ability) },
                { "night_stalker_void", ability => new Void(ability) },
                { "treant_leech_seed", ability => new LeechSeed(ability) },
                { "visage_grave_chill", ability => new GraveChill(ability) },
                { "juggernaut_omni_slash", ability => new Omnislash(ability) },

                
                //// modifier only
                // { "broodmother_insatiable_hunger", ability => new InsatiableHunger(ability) },


                //// units
                { "centaur_khan_war_stomp", ability => new WarStomp(ability) },
                //     { "visage_summon_familiars_stone_form",ability => new StoneForm( ability) },
            };

        public static Dictionary<string, string> AbilityModifiers { get; } = new Dictionary<string, string>
            {
                { "modifier_invoker_sun_strike", "invoker_sun_strike" },
                { "modifier_kunkka_torrent_thinker", "kunkka_torrent" },
                { "modifier_leshrac_split_earth_thinker", "leshrac_split_earth" },
                { "modifier_lina_light_strike_array", "lina_light_strike_array" },
                { "modifier_disruptor_static_storm_thinker", "disruptor_static_storm" },
                { "modifier_faceless_void_chronosphere", "faceless_void_chronosphere" },
                { "modifier_enigma_black_hole_thinker", "enigma_black_hole" },
            };

        public static Dictionary<string, string> AbilityParticles { get; } = new Dictionary<string, string>
            {
                { "meathook", "pudge_meat_hook" },
                { "pounce_trail", "slark_pounce" },
                { "powershot_channel", "windrunner_powershot" },
                { "venomous_gale", "venomancer_venomous_gale" },
                { "poison_nova", "venomancer_poison_nova" },
                { "whirling_axe_melee", "troll_warlord_whirling_axes_melee" },
            };

        public static Dictionary<string, Func<Ability, UsableAbility>> EvadeBlinkAbilities { get; } = new Dictionary
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
                { "item_force_staff", ability => new ForceStaff(ability, AbilityType.Blink) },
            };

        public static Dictionary<string, Func<Ability, UsableAbility>> EvadeCounterAbilities { get; } = new Dictionary
            <string, Func<Ability, UsableAbility>>
            {
                //// custom
                { "storm_spirit_ball_lightning", ability => new BallLightning(ability, AbilityType.Counter) },
                {
                    "ember_spirit_sleight_of_fist",
                    ability => new SleightOfFist(ability, AbilityType.Counter, AbilityFlags.TargetEnemy)
                },
                {
                    "oracle_fortunes_end",
                    ability =>
                    new UsableAbilities.Abilities.FortunesEnd(
                        ability,
                        AbilityType.Counter,
                        AbilityFlags.CanBeCastedOnAlly | AbilityFlags.BasicDispel)
                },
                { "phoenix_supernova", ability => new Supernova(ability, AbilityType.Counter) },
                { "tusk_snowball", ability => new Snowball(ability, AbilityType.Counter) },
                {
                    //todo: check dark rift
                    "abyssal_underlord_dark_rift",
                    ability => new DarkRift(ability, AbilityType.Counter, AbilityFlags.CanBeCastedOnAlly)
                },
                {
                    "item_sphere",
                    ability => new LinkensSphere(ability, AbilityType.Counter, AbilityFlags.CanBeCastedOnAlly)
                },

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
                { "item_blade_mail", ability => new NotTargetable(ability, AbilityType.Counter) },
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

                // cast on enemy
                {
                    "brewmaster_drunken_haze",
                    ability => new Targetable(ability, AbilityType.Counter, AbilityFlags.TargetEnemy)
                },
                {
                    "item_ethereal_blade",
                    ability => new Targetable(ability, AbilityType.Counter, AbilityFlags.TargetEnemy)
                },
                { "tinker_laser", ability => new Targetable(ability, AbilityType.Counter, AbilityFlags.TargetEnemy) },
                {
                    "item_heavens_halberd",
                    ability => new Targetable(ability, AbilityType.Counter, AbilityFlags.TargetEnemy)
                },
                { "bane_enfeeble", ability => new Targetable(ability, AbilityType.Counter, AbilityFlags.TargetEnemy) },
                {
                    "shadow_demon_demonic_purge",
                    ability => new Targetable(ability, AbilityType.Counter, AbilityFlags.TargetEnemy)
                },

                // cast on ally
                {
                    "lich_frost_armor",
                    ability => new Targetable(ability, AbilityType.Counter, AbilityFlags.CanBeCastedOnAlly)
                },
                //todo: sub -0.1 cast point from nightmare
                {
                    "bane_nightmare",
                    ability => new Targetable(ability, AbilityType.Counter, AbilityFlags.CanBeCastedOnAlly)
                },
                {
                    "oracle_fates_edict",
                    ability => new Targetable(ability, AbilityType.Counter, AbilityFlags.CanBeCastedOnAlly)
                },
                {
                    "pugna_decrepify",
                    ability => new Targetable(ability, AbilityType.Counter, AbilityFlags.CanBeCastedOnAlly)
                },
                {
                    "sven_warcry",
                    ability => new NotTargetable(ability, AbilityType.Counter, AbilityFlags.CanBeCastedOnAlly)
                },
                {
                    "treant_living_armor",
                    ability => new Targetable(ability, AbilityType.Counter, AbilityFlags.CanBeCastedOnAlly)
                },
                {
                    "tusk_frozen_sigil",
                    ability => new NotTargetable(ability, AbilityType.Counter, AbilityFlags.CanBeCastedOnAlly)
                },
                {
                    "item_buckler",
                    ability => new NotTargetable(ability, AbilityType.Counter, AbilityFlags.CanBeCastedOnAlly)
                },
                {
                    "item_pipe",
                    ability => new NotTargetable(ability, AbilityType.Counter, AbilityFlags.CanBeCastedOnAlly)
                },
                {
                    "item_crimson_guard",
                    ability => new NotTargetable(ability, AbilityType.Counter, AbilityFlags.CanBeCastedOnAlly)
                },
                {
                    "item_shivas_guard",
                    ability => new NotTargetable(ability, AbilityType.Counter, AbilityFlags.CanBeCastedOnAlly)
                },

                // * strong dispels
                {
                    "obsidian_destroyer_astral_imprisonment",
                    ability =>
                    new Targetable(
                        ability,
                        AbilityType.Counter,
                        AbilityFlags.CanBeCastedOnAlly | AbilityFlags.StrongDispel)
                },
                {
                    "shadow_demon_disruption",
                    ability =>
                    new Targetable(
                        ability,
                        AbilityType.Counter,
                        AbilityFlags.CanBeCastedOnAlly | AbilityFlags.StrongDispel)
                },
                {
                    "abaddon_aphotic_shield",
                    ability =>
                    new Targetable(
                        ability,
                        AbilityType.Counter,
                        AbilityFlags.CanBeCastedOnAlly | AbilityFlags.StrongDispel)
                },
                {
                    "legion_commander_press_the_attack",
                    ability =>
                    new Targetable(
                        ability,
                        AbilityType.Counter,
                        AbilityFlags.CanBeCastedOnAlly | AbilityFlags.StrongDispel)
                },
                {
                    "omniknight_repel",
                    ability =>
                    new Targetable(
                        ability,
                        AbilityType.Counter,
                        AbilityFlags.CanBeCastedOnAlly | AbilityFlags.StrongDispel)
                },
                {
                    "winter_wyvern_cold_embrace",
                    ability =>
                    new Targetable(
                        ability,
                        AbilityType.Counter,
                        AbilityFlags.CanBeCastedOnAlly | AbilityFlags.StrongDispel)
                },
                {
                    "item_glimmer_cape",
                    ability =>
                    new Targetable(
                        ability,
                        AbilityType.Counter,
                        AbilityFlags.CanBeCastedOnAlly | AbilityFlags.StrongDispel)
                },

                // * basic dispels
                {
                    "item_lotus_orb",
                    ability =>
                    new Targetable(
                        ability,
                        AbilityType.Counter,
                        AbilityFlags.CanBeCastedOnAlly | AbilityFlags.BasicDispel)
                },
            };

        public static Dictionary<string, Func<Ability, UsableAbility>> EvadeDisableAbilities { get; } = new Dictionary
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
                //todo: check ogr aghanim stun
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
                { "slardar_slithereen_crush", ability => new NotTargetable(ability, AbilityType.Disable) },
            };

        #endregion
    }
}