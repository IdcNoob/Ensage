namespace VisionControl
{
    using System;
    using System.Collections.Generic;

    using Ensage;

    using Units;
    using Units.Mines;
    using Units.Wards;

    internal static class Variables
    {
        #region Static Fields

        public static readonly Dictionary<string, string> UnitAbilityNames = new Dictionary<string, string>
        {
            { "npc_dota_techies_land_mine", "techies_land_mines" },
            { "npc_dota_techies_stasis_trap", "techies_stasis_trap" },
            { "npc_dota_techies_remote_mine", "techies_remote_mines" },
            { "npc_dota_templar_assassin_psionic_trap", "templar_assassin_trap" },
            { "npc_dota_pugna_nether_ward_1", "pugna_nether_ward" },
            { "npc_dota_pugna_nether_ward_2", "pugna_nether_ward" },
            { "npc_dota_pugna_nether_ward_3", "pugna_nether_ward" },
            { "npc_dota_pugna_nether_ward_4", "pugna_nether_ward" },
            { "npc_dota_treant_eyes", "treant_eyes_in_the_forest" },
            { "npc_dota_unit_tombstone1", "undying_tombstone" },
            { "npc_dota_unit_tombstone2", "undying_tombstone" },
            { "npc_dota_unit_tombstone3", "undying_tombstone" },
            { "npc_dota_unit_tombstone4", "undying_tombstone" },
            { "npc_dota_venomancer_plague_ward_1", "venomancer_plague_ward" },
            { "npc_dota_venomancer_plague_ward_2", "venomancer_plague_ward" },
            { "npc_dota_venomancer_plague_ward_3", "venomancer_plague_ward" },
            { "npc_dota_venomancer_plague_ward_4", "venomancer_plague_ward" },
            { "npc_dota_sentry_wards", "item_ward_sentry" },
            { "npc_dota_observer_wards", "item_ward_observer" }
        };

        public static readonly Dictionary<string, Func<Unit, IUnit>> Units = new Dictionary<string, Func<Unit, IUnit>>
        {
            { "techies_land_mines", x => new LandMine(x) },
            { "techies_stasis_trap", x => new StasisTrap(x) },
            { "techies_remote_mines", x => new RemoteMine(x) },
            { "templar_assassin_trap", x => new PsionicTrap(x) },
            { "pugna_nether_ward", x => new NetherWard(x) },
            { "treant_eyes_in_the_forest", x => new TreantEye(x) },
            { "undying_tombstone", x => new Tombstone(x) },
            { "venomancer_plague_ward", x => new PlagueWard(x) },
            { "item_ward_sentry", x => new SentryWard(x) },
            { "item_ward_observer", x => new ObserverWard(x) },
        };

        #endregion

        #region Public Properties

        public static MenuManager Menu { get; set; }

        #endregion
    }
}