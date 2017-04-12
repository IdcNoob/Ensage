namespace Debugger
{
    using System.Collections.Generic;

    internal static class Data
    {
        public static List<string> IgnoredAbilities { get; } = new List<string>
        {
            "special_",
            "_empty",
            "_hidden"
        };

        public static List<string> IgnoredFireEvents { get; } = new List<string>
        {
            "dota_action_success"
        };

        public static List<string> IgnoredFloats { get; } = new List<string>
        {
            "m_vecX",
            "m_vecY",
            "m_vecZ",
            "m_fGameTime",
            "m_flStartSequenceCycle"
        };

        public static List<string> IgnoredInt32 { get; } = new List<string>
        {
            "m_iFoWFrameNumber",
            "m_iNetTimeOfDay",
            "m_iNetWorth",
            "m_iIncomeGold",
            "m_iTotalEarnedGold",
            "m_iUnreliableGold",
            "m_iReliableGold",
            "m_nServerOrderSequenceNumber",
            "m_nResetEventsParity",
            "m_cellY",
            "m_NetworkSequenceIndex"
        };

        public static List<string> IgnoredModifiers { get; } = new List<string>
        {
            "modifier_projectile_vision",
            "modifier_truesight",
            "modifier_creep_haste",
            "modifier_creep_slow",
            "modifier_tower_aura",
            "modifier_tower_truesight_aura"
        };

        public static List<string> IgnoredParticles { get; } = new List<string>
        {
            "ui_mouseactions",
            "generic_hit_blood",
            "base_attacks",
            "generic_gameplay",
            "ensage_ui"
        };

        public static List<string> IgnoredUnits { get; } = new List<string>
        {
            "portrait_world_unit"
        };
    }
}