namespace ItemManager.Utils
{
    using System.Collections.Generic;

    internal static class ModifierUtils
    {
        public static string ArmletStrength { get; } = "modifier_item_armlet_unholy_strength";

        public static string BladeMailReflect { get; } = "modifier_item_blade_mail_reflect";

        public static string BottleRegeneration { get; } = "modifier_bottle_regeneration";

        public static HashSet<string> CanUseAbilitiesInInvis { get; } = new HashSet<string>
        {
            "modifier_broodmother_spin_web_invisible_applier",
            "modifier_riki_permanent_invisibility",
            "modifier_treant_natures_guise_invis"
        };

        public static HashSet<string> DelayPowerTreadsSwitchModifiers { get; } = new HashSet<string>
        {
            // abilities
            "modifier_leshrac_pulse_nova",
            "modifier_morphling_morph_agi",
            "modifier_morphling_morph_str",
            "modifier_voodoo_restoration_aura",
            "modifier_brewmaster_primal_split",
            "modifier_eul_cyclone",
            "modifier_storm_spirit_ball_lightning",
            // heal
            "modifier_item_urn_heal",
            "modifier_flask_healing",
            "modifier_bottle_regeneration",
            "modifier_enchantress_natures_attendants",
            "modifier_oracle_purifying_flames",
            "modifier_warlock_shadow_word",
            "modifier_filler_heal",
            // invis
            "modifier_invisible",
            "modifier_bounty_hunter_wind_walk",
            "modifier_clinkz_wind_walk",
            "modifier_item_glimmer_cape_fade",
            "modifier_invoker_ghost_walk_self",
            "modifier_mirana_moonlight_shadow",
            "modifier_nyx_assassin_vendetta",
            "modifier_sandking_sand_storm_invis",
            "modifier_rune_invis",
            "modifier_item_shadow_amulet_fade",
            "modifier_item_silver_edge_windwalk",
            "modifier_item_invisibility_edge_windwalk",
            "modifier_templar_assassin_meld",
            "modifier_weaver_shukuchi"
        };

        public static string DustOfAppearance { get; } = "modifier_item_dustofappearance";

        public static string FountainRegeneration { get; } = "modifier_fountain_aura_buff";

        public static string IceBlastDebuff { get; } = "modifier_ice_blast";

        public static string LivingArmorModifier { get; } = "modifier_treant_living_armor";

        public static string ShrineRegeneration { get; } = "modifier_filler_heal";

        public static string SpiritBreakerCharge { get; } = "modifier_spirit_breaker_charge_of_darkness";

        public static string SpiritVesselDebuff { get; } = "modifier_item_spirit_vessel_damage";

        public static string SpiritVesselRegeneration { get; } = "modifier_item_spirit_vessel_heal";

        public static string TangoRegeneration { get; } = "modifier_tango_heal";

        public static string UrnDebuff { get; } = "modifier_item_urn_damage";

        public static string UrnRegeneration { get; } = "modifier_item_urn_heal";
    }
}