namespace ItemManager.Utils
{
    using System;
    using System.Linq;

    using Core;

    using Ensage;

    internal static class HeroUtils
    {
        public static int BuybackCost(this Hero hero)
        {
            return (int)(100 + Math.Pow(hero.Level, 2) * 1.5 + Game.GameTime / 60 * 15);
        }

        public static int GoldLossOnDeath(this Hero hero, ItemManager items)
        {
            return Math.Min(
                hero.Player.UnreliableGold,
                100 + (hero.Player.ReliableGold + hero.Player.UnreliableGold + items.AllItems.Sum(x => (int)x.Cost))
                / 50);
        }

        public static float RespawnTime(this Hero hero)
        {
            return (float)3.8 * hero.Level + 5 + hero.RespawnTimePenalty;
        }
    }
}