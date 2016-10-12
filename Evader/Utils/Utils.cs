namespace Evader.Utils
{
    using System;

    using Ensage;
    using Ensage.Common.Extensions;

    internal static class Utils
    {
        #region Public Methods and Operators

        //public static ClassID? AbilityClassID(this Modifier modifier)
        //{
        //    var name = modifier.Name;

        //    if (name == "modifier_stunned" || name == "modifier_silence")
        //    {
        //        name = modifier.TextureName;
        //    }

        //    ClassID? id;
        //    ModiferAbilityClassID.TryGetValue(name, out id);

        //    return id;
        //}

        public static float GetRealCastRange(this Ability ability)
        {
            var castRange = ability.GetCastRange();

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

        public static bool IsTurning(this Unit unit)
        {
            return (int)unit.RotationDifference != 0;
        }

        #endregion

        //private static readonly Dictionary<string, ClassID?> ModiferAbilityClassID = new Dictionary<string, ClassID?>
        //    {
        //        { "lina_light_strike_array", ClassID.CDOTA_Ability_Lina_LightStrikeArray },
        //        { "death_prophet_silence", ClassID.CDOTA_Ability_DeathProphet_Silence },
        //    };
    }
}