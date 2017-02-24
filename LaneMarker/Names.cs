﻿namespace LaneMarker
{
    using System.Collections.Generic;

    internal static class Names
    {
        #region Static Fields

        public static Dictionary<string, string> Heroes = new Dictionary<string, string>
        {
            { "Io, Wisp", "npc_dota_hero_wisp" },
            { "Axe", "npc_dota_hero_axe" },
            { "Bane", "npc_dota_hero_bane" },
            { "Chen​", "npc_dota_hero_chen" },
            { "Doom​, Doombringer", "npc_dota_hero_doom_bringer" },
            { "Lich​", "npc_dota_hero_lich" },
            { "Lina​", "npc_dota_hero_lina" },
            { "Lion​", "npc_dota_hero_lion" },
            { "Luna", "npc_dota_hero_luna" },
            { "Puck​", "npc_dota_hero_puck" },
            { "Riki​", "npc_dota_hero_riki" },
            { "Sven​", "npc_dota_hero_sven" },
            { "Tiny​", "npc_dota_hero_tiny" },
            { "Zeus", "npc_dota_hero_zuus" },
            { "Abaddon", "npc_dota_hero_abaddon" },
            { "Alchemist", "npc_dota_hero_alchemist" },
            { "Ancient Apparition, AA", "npc_dota_hero_ancient_apparition" },
            { "Anti-Mage, AM, Magina", "npc_dota_hero_antimage" },
            { "Arc Warden​", "npc_dota_hero_arc_warden" },
            { "Batrider", "npc_dota_hero_batrider" },
            { "Beastmaster​, Rexxar", "npc_dota_hero_beastmaster" },
            { "Bloodseeker​, BS", "npc_dota_hero_bloodseeker" },
            { "Bounty Hunter​, Gondar, BH", "npc_dota_hero_bounty_hunter" },
            { "Brewmaster​", "npc_dota_hero_brewmaster" },
            { "Bristleback​", "npc_dota_hero_bristleback" },
            { "Broodmother​", "npc_dota_hero_broodmother" },
            { "Centaur Warrunner​", "npc_dota_hero_centaur" },
            { "Chaos Knight​", "npc_dota_hero_chaos_knight" },
            { "Clinkz​, Bone Fletcher", "npc_dota_hero_clinkz" },
            { "Clockwerk, Rattletrap​", "npc_dota_hero_rattletrap" },
            { "Crystal Maiden​, CM", "npc_dota_hero_crystal_maiden" },
            { "Dark Seer​, DS", "npc_dota_hero_dark_seer" },
            { "Dazzle​", "npc_dota_hero_dazzle" },
            { "Death Prophet, Krobelus, DP​", "npc_dota_hero_death_prophet" },
            { "Disruptor, Thrall​", "npc_dota_hero_disruptor" },
            { "Dragon Knight, DK​", "npc_dota_hero_dragon_knight" },
            { "Drow Ranger​, DR, Trax", "npc_dota_hero_drow_ranger" },
            { "Earth Spirit, ES​", "npc_dota_hero_earth_spirit" },
            { "Earthshaker​", "npc_dota_hero_earthshaker" },
            { "Elder Titan​", "npc_dota_hero_elder_titan" },
            { "Ember Spirit​", "npc_dota_hero_ember_spirit" },
            { "Enchantress​", "npc_dota_hero_enchantress" },
            { "Enigma​", "npc_dota_hero_enigma" },
            { "Faceless Void​", "npc_dota_hero_faceless_void" },
            { "Gyrocopter​", "npc_dota_hero_gyrocopter" },
            { "Huskar​", "npc_dota_hero_huskar" },
            { "Invoker​", "npc_dota_hero_invoker" },
            { "Jakiro, Twin Headed Dragon, THD​", "npc_dota_hero_jakiro" },
            { "Juggernaut​", "npc_dota_hero_juggernaut" },
            { "Keeper of the Light, Gandalf​", "npc_dota_hero_keeper_of_the_light" },
            { "Kunkka​", "npc_dota_hero_kunkka" },
            { "Legion Commander, LC​", "npc_dota_hero_legion_commander" },
            { "Leshrac​", "npc_dota_hero_leshrac" },
            { "Lifestealer, Naix, LS​", "npc_dota_hero_life_stealer" },
            { "Lone Druid​, Sylla, LD", "npc_dota_hero_lone_druid" },
            { "Lycan", "npc_dota_hero_lycan" },
            { "Magnus", "npc_dota_hero_magnataur" },
            { "Medusa", "npc_dota_hero_medusa" },
            { "Meepo, Geomancer", "npc_dota_hero_meepo" },
            { "Mirana", "npc_dota_hero_mirana" },
            { "Monkey King, MK", "npc_dota_hero_monkey_king" },
            { "Morphling​", "npc_dota_hero_morphling" },
            { "Naga Siren​", "npc_dota_hero_naga_siren" },
            { "Nature's Prophet, Furion​", "npc_dota_hero_furion" },
            { "Necrophos​", "npc_dota_hero_necrolyte" },
            { "Night Stalker, Balanar, NS", "npc_dota_hero_night_stalker" },
            { "Nyx Assassin​, Nerubian", "npc_dota_hero_nyx_assassin" },
            { "Ogre Magi​", "npc_dota_hero_ogre_magi" },
            { "Omniknight​", "npc_dota_hero_omniknight" },
            { "Oracle", "npc_dota_hero_oracle" },
            { "Outworld Devourer, Obsidian Destroyer, OD​", "npc_dota_hero_obsidian_destroyer" },
            { "Phantom Assassin, Mortred, PA​", "npc_dota_hero_phantom_assassin" },
            { "Phantom Lancer​, PL", "npc_dota_hero_phantom_lancer" },
            { "Phoenix​", "npc_dota_hero_phoenix" },
            { "Pudge​, Butcher", "npc_dota_hero_pudge" },
            { "Pugna​", "npc_dota_hero_pugna" },
            { "Queen of Pain, Akasha, QOP​", "npc_dota_hero_queenofpain" },
            { "Razor​", "npc_dota_hero_razor" },
            { "Rubick​", "npc_dota_hero_rubick" },
            { "Sand King​", "npc_dota_hero_sand_king" },
            { "Shadow Demon​", "npc_dota_hero_shadow_demon" },
            { "Shadow Fiend, SF, Nevermore​​", "npc_dota_hero_nevermore" },
            { "Shadow Shaman​, Rasta", "npc_dota_hero_shadow_shaman" },
            { "Silencer​", "npc_dota_hero_silencer" },
            { "Skywrath Mage​", "npc_dota_hero_skywrath_mage" },
            { "Slardar​", "npc_dota_hero_slardar" },
            { "Slark​", "npc_dota_hero_slark" },
            { "Sniper​", "npc_dota_hero_sniper" },
            { "Spectre​", "npc_dota_hero_spectre" },
            { "Spirit Breaker, Barathrum​", "npc_dota_hero_spirit_breaker" },
            { "Storm Spirit​", "npc_dota_hero_storm_spirit" },
            { "Techies​​, Fun", "npc_dota_hero_techies" },
            { "Templar Assassin, Lanaya​", "npc_dota_hero_templar_assassin" },
            { "Terrorblade, TB​", "npc_dota_hero_terrorblade" },
            { "Tidehunter​", "npc_dota_hero_tidehunter" },
            { "Timbersaw​", "npc_dota_hero_shredder" },
            { "Tinker​", "npc_dota_hero_tinker" },
            { "Treant Protector​", "npc_dota_hero_treant" },
            { "Troll Warlord​", "npc_dota_hero_troll_warlord" },
            { "Tusk​", "npc_dota_hero_tusk" },
            { "Undying, Dirge​", "npc_dota_hero_undying" },
            { "Ursa​", "npc_dota_hero_ursa" },
            { "Vengeful Spirit​", "npc_dota_hero_vengefulspirit" },
            { "Venomancer​", "npc_dota_hero_venomancer" },
            { "Viper​", "npc_dota_hero_viper" },
            { "Visage​", "npc_dota_hero_visage" },
            { "Warlock​", "npc_dota_hero_warlock" },
            { "Weaver​", "npc_dota_hero_weaver" },
            { "Windranger​, Windrunner, WR", "npc_dota_hero_windrunner" },
            { "Winter Wyvern, WW​​", "npc_dota_hero_winter_wyvern" },
            { "Wraith King​, Leoric, Skeleton King, WK", "npc_dota_hero_skeleton_king" },
            { "Witch Doctor​", "npc_dota_hero_witch_doctor" }
        };

        #endregion
    }
}