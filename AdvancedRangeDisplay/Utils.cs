namespace AdvancedRangeDisplay
{
    using System;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;

    internal static class Utils
    {
        #region Public Methods and Operators

        public static string GetDefaultName(this Ability ability)
        {
            var name = ability.StoredName();
            return char.IsDigit(name[name.Length - 1]) ? name.Remove(name.Length - 2) : name;
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

        #endregion
    }
}