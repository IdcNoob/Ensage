namespace GoldSpender.Modules
{
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;

    using Utils;

    internal class AutoPurchase : GoldManager
    {
        #region Constructors and Destructors

        public AutoPurchase()
        {
            Player.OnExecuteOrder += OnExecuteOrder;
        }

        ~AutoPurchase()
        {
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        #endregion

        #region Public Methods and Operators

        public override void BuyItems()
        {
            var reliableGold = ReliableGold;
            var unreliableGold = UnreliableGold;

            if (ShouldSaveForBuyback(Menu.AutoPurchaseSaveBuybackTime))
            {
                SaveBuyBackGold(out reliableGold, out unreliableGold);
            }

            var gold = reliableGold + unreliableGold;
            var itemsToBuy = new List<uint>();

            foreach (var itemID in
                Player.QuickBuyItems.OrderByDescending(x => Variables.HighPriorityItems.Contains(x))
                    .Where(x => ItemsData.IsPurchasable(x, Hero.ActiveShop)))
            {
                switch (itemID)
                {
                    case AbilityId.item_energy_booster:
                        if (Hero.FindItem("item_arcane_boots") != null)
                        {
                            continue;
                        }
                        break;
                }

                var cost = Ability.GetAbilityDataByID((uint)itemID).Cost;

                if (gold >= cost)
                {
                    itemsToBuy.Add((uint)itemID);
                    gold -= (int)cost;
                }
            }

            if (Hero.ActiveShop != ShopType.Base)
            {
                itemsToBuy = itemsToBuy.Take(Hero.Inventory.FreeSlots.Count()).ToList();
            }

            itemsToBuy.ForEach(x => Player.BuyItem(Hero, x));

            if (itemsToBuy.Any())
            {
                Sleeper.Sleep(10000);
            }
        }

        public override bool ShouldSpendGold()
        {
            if (Sleeper.Sleeping || !Menu.AutoPurchaseEnabled)
            {
                return false;
            }

            return Player.QuickBuyItems.Any() && Hero.ActiveShop != ShopType.None;
        }

        #endregion

        #region Methods

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (args.Order == Order.DisassembleItem)
            {
                Sleeper.Sleep(10000);
            }
        }

        #endregion
    }
}