namespace InformationPinger
{
    using System.Collections.Generic;

    using Ensage.Common.Objects.UtilityObjects;

    internal class Variables
    {
        #region Static Fields

        public static List<string> Abilities = new List<string>
            {
                "tidehunter_ravage",
                "enigma_black_hole",
                "leoric_reincarnate",
                "ancient_apparition_ice_blast",
                "batrider_flaming_lasso",
                "beastmaster_primal_roar",
                "bloodseeker_rupture",
                "rattletrap_hookshot",
                "doom_bringer_doom",
                "earthshaker_echo_slam",
                "faceless_void_chronosphere",
                "legion_commander_duel",
                "lina_laguna_blade",
                "lion_finger_of_death",
                "luna_eclipse",
                "magnataur_reverse_polarity",
                "nyx_assassin_vendetta",
                "silencer_global_silence",
                "spectre_haunt",
                "warlock_rain_of_chaos",
                "zuus_thundergods_wrath"
            };

        public static List<string> IncludedItems = new List<string>
            {
                "item_smoke_of_deceit",
                "item_dust",
                "item_gem",
                "item_ghost"
            };

        #endregion

        #region Public Properties

        public static MultiSleeper Sleeper { get; set; }

        #endregion
    }
}