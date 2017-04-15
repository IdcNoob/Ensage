namespace ItemManager.Utils
{
    using System;

    using Ensage;

    internal static class HeroUtils
    {
        public static int BuybackCost(this Hero hero)
        {
            return (int)(100 + Math.Pow(hero.Level, 2) * 1.5 + Game.GameTime / 60 * 15);
        }

        public static bool IsAtBase(this Hero hero)
        {
            return hero.ActiveShop == ShopType.Base;
        }

        public static bool ItemsCanBeDisabled(this Hero hero)
        {
            return hero.ActiveShop == ShopType.None;
        }

        public static float RespawnTime(this Hero hero)
        {
            return (float)3.8 * hero.Level + 5 + hero.RespawnTimePenalty;
        }
    }
}