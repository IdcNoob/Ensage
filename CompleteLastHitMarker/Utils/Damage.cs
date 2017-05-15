namespace CompleteLastHitMarker.Utils
{
    using Ensage;

    internal class Damage
    {
        public static float Multiplier(AttackDamageType attackType, ArmorType armorType)
        {
            switch (armorType)
            {
                case ArmorType.Basic:
                    switch (attackType)
                    {
                        case AttackDamageType.Pierce:
                            return 1.5f;
                    }
                    break;
                case ArmorType.Structure:
                    switch (attackType)
                    {
                        case AttackDamageType.Basic:
                            return 0.7f;
                        case AttackDamageType.Pierce:
                            return 0.35f;
                        case AttackDamageType.Siege:
                            return 2.5f;
                        case AttackDamageType.Hero:
                            return 0.5f;
                    }
                    break;
                case ArmorType.Hero:
                    switch (attackType)
                    {
                        case AttackDamageType.Basic:
                            return 0.75f;
                        case AttackDamageType.Pierce:
                            return 0.5f;
                        case AttackDamageType.Siege:
                            return 0.85f;
                    }
                    break;
            }

            return 1;
        }
    }
}