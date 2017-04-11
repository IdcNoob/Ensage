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
    using EvadableAbilities.Heroes.Chen;
    using EvadableAbilities.Heroes.Clinkz;
    using EvadableAbilities.Heroes.Clockwerk;
    using EvadableAbilities.Heroes.CrystalMaiden;
    using EvadableAbilities.Heroes.DarkSeer;
    using EvadableAbilities.Heroes.Dazzle;
    using EvadableAbilities.Heroes.DeathProphet;
    using EvadableAbilities.Heroes.Disruptor;
    using EvadableAbilities.Heroes.Doom;
    using EvadableAbilities.Heroes.DragonKnight;
    using EvadableAbilities.Heroes.DrowRanger;
    using EvadableAbilities.Heroes.Earthshaker;
    using EvadableAbilities.Heroes.EarthSpirit;
    using EvadableAbilities.Heroes.ElderTitan;
    using EvadableAbilities.Heroes.EmberSpirit;
    using EvadableAbilities.Heroes.Enchantress;
    using EvadableAbilities.Heroes.Enigma;
    using EvadableAbilities.Heroes.FacelessVoid;
    using EvadableAbilities.Heroes.Gyrocopter;
    using EvadableAbilities.Heroes.Huskar;
    using EvadableAbilities.Heroes.Invoker;
    using EvadableAbilities.Heroes.Jakiro;
    using EvadableAbilities.Heroes.Juggernaut;
    using EvadableAbilities.Heroes.KeeperOfTheLight;
    using EvadableAbilities.Heroes.Kunkka;
    using EvadableAbilities.Heroes.LegionCommander;
    using EvadableAbilities.Heroes.Leshrac;
    using EvadableAbilities.Heroes.Lich;
    using EvadableAbilities.Heroes.Lifestealer;
    using EvadableAbilities.Heroes.Lina;
    using EvadableAbilities.Heroes.Lion;
    using EvadableAbilities.Heroes.Luna;
    using EvadableAbilities.Heroes.Magnus;
    using EvadableAbilities.Heroes.Medusa;
    using EvadableAbilities.Heroes.Meepo;
    using EvadableAbilities.Heroes.Mirana;
    using EvadableAbilities.Heroes.MonkeyKing;
    using EvadableAbilities.Heroes.Morphling;
    using EvadableAbilities.Heroes.NagaSiren;
    using EvadableAbilities.Heroes.NaturesProphet;
    using EvadableAbilities.Heroes.Necrophos;
    using EvadableAbilities.Heroes.NightStalker;
    using EvadableAbilities.Heroes.NyxAssassin;
    using EvadableAbilities.Heroes.OgreMagi;
    using EvadableAbilities.Heroes.Omniknight;
    using EvadableAbilities.Heroes.Oracle;
    using EvadableAbilities.Heroes.OutworldDevourer;
    using EvadableAbilities.Heroes.PhantomAssassin;
    using EvadableAbilities.Heroes.PhantomLancer;
    using EvadableAbilities.Heroes.Phoenix;
    using EvadableAbilities.Heroes.Puck;
    using EvadableAbilities.Heroes.Pudge;
    using EvadableAbilities.Heroes.Pugna;
    using EvadableAbilities.Heroes.QueenOfPain;
    using EvadableAbilities.Heroes.Razor;
    using EvadableAbilities.Heroes.Riki;
    using EvadableAbilities.Heroes.Rubick;
    using EvadableAbilities.Heroes.SandKing;
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
    using EvadableAbilities.Heroes.Techies;
    using EvadableAbilities.Heroes.TemplarAssassin;
    using EvadableAbilities.Heroes.Terrorblade;
    using EvadableAbilities.Heroes.Tidehunter;
    using EvadableAbilities.Heroes.Timbersaw;
    using EvadableAbilities.Heroes.Tinker;
    using EvadableAbilities.Heroes.Tiny;
    using EvadableAbilities.Heroes.TreantProtector;
    using EvadableAbilities.Heroes.TrollWarlord;
    using EvadableAbilities.Heroes.Tusk;
    using EvadableAbilities.Heroes.Underlord;
    using EvadableAbilities.Heroes.Undying;
    using EvadableAbilities.Heroes.Ursa;
    using EvadableAbilities.Heroes.VengefulSpirit;
    using EvadableAbilities.Heroes.Venomancer;
    using EvadableAbilities.Heroes.Viper;
    using EvadableAbilities.Heroes.Visage;
    using EvadableAbilities.Heroes.Warlock;
    using EvadableAbilities.Heroes.Weaver;
    using EvadableAbilities.Heroes.Windranger;
    using EvadableAbilities.Heroes.WinterWyvern;
    using EvadableAbilities.Heroes.WitchDoctor;
    using EvadableAbilities.Heroes.WraithKing;
    using EvadableAbilities.Heroes.Zeus;
    using EvadableAbilities.Items;
    using EvadableAbilities.Units.AncientProwlerShaman;
    using EvadableAbilities.Units.CentaurConqueror;
    using EvadableAbilities.Units.DarkTrollSummoner;
    using EvadableAbilities.Units.Familiar;
    using EvadableAbilities.Units.HellbearSmasher;
    using EvadableAbilities.Units.PrimalSplit;
    using EvadableAbilities.Units.SatyrTormenter;
    using EvadableAbilities.Units.SpiritBear;

    using Void = EvadableAbilities.Heroes.NightStalker.Void;

    internal class EnemyAbilities
    {
        public Dictionary<string, Func<Ability, EvadableAbility>> EvadableAbilities { get; } =
            new Dictionary<string, Func<Ability, EvadableAbility>>
            {
                { "abaddon_death_coil", ability => new MistCoil(ability) },
                { "abaddon_borrowed_time", ability => new BorrowedTime(ability) },
                { "alchemist_unstable_concoction_throw", ability => new UnstableConcoction(ability) },
                { "ancient_apparition_ice_blast", ability => new IceBlast(ability) },
                { "antimage_blink", ability => new AntiMageBlink(ability) },
                { "antimage_mana_void", ability => new ManaVoid(ability) },
                { "arc_warden_flux", ability => new Flux(ability) },
                { "arc_warden_spark_wraith", ability => new SparkWraith(ability) },
                { "axe_berserkers_call", ability => new BerserkersCall(ability) },
                { "axe_battle_hunger", ability => new BattleHunger(ability) },
                { "axe_culling_blade", ability => new CullingBlade(ability) },
                { "bane_brain_sap", ability => new BrainSap(ability) },
                { "bane_nightmare", ability => new Nightmare(ability) },
                { "bane_fiends_grip", ability => new FiendsGrip(ability) },
                { "batrider_sticky_napalm", ability => new StickyNapalm(ability) },
                { "batrider_flaming_lasso", ability => new FlamingLasso(ability) },
                { "beastmaster_wild_axes", ability => new WildAxes(ability) },
                { "beastmaster_primal_roar", ability => new PrimalRoar(ability) },
                { "bloodseeker_blood_bath", ability => new BloodRite(ability) },
                { "bloodseeker_rupture", ability => new Rupture(ability) },
                { "bounty_hunter_shuriken_toss", ability => new ShurikenToss(ability) },
                { "bounty_hunter_track", ability => new Track(ability) },
                { "brewmaster_thunder_clap", ability => new ThunderClap(ability) },
                { "brewmaster_drunken_haze", ability => new DrunkenHaze(ability) },
                { "brewmaster_primal_split", ability => new PrimalSplit(ability) },
                { "broodmother_spawn_spiderlings", ability => new SpawnSpiderlings(ability) },
                { "broodmother_insatiable_hunger", ability => new InsatiableHunger(ability) },
                { "centaur_hoof_stomp", ability => new HoofStomp(ability) },
                { "centaur_double_edge", ability => new DoubleEdge(ability) },
                { "chaos_knight_chaos_bolt", ability => new ChaosBolt(ability) },
                { "chaos_knight_reality_rift", ability => new RealityRift(ability) },
                { "chaos_knight_phantasm", ability => new Phantasm(ability) },
                { "chen_penitence", ability => new Penitence(ability) },
                { "chen_test_of_faith", ability => new TestOfFaith(ability) },
                { "chen_test_of_faith_teleport", ability => new TestOfFaithTeleport(ability) },
                { "chen_hand_of_god", ability => new HandOfGod(ability) },
                { "clinkz_strafe", ability => new Strafe(ability) },
                { "rattletrap_power_cogs", ability => new PowerCogs(ability) },
                { "rattletrap_rocket_flare", ability => new RocketFlare(ability) },
                { "rattletrap_hookshot", ability => new Hookshot(ability) },
                { "crystal_maiden_crystal_nova", ability => new CrystalNova(ability) },
                { "crystal_maiden_frostbite", ability => new Frostbite(ability) },
                { "crystal_maiden_freezing_field", ability => new FreezingField(ability) },
                { "dark_seer_vacuum", ability => new Vacuum(ability) },
                { "dark_seer_surge", ability => new Surge(ability) },
                { "dark_seer_wall_of_replica", ability => new WallOfReplica(ability) },
                { "dazzle_poison_touch", ability => new PoisonTouch(ability) },
                { "dazzle_shadow_wave", ability => new ShadowWave(ability) },
                { "dazzle_shallow_grave", ability => new ShallowGrave(ability) },
                { "death_prophet_carrion_swarm", ability => new CryptSwarm(ability) },
                { "death_prophet_silence", ability => new Silence(ability) },
                { "death_prophet_spirit_siphon", ability => new SpiritSiphon(ability) },
                { "death_prophet_exorcism", ability => new Exorcism(ability) },
                { "disruptor_thunder_strike", ability => new ThunderStrike(ability) },
                { "disruptor_glimpse", ability => new Glimpse(ability) },
                { "disruptor_static_storm", ability => new StaticStorm(ability) },
                { "doom_bringer_doom", ability => new Doom(ability) },
                { "dragon_knight_breathe_fire", ability => new BreatheFire(ability) },
                { "dragon_knight_dragon_tail", ability => new DragonTail(ability) },
                { "drow_ranger_wave_of_silence", ability => new WaveOfSilence(ability) },
                { "earthshaker_fissure", ability => new Fissure(ability) },
                { "earthshaker_enchant_totem", ability => new EnchantTotem(ability) },
                { "earthshaker_aftershock", ability => new Aftershock(ability) },
                { "earth_spirit_boulder_smash", ability => new BoulderSmash(ability) },
                { "earth_spirit_rolling_boulder", ability => new RollingBoulder(ability) },
                { "earth_spirit_geomagnetic_grip", ability => new GeomagneticGrip(ability) },
                { "earth_spirit_magnetize", ability => new Magnetize(ability) },
                { "elder_titan_echo_stomp", ability => new EchoStomp(ability) },
                { "elder_titan_earth_splitter", ability => new EarthSplitter(ability) },
                { "ember_spirit_sleight_of_fist", ability => new SleightOfFist(ability) },
                { "ember_spirit_searing_chains", ability => new SearingChains(ability) },
                { "ember_spirit_flame_guard", ability => new FlameGuard(ability) },
                { "enchantress_enchant", ability => new Enchant(ability) },
                { "enigma_malefice", ability => new Malefice(ability) },
                { "enigma_black_hole", ability => new BlackHole(ability) },
                { "faceless_void_time_walk", ability => new TimeWalk(ability) },
                { "faceless_void_time_dilation", ability => new TimeDilation(ability) },
                { "faceless_void_chronosphere", ability => new Chronosphere(ability) },
                { "gyrocopter_rocket_barrage", ability => new RocketBarrage(ability) },
                { "gyrocopter_homing_missile", ability => new HomingMissile(ability) },
                { "gyrocopter_call_down", ability => new CallDown(ability) },
                { "huskar_inner_vitality", ability => new InnerVitality(ability) },
                { "huskar_life_break", ability => new LifeBreak(ability) },
                { "invoker_cold_snap", ability => new ColdSnap(ability) },
                { "invoker_emp", ability => new EMP(ability) },
                { "invoker_alacrity", ability => new Alacrity(ability) },
                { "invoker_chaos_meteor", ability => new ChaosMeteor(ability) },
                { "invoker_deafening_blast", ability => new DeafeningBlast(ability) },
                { "invoker_sun_strike", ability => new SunStrike(ability) },
                { "invoker_tornado", ability => new Tornado(ability) },
                { "jakiro_dual_breath", ability => new DualBreath(ability) },
                { "jakiro_ice_path", ability => new IcePath(ability) },
                { "jakiro_liquid_fire", ability => new LiquidFire(ability) },
                { "jakiro_macropyre", ability => new Macropyre(ability) },
                { "juggernaut_blade_fury", ability => new BladeFury(ability) },
                { "juggernaut_omni_slash", ability => new Omnislash(ability) },
                { "keeper_of_the_light_illuminate", ability => new Illuminate(ability) },
                { "keeper_of_the_light_mana_leak", ability => new ManaLeak(ability) },
                { "keeper_of_the_light_blinding_light", ability => new BlindingLight(ability) },
                { "kunkka_torrent", ability => new Torrent(ability) },
                { "kunkka_tidebringer", ability => new Tidebringer(ability) },
                { "kunkka_x_marks_the_spot", ability => new Xmark(ability) },
                { "kunkka_return", ability => new Xreturn(ability) },
                { "kunkka_ghostship", ability => new Ghostship(ability) },
                { "legion_commander_overwhelming_odds", ability => new OverwhelmingOdds(ability) },
                { "legion_commander_press_the_attack", ability => new PressTheAttack(ability) },
                { "legion_commander_duel", ability => new Duel(ability) },
                { "leshrac_split_earth", ability => new SplitEarth(ability) },
                { "leshrac_lightning_storm", ability => new LightningStorm(ability) },
                { "leshrac_pulse_nova", ability => new PulseNova(ability) },
                { "lich_frost_nova", ability => new FrostBlast(ability) },
                { "lich_frost_armor", ability => new IceArmor(ability) },
                { "lich_chain_frost", ability => new ChainFrost(ability) },
                { "life_stealer_open_wounds", ability => new OpenWounds(ability) },
                { "lina_dragon_slave", ability => new DragonSlave(ability) },
                { "lina_light_strike_array", ability => new LightStrikeArray(ability) },
                { "lina_laguna_blade", ability => new LagunaBlade(ability) },
                { "lion_impale", ability => new EarthSpike(ability) },
                { "lion_voodoo", ability => new LionHex(ability) },
                { "lion_finger_of_death", ability => new FingerOfDeath(ability) },
                { "luna_lucent_beam", ability => new LucentBeam(ability) },
                { "luna_eclipse", ability => new Eclipse(ability) },
                { "magnataur_shockwave", ability => new Shockwave(ability) },
                { "magnataur_empower", ability => new Empower(ability) },
                { "magnataur_skewer", ability => new Skewer(ability) },
                { "magnataur_reverse_polarity", ability => new ReversePolarity(ability) },
                { "medusa_mystic_snake", ability => new MysticSnake(ability) },
                { "meepo_earthbind", ability => new Earthbind(ability) },
                { "meepo_poof", ability => new Poof(ability) },
                { "mirana_starfall", ability => new Starstorm(ability) },
                { "mirana_arrow", ability => new SacredArrow(ability) },
                { "mirana_invis", ability => new MoonlightShadow(ability) },
                { "monkey_king_boundless_strike", ability => new BoundlessStrike(ability) },
                { "monkey_king_primal_spring", ability => new PrimalSpring(ability) },
                { "monkey_king_jingu_mastery", ability => new JinguMastery(ability) },
                { "morphling_waveform", ability => new Waveform(ability) },
                { "morphling_adaptive_strike", ability => new AdaptiveStrike(ability) },
                { "morphling_morph_replicate", ability => new ReplicateTeleport(ability) },
                { "naga_siren_ensnare", ability => new Ensnare(ability) },
                { "naga_siren_rip_tide", ability => new RipTide(ability) },
                { "naga_siren_song_of_the_siren", ability => new SongOfTheSiren(ability) },
                { "furion_sprout", ability => new Sprout(ability) },
                { "necrolyte_death_pulse", ability => new DeathPulse(ability) },
                { "necrolyte_reapers_scythe", ability => new ReapersScythe(ability) },
                { "night_stalker_void", ability => new Void(ability) },
                { "night_stalker_crippling_fear", ability => new CripplingFear(ability) },
                { "nyx_assassin_impale", ability => new Impale(ability) },
                { "nyx_assassin_mana_burn", ability => new ManaBurn(ability) },
                { "nyx_assassin_vendetta", ability => new Vendetta(ability) },
                { "ogre_magi_fireblast", ability => new Fireblast(ability) },
                { "ogre_magi_unrefined_fireblast", ability => new Fireblast(ability) },
                { "ogre_magi_ignite", ability => new Ignite(ability) },
                { "omniknight_purification", ability => new Purification(ability) },
                { "omniknight_guardian_angel", ability => new GuardianAngel(ability) },
                { "oracle_fortunes_end", ability => new FortunesEnd(ability) },
                { "oracle_fates_edict", ability => new FatesEdict(ability) },
                { "oracle_purifying_flames", ability => new PurifyingFlames(ability) },
                { "oracle_false_promise", ability => new FalsePromise(ability) },
                { "obsidian_destroyer_astral_imprisonment", ability => new AstralImprisonment(ability) },
                { "obsidian_destroyer_sanity_eclipse", ability => new SanitysEclipse(ability) },
                { "phantom_assassin_stifling_dagger", ability => new StiflingDagger(ability) },
                { "phantom_assassin_phantom_strike", ability => new PhantomStrike(ability) },
                { "phantom_lancer_spirit_lance", ability => new SpiritLance(ability) },
                { "phoenix_icarus_dive", ability => new IcarusDive(ability) },
                { "phoenix_launch_fire_spirit", ability => new FireSpirits(ability) },
                { "phoenix_supernova", ability => new Supernova(ability) },
                { "puck_illusory_orb", ability => new IllusoryOrb(ability) },
                { "puck_waning_rift", ability => new WaningRift(ability) },
                { "puck_dream_coil", ability => new DreamCoil(ability) },
                { "pudge_meat_hook", ability => new MeatHook(ability) },
                { "pudge_rot", ability => new Rot(ability) },
                { "pudge_dismember", ability => new Dismember(ability) },
                { "pugna_nether_blast", ability => new NetherBlast(ability) },
                { "pugna_decrepify", ability => new Decrepify(ability) },
                { "pugna_life_drain", ability => new LifeDrain(ability) },
                { "queenofpain_shadow_strike", ability => new ShadowStrike(ability) },
                { "queenofpain_blink", ability => new QueenOfPainBlink(ability) },
                { "queenofpain_scream_of_pain", ability => new ScreamOfPain(ability) },
                { "queenofpain_sonic_wave", ability => new SonicWave(ability) },
                { "razor_plasma_field", ability => new PlasmaField(ability) },
                { "razor_static_link", ability => new StaticLink(ability) },
                { "razor_unstable_current", ability => new UnstableCurrent(ability) },
                { "razor_eye_of_the_storm", ability => new EyeOfTheStorm(ability) },
                { "riki_smoke_screen", ability => new SmokeScreen(ability) },
                { "riki_blink_strike", ability => new BlinkStrike(ability) },
                { "riki_tricks_of_the_trade", ability => new TricksOfTheTrade(ability) },
                { "rubick_telekinesis", ability => new Telekinesis(ability) },
                { "sandking_burrowstrike", ability => new Burrowstrike(ability) },
                { "sandking_epicenter", ability => new Epicenter(ability) },
                { "shadow_demon_disruption", ability => new Disruption(ability) },
                { "shadow_demon_soul_catcher", ability => new SoulCatcher(ability) },
                { "shadow_demon_shadow_poison", ability => new ShadowPoison(ability) },
                { "shadow_demon_demonic_purge", ability => new DemonicPurge(ability) },
                { "nevermore_shadowraze1", ability => new Shadowraze(ability) },
                { "nevermore_shadowraze2", ability => new Shadowraze(ability) },
                { "nevermore_shadowraze3", ability => new Shadowraze(ability) },
                { "nevermore_requiem", ability => new RequiemOfSouls(ability) },
                { "shadow_shaman_ether_shock", ability => new EtherShock(ability) },
                { "shadow_shaman_voodoo", ability => new ShamanHex(ability) },
                { "shadow_shaman_shackles", ability => new Shackles(ability) },
                { "silencer_curse_of_the_silent", ability => new ArcaneCurse(ability) },
                { "silencer_last_word", ability => new LastWord(ability) },
                { "silencer_global_silence", ability => new GlobalSilence(ability) },
                { "skywrath_mage_arcane_bolt", ability => new ArcaneBolt(ability) },
                { "skywrath_mage_concussive_shot", ability => new ConcussiveShot(ability) },
                { "skywrath_mage_ancient_seal", ability => new AncientSeal(ability) },
                { "skywrath_mage_mystic_flare", ability => new MysticFlare(ability) },
                { "slardar_slithereen_crush", ability => new SlithereenCrush(ability) },
                { "slardar_amplify_damage", ability => new AmplifyDamage(ability) },
                { "slark_dark_pact", ability => new DarkPact(ability) },
                { "slark_pounce", ability => new Pounce(ability) },
                { "sniper_shrapnel", ability => new Shrapnel(ability) },
                { "sniper_assassinate", ability => new Assassinate(ability) },
                { "spectre_spectral_dagger", ability => new SpectralDagger(ability) },
                { "spirit_breaker_charge_of_darkness", ability => new ChargeOfDarkness(ability) },
                { "spirit_breaker_nether_strike", ability => new NetherStrike(ability) },
                { "storm_spirit_static_remnant", ability => new StaticRemnant(ability) },
                { "storm_spirit_electric_vortex", ability => new Vortex(ability) },
                { "sven_storm_bolt", ability => new StormHammer(ability) },
                { "sven_warcry", ability => new Warcry(ability) },
                { "sven_gods_strength", ability => new GodsStrength(ability) },
                { "techies_suicide", ability => new BlastOff(ability) },
                { "templar_assassin_meld", ability => new Meld(ability) },
                { "templar_assassin_trap", ability => new PsionicTrap(ability) },
                { "terrorblade_reflection", ability => new Reflection(ability) },
                { "terrorblade_metamorphosis", ability => new Metamorphosis(ability) },
                { "terrorblade_sunder", ability => new Sunder(ability) },
                { "tidehunter_gush", ability => new Gush(ability) },
                { "tidehunter_anchor_smash", ability => new AnchorSmash(ability) },
                { "tidehunter_ravage", ability => new Ravage(ability) },
                { "shredder_whirling_death", ability => new WhirlingDeath(ability) },
                { "shredder_timber_chain", ability => new TimberChain(ability) },
                { "shredder_chakram", ability => new Chakram(ability) },
                { "shredder_chakram_2", ability => new Chakram(ability) },
                { "tinker_laser", ability => new Laser(ability) },
                { "tinker_heat_seeking_missile", ability => new HeatSeekingMissile(ability) },
                { "tiny_avalanche", ability => new Avalanche(ability) },
                { "treant_natures_guise", ability => new NaturesGuise(ability) },
                { "treant_leech_seed", ability => new LeechSeed(ability) },
                { "treant_living_armor", ability => new LivingArmor(ability) },
                { "treant_overgrowth", ability => new Overgrowth(ability) },
                { "troll_warlord_whirling_axes_melee", ability => new WhirlingAxesMelee(ability) },
                { "troll_warlord_whirling_axes_ranged", ability => new WhirlingAxesRanged(ability) },
                { "troll_warlord_battle_trance", ability => new BattleTrance(ability) },
                { "tusk_ice_shards", ability => new IceShards(ability) },
                { "tusk_snowball", ability => new Snowball(ability) },
                { "tusk_walrus_punch", ability => new WalrusPunch(ability) },
                { "tusk_walrus_kick", ability => new WalrusKick(ability) },
                { "abyssal_underlord_firestorm", ability => new Firestorm(ability) },
                { "abyssal_underlord_pit_of_malice", ability => new PitOfMalice(ability) },
                { "abyssal_underlord_dark_rift", ability => new DarkRift(ability) },
                { "undying_decay", ability => new Decay(ability) },
                { "undying_soul_rip", ability => new SoulRip(ability) },
                { "undying_tombstone", ability => new Tombstone(ability) },
                { "ursa_earthshock", ability => new Earthshock(ability) },
                { "ursa_overpower", ability => new Overpower(ability) },
                { "ursa_enrage", ability => new Enrage(ability) },
                { "vengefulspirit_magic_missile", ability => new MagicMissile(ability) },
                { "vengefulspirit_wave_of_terror", ability => new WaveOfTerror(ability) },
                { "vengefulspirit_nether_swap", ability => new NetherSwap(ability) },
                { "venomancer_venomous_gale", ability => new VenomousGale(ability) },
                { "venomancer_poison_nova", ability => new PoisonNova(ability) },
                { "viper_viper_strike", ability => new ViperStrike(ability) },
                { "viper_corrosive_skin", ability => new CorrosiveSkin(ability) },
                { "visage_grave_chill", ability => new GraveChill(ability) },
                { "visage_soul_assumption", ability => new SoulAssumption(ability) },
                { "warlock_fatal_bonds", ability => new FatalBonds(ability) },
                { "warlock_shadow_word", ability => new ShadowWord(ability) },
                { "warlock_rain_of_chaos", ability => new ChaoticOffering(ability) },
                { "weaver_time_lapse", ability => new TimeLapse(ability) },
                { "windrunner_shackleshot", ability => new Shackleshot(ability) },
                { "windrunner_powershot", ability => new Powershot(ability) },
                { "windrunner_windrun", ability => new Windrun(ability) },
                { "windrunner_focusfire", ability => new FocusFire(ability) },
                { "winter_wyvern_splinter_blast", ability => new SplinterBlast(ability) },
                { "winter_wyvern_winters_curse", ability => new WintersCurse(ability) },
                { "witch_doctor_paralyzing_cask", ability => new ParalyzingCask(ability) },
                { "witch_doctor_maledict", ability => new Maledict(ability) },
                { "witch_doctor_death_ward", ability => new DeathWard(ability) },
                { "skeleton_king_hellfire_blast", ability => new WraithfireBlast(ability) },
                { "zuus_arc_lightning", ability => new ArcLightning(ability) },
                { "zuus_lightning_bolt", ability => new LightningBolt(ability) },
                { "zuus_thundergods_wrath", ability => new ThundergodsWrath(ability) },
                { "zuus_cloud", ability => new Nimbus(ability) },
                { "item_ethereal_blade", ability => new EtherealBlade(ability) },
                { "item_rod_of_atos", ability => new RodOfAtos(ability) },
                { "item_sheepstick", ability => new ScytheOfVyse(ability) },
                { "item_orchid", ability => new OrchidMalevolence(ability) },
                { "item_bloodthorn", ability => new Bloodthorn(ability) },
                { "item_ghost", ability => new Ghost(ability) },
                { "item_medallion_of_courage", ability => new MedallionOfCourage(ability) },
                { "item_solar_crest", ability => new SolarCrest(ability) },
                { "item_veil_of_discord", ability => new VeilOfDiscord(ability) },
                { "item_abyssal_blade", ability => new AbyssalBlade(ability) },
                { "item_diffusal_blade", ability => new DiffusalBblade(ability) },
                { "item_diffusal_blade_2", ability => new DiffusalBblade(ability) },
                { "item_mjollnir", ability => new Mjollnir(ability) },
                { "item_satanic", ability => new Satanic(ability) },
                { "item_dust", ability => new DustOfAppearance(ability) },
                { "centaur_khan_war_stomp", ability => new WarStomp(ability) },
                { "satyr_hellcaller_shockwave", ability => new Hadouken(ability) },
                { "lone_druid_spirit_bear_entangle", ability => new Entangle(ability) },
                { "brewmaster_earth_hurl_boulder", ability => new HurlBoulder(ability) },
                { "brewmaster_storm_cyclone", ability => new Cyclone(ability) },
                { "visage_summon_familiars_stone_form", ability => new StoneForm(ability) },
                { "dark_troll_warlord_ensnare", ability => new TrollEnsnare(ability) },
                { "mud_golem_hurl_boulder", ability => new HurlBoulder(ability) },
                { "spawnlord_master_stomp", ability => new Desecrate(ability) },
                { "spawnlord_master_freeze", ability => new Petrify(ability) },
                { "polar_furbolg_ursa_warrior_thunder_clap", ability => new HellbearThunderClap(ability) }
            };
    }
}