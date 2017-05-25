namespace ItemManager.Utils
{
    using System.Linq;

    using Ensage;

    internal static class UnitUtils
    {
        public static bool IsReallyHexed(this Unit unit)
        {
            return unit.Modifiers.Any(x => ModifierUtils.HexModifiers.Contains(x.Name));
        }
    }
}