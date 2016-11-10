namespace Evader.Data
{
    using System.Collections.Generic;

    internal static class AdditionalAbilityData
    {
        #region Public Properties

        public static Dictionary<string, string> ModifierThinkers { get; } = new Dictionary<string, string>
        {
            { "modifier_invoker_sun_strike", "invoker_sun_strike" },
            { "modifier_kunkka_torrent_thinker", "kunkka_torrent" },
            { "modifier_leshrac_split_earth_thinker", "leshrac_split_earth" },
            { "modifier_lina_light_strike_array", "lina_light_strike_array" },
            { "modifier_disruptor_static_storm_thinker", "disruptor_static_storm" },
            { "modifier_faceless_void_chronosphere", "faceless_void_chronosphere" },
            { "modifier_enigma_black_hole_thinker", "enigma_black_hole" }
        };

        public static Dictionary<string, string> Particles { get; } = new Dictionary<string, string>
        {
            { "meathook", "pudge_meat_hook" },
            { "pounce_trail", "slark_pounce" },
            { "powershot_channel", "windrunner_powershot" },
            { "venomous_gale", "venomancer_venomous_gale" },
            { "poison_nova", "venomancer_poison_nova" },
            { "whirling_axe_melee", "troll_warlord_whirling_axes_melee" }
        };

        #endregion
    }
}