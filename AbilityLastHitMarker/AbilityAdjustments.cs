namespace AbilityLastHitMarker
{
    using System.Collections.Generic;

    internal class AbilityAdjustments
    {
        #region Static Fields

        public static readonly List<string> IgnoredAbilities = new List<string>
                                                                   {
                                                                       "tinker_heat_seeking_missile",
                                                                       "zuus_thundergods_wrath"
                                                                   };

        public static readonly List<string> IncludedAbilities = new List<string>
                                                                    {
                                                                        "techies_land_mines", "techies_remote_mines",
                                                                        "meepo_poof", "clinkz_searing_arrows"
                                                                    };

        public static readonly Dictionary<string, int> TowerDamageAbilities = new Dictionary<string, int>
                                                                                  {
                                                                                      { "pugna_nether_blast", 2 },
                                                                                      { "tiny_toss", 3 },
                                                                                      { "techies_land_mines", 1 }
                                                                                  };

        #endregion
    }
}