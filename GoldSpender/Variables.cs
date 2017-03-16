namespace GoldSpender
{
    using System.Collections.Generic;

    using Ensage;

    internal class Variables
    {
        #region Static Fields

        public static List<AbilityId> HighPriorityItems = new List<AbilityId>
        {
            AbilityId.item_point_booster //Point Booster
        };

        public static Dictionary<string, AbilityId> ItemsToBuyNearDeath = new Dictionary<string, AbilityId>
        {
            { "item_smoke_of_deceit", AbilityId.item_smoke_of_deceit },
            { "item_dust", AbilityId.item_dust },
            { "item_tome_of_knowledge", AbilityId.item_tome_of_knowledge },
            { "item_ward_sentry", AbilityId.item_ward_sentry },
            { "item_ward_observer", AbilityId.item_ward_observer },
            { "attribute_bonus", AbilityId.ability_base },
            { "item_tpscroll", AbilityId.item_tpscroll }
        };

        #endregion

        #region Public Properties

        public static Hero Hero { get; set; }

        public static MenuManager MenuManager { get; set; }

        #endregion
    }
}