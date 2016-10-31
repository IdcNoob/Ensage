namespace Evader.Common
{
    using System;

    using Ensage;
    using Ensage.Common.Extensions;

    internal static class Utils
    {
        #region Public Methods and Operators

        public static string AbilityName(this Modifier modifier)
        {
            var modifierTextureName = modifier.TextureName;
            var modifierName = modifier.Name;

            if (modifierName == "modifier_stunned" && modifierTextureName == "earthshaker_aftershock")
            {
                return "earthshaker_enchant_totem";
            }
            if (modifierName == "modifier_skeleton_king_hellfire_blast")
            {
                // stun dot modifier
                return null;
            }

            return modifierTextureName;
        }

        public static string GetName(this Unit unit)
        {
            var hero = unit as Hero;
            if (hero != null)
            {
                return hero.GetRealName();
            }

            switch (unit.Name)
            {
                case "npc_dota_neutral_centaur_khan":
                    return "Centaur Conqueror";
                case "npc_dota_neutral_satyr_hellcaller":
                    return "Satyr Tormenter";
            }

            Debugger.WriteLine("Real name not found for: " + unit.Name);
            return string.Empty;
        }

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
    }
}