namespace Evader.Data
{
    using System;
    using System.Collections.Generic;

    using Ensage;

    using EvadableAbilities.Base;
    using EvadableAbilities.Heroes.Abaddon;
    using EvadableAbilities.Heroes.Alchemist;
    using EvadableAbilities.Heroes.AncientApparition;
    using EvadableAbilities.Heroes.AntiMage;
    using EvadableAbilities.Heroes.ArcWarden;
    using EvadableAbilities.Heroes.Axe;
    using EvadableAbilities.Heroes.Bane;
    using EvadableAbilities.Heroes.Batrider;
    using EvadableAbilities.Heroes.Beastmaster;
    using EvadableAbilities.Heroes.Bloodseeker;
    using EvadableAbilities.Heroes.BountyHunter;
    using EvadableAbilities.Heroes.Brewmaster;
    using EvadableAbilities.Heroes.Broodmother;
    using EvadableAbilities.Heroes.CentaurWarrunner;
    using EvadableAbilities.Heroes.ChaosKnight;
    using EvadableAbilities.Heroes.Clockwerk;
    using EvadableAbilities.Heroes.CrystalMaiden;
    using EvadableAbilities.Heroes.DarkSeer;
    using EvadableAbilities.Heroes.Dazzle;
    using EvadableAbilities.Heroes.DeathProphet;
    using EvadableAbilities.Heroes.Disruptor;
    using EvadableAbilities.Heroes.Doom;
    using EvadableAbilities.Heroes.DrowRanger;
    using EvadableAbilities.Heroes.Earthshaker;
    using EvadableAbilities.Heroes.ElderTitan;
    using EvadableAbilities.Heroes.Enigma;
    using EvadableAbilities.Heroes.FacelessVoid;
    using EvadableAbilities.Heroes.Huskar;
    using EvadableAbilities.Heroes.Invoker;
    using EvadableAbilities.Heroes.Jakiro;
    using EvadableAbilities.Heroes.Juggernaut;
    using EvadableAbilities.Heroes.KeeperOfTheLight;
    using EvadableAbilities.Heroes.Kunkka;
    using EvadableAbilities.Heroes.LegionCommander;
    using EvadableAbilities.Heroes.Leshrac;
    using EvadableAbilities.Heroes.Lich;
    using EvadableAbilities.Heroes.Lina;
    using EvadableAbilities.Heroes.Lion;
    using EvadableAbilities.Heroes.Luna;
    using EvadableAbilities.Heroes.Magnus;
    using EvadableAbilities.Heroes.Medusa;
    using EvadableAbilities.Heroes.Mirana;
    using EvadableAbilities.Heroes.Morphling;
    using EvadableAbilities.Heroes.NagaSiren;
    using EvadableAbilities.Heroes.Necrophos;
    using EvadableAbilities.Heroes.NightStalker;
    using EvadableAbilities.Heroes.NyxAssassin;
    using EvadableAbilities.Heroes.OgreMagi;
    using EvadableAbilities.Heroes.Omniknight;
    using EvadableAbilities.Heroes.Oracle;
    using EvadableAbilities.Heroes.OutworldDevourer;
    using EvadableAbilities.Heroes.PhantomAssassin;
    using EvadableAbilities.Heroes.PhantomLancer;
    using EvadableAbilities.Heroes.Puck;
    using EvadableAbilities.Heroes.Pudge;
    using EvadableAbilities.Heroes.QueenOfPain;
    using EvadableAbilities.Heroes.ShadowDemon;
    using EvadableAbilities.Heroes.ShadowFiend;
    using EvadableAbilities.Heroes.ShadowShaman;
    using EvadableAbilities.Heroes.Silencer;
    using EvadableAbilities.Heroes.SkywrathMage;
    using EvadableAbilities.Heroes.Slardar;
    using EvadableAbilities.Heroes.Slark;
    using EvadableAbilities.Heroes.Sniper;
    using EvadableAbilities.Heroes.Spectre;
    using EvadableAbilities.Heroes.SpiritBreaker;
    using EvadableAbilities.Heroes.StormSpirit;
    using EvadableAbilities.Heroes.Sven;
    using EvadableAbilities.Heroes.Terrorblade;
    using EvadableAbilities.Heroes.Tidehunter;
    using EvadableAbilities.Heroes.Timbersaw;
    using EvadableAbilities.Heroes.Tinker;
    using EvadableAbilities.Heroes.TreantProtector;
    using EvadableAbilities.Heroes.TrollWarlord;
    using EvadableAbilities.Heroes.Tusk;
    using EvadableAbilities.Heroes.Undying;
    using EvadableAbilities.Heroes.Ursa;
    using EvadableAbilities.Heroes.VengefulSpirit;
    using EvadableAbilities.Heroes.Venomancer;
    using EvadableAbilities.Heroes.Viper;
    using EvadableAbilities.Heroes.Visage;
    using EvadableAbilities.Heroes.Warlock;
    using EvadableAbilities.Heroes.Windranger;
    using EvadableAbilities.Heroes.WitchDoctor;
    using EvadableAbilities.Heroes.WraithKing;
    using EvadableAbilities.Heroes.Zeus;
    using EvadableAbilities.Items;
    using EvadableAbilities.Units.CentaurConqueror;
    using EvadableAbilities.Units.SatyrTormenter;

    using Void = EvadableAbilities.Heroes.NightStalker.Void;

    #region

    #endregion

    internal class EnemyAbilities
    {
        #region Public Properties

        public Dictionary<string, Func<Ability, EvadableAbility>> EvadableAbilities { get; } = new Dictionary
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
                { "ancient_apparition_ice_blast", ability => new IceBlast(ability) },


                //// default
                { "lich_chain_frost", ability => new ChainFrost(ability) },
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
                { "legion_commander_duel", ability => new Duel(ability) },

                
                //// modifier only
                { "broodmother_insatiable_hunger", ability => new InsatiableHunger(ability) },
                { "abaddon_borrowed_time", ability => new BorrowedTime(ability) },
                { "lion_voodoo", ability => new LionHex(ability) },
                { "item_sheepstick", ability => new ScytheOfVyse(ability) },
                { "shadow_shaman_voodoo", ability => new ShamanHex(ability) },
                { "phantom_assassin_phantom_strike", ability => new PhantomStrike(ability) },
                { "silencer_global_silence", ability => new GlobalSilence(ability) },
                { "puck_waning_rift", ability => new WaningRift(ability) },
                { "skywrath_mage_ancient_seal", ability => new AncientSeal(ability) },
                { "item_orchid", ability => new OrchidMalevolence(ability) },
                { "item_bloodthorn", ability => new Bloodthorn(ability) },


                //// units
                { "centaur_khan_war_stomp", ability => new WarStomp(ability) },
                { "satyr_hellcaller_shockwave", ability => new Hadouken(ability) }
                //     { "visage_summon_familiars_stone_form",ability => new StoneForm( ability) },
            };

        #endregion
    }
}