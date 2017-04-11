namespace ExperienceTracker
{
    using System;
    using System.Collections.Generic;

    using Ensage;

    internal static class Utils
    {
        private static readonly Dictionary<string, int> Experience = new Dictionary<string, int>();

        public static int GetGrantedExperience(this Creep creep)
        {
            int exp;
            if (Experience.TryGetValue(creep.Name, out exp))
            {
                return exp;
            }

            try
            {
                exp = Game.FindKeyValues(creep.Name + "/BountyXP", KeyValueSource.Unit).IntValue;
            }
            catch (KeyValuesNotFoundException)
            {
                Console.WriteLine("[ExperienceTracker] Warning! XP for " + creep.Name + " not found.");
                exp = 0;
            }

            Experience.Add(creep.Name, exp);
            return exp;
        }
    }
}