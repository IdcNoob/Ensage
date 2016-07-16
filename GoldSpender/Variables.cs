namespace GoldSpender
{
    using System.Collections.Generic;

    using Ensage;

    internal class Variables
    {
        #region Static Fields

        public static List<uint> HighPriorityItems = new List<uint>
                                                         {
                                                             60 //Point Booster
                                                         };

        public static Dictionary<string, uint> ItemsToBuyNearDeath = new Dictionary<string, uint>
                                                                         {
                                                                             { "item_smoke_of_deceit", 188 },
                                                                             { "item_ward_sentry", 43 },
                                                                             { "item_tome_of_knowledge", 257 },
                                                                             { "item_ward_observer", 42 },
                                                                             { "attribute_bonus", 0 },
                                                                             { "item_tpscroll", 46 }
                                                                         };

        #endregion

        #region Public Properties

        public static Hero Hero { get; set; }

        public static MenuManager MenuManager { get; set; }

        #endregion
    }
}