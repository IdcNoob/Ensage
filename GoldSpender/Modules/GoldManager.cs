namespace GoldSpender.Modules
{
    using System;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Objects.UtilityObjects;

    internal abstract class GoldManager
    {
        #region Fields

        protected Sleeper Sleeper = new Sleeper();

        #endregion

        #region Properties

        protected int BuybackCost => (int)(100 + Math.Pow(Hero.Level, 2) * 1.5 + Game.GameTime / 60 * 15);

        protected int GoldLossOnDeath
            =>
                Math.Min(
                    UnreliableGold,
                    100
                    + (ReliableGold + UnreliableGold
                       + ObjectManager.GetEntitiesParallel<Item>()
                           .Where(
                               x => x != null && x.IsValid && x.Owner != null && x.Owner.IsValid && x.Owner.Equals(Hero))
                           .Sum(x => (int)x.Cost)) / 50);

        protected Hero Hero => Variables.Hero;

        protected MenuManager Menu => Variables.MenuManager;

        protected int ReliableGold => Hero.Player.ReliableGold;

        protected int UnreliableGold => Hero.Player.UnreliableGold;

        #endregion

        #region Public Methods and Operators

        public abstract void BuyItems();

        public abstract bool ShouldSpendGold();

        #endregion

        #region Methods

        protected void SaveBuyBackGold(out int reliableOut, out int unreliableOut)
        {
            reliableOut = ReliableGold;
            unreliableOut = UnreliableGold;

            var buybackCost = BuybackCost + GoldLossOnDeath;

            if (unreliableOut + reliableOut >= buybackCost)
            {
                if (buybackCost - reliableOut > 0)
                {
                    buybackCost -= reliableOut;
                    if (buybackCost - unreliableOut <= 0)
                    {
                        reliableOut = 0;
                        unreliableOut -= buybackCost;
                    }
                }
                else
                {
                    reliableOut -= buybackCost;
                }
            }
        }

        protected bool ShouldSaveForBuyback(float time)
        {
            return time > 0 && Game.GameTime / 60 > time
                   && Hero.Player.BuybackCooldownTime < 3.8 * Hero.Level + 5 + Hero.RespawnTimePenalty;
        }

        #endregion
    }
}