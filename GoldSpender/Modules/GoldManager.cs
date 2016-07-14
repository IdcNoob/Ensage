namespace GoldSpender.Modules
{
    using System;

    using Ensage;
    using Ensage.Common.Objects.UtilityObjects;

    internal abstract class GoldManager
    {
        #region Fields

        protected Sleeper Sleeper = new Sleeper();

        #endregion

        #region Properties

        protected int BuybackCost => (int)(100 + Math.Pow(Hero.Level, 2) * 1.5 + Game.GameTime / 60 * 15);

        protected int GoldLossOnDeath => (int)Math.Min(Hero.Level * 30, UnreliableGold);

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
            var reliableGold = ReliableGold;
            var unreliableGold = UnreliableGold;

            var buybackCost = BuybackCost + GoldLossOnDeath;

            if (unreliableGold + reliableGold >= buybackCost)
            {
                if (buybackCost - reliableGold > 0)
                {
                    buybackCost -= reliableGold;
                    if (buybackCost - unreliableGold <= 0)
                    {
                        reliableGold = 0;
                        unreliableGold -= buybackCost;
                    }
                }
                else
                {
                    reliableGold -= buybackCost;
                }
            }

            reliableOut = reliableGold;
            unreliableOut = unreliableGold;
        }

        protected bool ShouldSaveForBuyback(float time)
        {
            return time > 0 && Game.GameTime / 60 > time && Hero.Player.BuybackCooldownTime <= 15;
        }

        #endregion
    }
}