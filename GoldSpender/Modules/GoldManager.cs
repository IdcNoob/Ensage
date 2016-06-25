namespace GoldSpender.Modules
{
    using Ensage;

    internal abstract class GoldManager
    {
        #region Properties

        protected Hero Hero => Variables.Hero;

        protected MenuManager Menu => Variables.MenuManager;

        protected int TotalGold => UnreliableGold + Hero.Player.ReliableGold;

        protected int UnreliableGold => Hero.Player.UnreliableGold;

        #endregion

        #region Public Methods and Operators

        public abstract bool ShouldSpendGold();

        public abstract void BuyItems();

        #endregion
    }
}