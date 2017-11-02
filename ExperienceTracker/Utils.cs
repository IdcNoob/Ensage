namespace ExperienceTracker
{
    using System;
    using System.Collections.Generic;

    using Ensage;

    using log4net;

    using PlaySharp.Toolkit.Logging;

    internal static class Utils
    {
        private static readonly Dictionary<string, int> Experience = new Dictionary<string, int>();

        private static readonly ILog Log = AssemblyLogs.GetLogger(typeof(ExperienceTracker));

        public static int GetGrantedExperience(this Creep creep)
        {
            int exp;
            if (Experience.TryGetValue(creep.Name, out exp))
            {
                return creep.AdjustedExperience(exp);
            }

            try
            {
                exp = Game.FindKeyValues(creep.Name + "/BountyXP", KeyValueSource.Unit).IntValue;
            }
            catch (KeyValuesNotFoundException)
            {
                Log.Error("XP value for " + creep.Name + " not found");
                exp = 0;
            }

            Experience.Add(creep.Name, exp);
            return creep.AdjustedExperience(exp);
        }

        private static int AdjustedExperience(this Creep creep, int exp)
        {
            if (creep.Owner != null)
            {
                // dominated creep gives less xp
                // wtf volvo
                exp = exp / 2;
            }
            else if (creep.IsNeutral)
            {
                exp = (int)(exp * (((Math.Floor(Game.GameTime / 450) * 2) / 100) + 1));
            }

            return exp;
        }
    }
}