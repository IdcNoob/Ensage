namespace GoldSpender
{
    using System.Linq;

    using Ensage;
    using Ensage.Common;

    internal class GoldSpender
    {
        #region Public Methods and Operators

        public void OnUpdate()
        {
            if (!Utils.SleepCheck("GoldSpender.Sleep"))
            {
                return;
            }

            Utils.Sleep(200 + Game.Ping, "GoldSpender.Sleep");

            if (Game.IsPaused || !Variables.Hero.IsAlive)
            {
                return;
            }

            if (Variables.MenuManager.TestEnabled)
            {
                var text = Player.QuickBuyItems.Aggregate(
                    "QB:",
                    (current, quickBuyItem) => current + (" " + quickBuyItem));
                Game.PrintMessage(text, MessageType.LogMessage);
            }

            var module = Variables.Modules.FirstOrDefault(x => x.ShouldSpendGold());
            module?.BuyItems();
        }

        #endregion
    }
}