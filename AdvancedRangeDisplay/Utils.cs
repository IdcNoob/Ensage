namespace AdvancedRangeDisplay
{
    using System;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;

    internal static class Utils
    {
        public static string GetDefaultName(this Ability ability)
        {
            if (ability == null || !ability.IsValid)
            {
                return string.Empty;
            }

            var name = ability.StoredName();
            return char.IsDigit(name[name.Length - 1]) ? name.Remove(name.Length - 2) : name;
        }

        public static float GetRealAttackRange(this Hero hero)
        {
            var range = hero.GetAttackRange();

            if (hero.AttackCapability == AttackCapability.Ranged)
            {
                range += Math.Max(range / 9, 80);
            }
            else
            {
                range += 25;
            }

            return range;
        }

        public static float GetRealCastRange(this Ability ability)
        {
            var castRange = ability.GetCastRange();

            if (castRange <= 0 || castRange >= 5000)
            {
                castRange = ability.GetRadius();
            }

            if (!ability.IsAbilityBehavior(AbilityBehavior.NoTarget))
            {
                castRange += Math.Max(castRange / 9, 80);
            }
            else
            {
                castRange += Math.Max(castRange / 7, 40);
            }

            return castRange;
        }
    }
}