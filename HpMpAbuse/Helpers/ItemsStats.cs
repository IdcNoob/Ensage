namespace HpMpAbuse.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;

    internal class ItemsStats
    {
        #region Fields

        private readonly List<string> bonusAllStats = new List<string>
        {
            "bonus_all_stats",
            "bonus_stats"
        };

        private readonly List<string> bonusHealth = new List<string>
        {
            "bonus_strength",
            "bonus_str",
            "bonus_health",
        };

        private readonly List<string> bonusMana = new List<string>
        {
            "bonus_intellect",
            "bonus_int",
            "bonus_mana",
        };

        private readonly Dictionary<uint, Stats> savedStats = new Dictionary<uint, Stats>();

        #endregion

        #region Enums

        [Flags]
        public enum Stats
        {
            None = 0,

            Any = 1,

            Health = Any | 2,

            Mana = Any | 4,

            All = Health | Mana
        }

        #endregion

        #region Public Methods and Operators

        public Stats GetStats(Item item)
        {
            Stats stats;
            if (savedStats.TryGetValue(item.Handle, out stats))
            {
                return stats;
            }

            if (item.AbilitySpecialData.Any(x => bonusAllStats.Contains(x.Name)))
            {
                stats = Stats.Health | Stats.Mana;
            }
            else
            {
                if (item.AbilitySpecialData.Any(x => bonusHealth.Contains(x.Name)))
                {
                    stats |= Stats.Health;
                }
                if (item.AbilitySpecialData.Any(x => bonusMana.Contains(x.Name)))
                {
                    stats |= Stats.Mana;
                }
            }

            savedStats.Add(item.Handle, stats);
            return stats;
        }

        #endregion
    }
}