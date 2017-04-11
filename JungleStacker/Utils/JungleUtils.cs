namespace JungleStacker.Utils
{
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Objects;

    internal static class JungleUtils
    {
        private static readonly List<string> DuplicateCreeps = new List<string>
        {
            "npc_dota_neutral_satyr_soulstealer",
            "npc_dota_neutral_mud_golem",
            "npc_dota_neutral_gnoll_assassin"
        };

        private static readonly List<string> UniqueCreeps = new List<string>
        {
            "npc_dota_neutral_harpy_storm",
            "npc_dota_neutral_ghost",
            "npc_dota_neutral_forest_troll_high_priest",
            "npc_dota_neutral_kobold_taskmaster",
            "npc_dota_neutral_alpha_wolf",
            "npc_dota_neutral_ogre_magi",
            "npc_dota_neutral_satyr_hellcaller",
            "npc_dota_neutral_centaur_khan",
            "npc_dota_neutral_dark_troll_warlord",
            "npc_dota_neutral_enraged_wildkin",
            "npc_dota_neutral_polar_furbolg_ursa_warrior",
            "npc_dota_neutral_big_thunder_lizard",
            "npc_dota_neutral_black_dragon",
            "npc_dota_neutral_granite_golem",
            "npc_dota_neutral_prowler_shaman"
        };

        public static int CountStacks(this List<Creep> creeps)
        {
            return creeps.Count(x => UniqueCreeps.Contains(x.StoredName()))
                   + (creeps.Count(x => DuplicateCreeps.Contains(x.StoredName()))
                      - creeps.Count(x => x.StoredName() == "npc_dota_neutral_satyr_hellcaller")) / 2;
        }
    }
}