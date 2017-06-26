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

        public static List<string> IgnoredBools { get; } = new List<string>
        {
            "m_bIsMoving",
            "m_bHidden",
            "m_bIsGeneratingEconItem",
            "m_bInitialized",
            "m_bCanBeDominated",
            "m_bSelectionRingVisible",
            "m_bActivated",
            "m_bSellable",
            "m_bKillable",
            "m_bPurchasable",
            "m_bDroppable",
            "m_bCombinable",
            "m_bStackable",
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
            "m_flStartSequenceCycle",
            "m_flPlaybackRate",
            "m_flLastSpawnTime",
            "m_flMusicOperatorVals",
            "m_fStuns",
            "fStockTime",
            "m_vecMins",
            "m_vecMaxs"
        };

        public static List<string> SemiIgnoredFloats { get; } = new List<string>
        {
            "m_fCooldown",
            "m_flCooldownLength",
            "m_flPurchaseTime",
            "m_flAssembledTime",
            "m_flRadarCooldowns",
            "m_flElasticity",
            "m_flScale"
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
            "m_cellX",
            "m_cellZ",
            "m_NetworkSequenceIndex",
            "m_nNewSequenceParity"
        };

        public static List<string> SemiIgnoredInt32 { get; } = new List<string>
        {
            "m_anglediff",
            "m_NetworkActivity",
            "m_iHealth",
            "m_iTaggedAsVisibleByTeam",
            "m_iHeroDamage",
            "m_iRecentDamage",
            "m_iDamageBonus",
            "m_iPauseTeam"
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