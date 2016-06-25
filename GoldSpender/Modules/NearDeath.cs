namespace GoldSpender.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;

    internal class NearDeath : GoldManager
    {
        #region Fields

        private readonly List<uint> boughtItems = new List<uint>();

        private Team enemyTeam = Team.None;

        private int lastGold;

        #endregion

        #region Properties

        private Team EnemyTeam
        {
            get
            {
                if (enemyTeam == Team.None)
                {
                    enemyTeam = Hero.GetEnemyTeam();
                }
                return enemyTeam;
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void BuyItems()
        {
            var item =
                Variables.ItemsToBuy.OrderByDescending(x => Menu.GetNearDeathItemPriority(x.Key))
                    .FirstOrDefault(x => Menu.IsNearDeathItemActive(x.Key) && !boughtItems.Contains(x.Value));

            var itemId = item.Value;

            if (itemId > 0)
            {
                var itemName = item.Key;

                if (itemId == 1)
                {
                    if (Math.Abs(lastGold - TotalGold) > 10)
                    {
                        BuyItem(itemId);
                    }
                    else
                    {
                        boughtItems.Add(itemId);
                    }
                }
                else
                {
                    var preventPurshase = false;

                    switch (itemId)
                    {
                        case 46: // tp
                            if (GetCount(itemName) >= 2 || Hero.FindItem("item_travel_boots", true) != null
                                || Hero.FindItem("item_travel_boots_2", true) != null)
                            {
                                preventPurshase = true;
                            }
                            break;
                        case 43: // sentry
                            if (GetWardsCount(itemName) >= 3)
                            {
                                preventPurshase = true;
                            }
                            break;
                    }

                    if (!preventPurshase)
                    {
                        BuyItem(itemId);
                    }

                    boughtItems.Add(itemId);
                }
            }
            else
            {
                Utils.Sleep(20000, "GoldSpender.NearDeath.AllBought");
                boughtItems.Clear();
            }
        }

        public override bool ShouldSpendGold()
        {
            if (!Utils.SleepCheck("GoldSpender.NearDeath.AllBought"))
            {
                return false;
            }

            var disableTime = Menu.NearDeathAutoDisableTime;

            if (!Menu.NearDeathEnabled || disableTime > 0 && Game.GameTime / 60 > disableTime || UnreliableGold < 20)
            {
                if (boughtItems.Any())
                {
                    boughtItems.Clear();
                }
                return false;
            }

            var distance = Menu.NearDeathEnemyDistance;

            return (float)Hero.Health / Hero.MaximumHealth <= (float)Menu.NearDeathHpThreshold / 100
                   && (distance <= 0
                       || Heroes.GetByTeam(EnemyTeam)
                              .Any(x => !x.IsIllusion && x.IsAlive && x.Distance2D(Hero) <= distance));
        }

        #endregion

        #region Methods

        private void BuyItem(uint id)
        {
            if (id == 1)
            {
                Game.ExecuteCommand("dota_purchase_quickbuy");
                lastGold = TotalGold;
            }
            else
            {
                Player.BuyItem(Hero, id);
            }
        }

        private uint GetCount(string itemName)
        {
            return (Hero.Inventory.Items.FirstOrDefault(x => x.Name == itemName)?.CurrentCharges ?? 0)
                   + (Hero.Inventory.StashItems.FirstOrDefault(x => x.Name == itemName)?.CurrentCharges ?? 0);
        }

        private uint GetWardsCount(string itemName)
        {
            var inventoryDispenser = Hero.Inventory.Items.FirstOrDefault(x => x.Name == "item_ward_dispenser");
            var stashDispenser = Hero.Inventory.StashItems.FirstOrDefault(x => x.Name == "item_ward_dispenser");

            return GetCount(itemName)
                   + (itemName == "item_ward_observer"
                          ? (inventoryDispenser?.CurrentCharges ?? 0) + (stashDispenser?.CurrentCharges ?? 0)
                          : (inventoryDispenser?.SecondaryCharges ?? 0) + (stashDispenser?.SecondaryCharges ?? 0));
        }

        #endregion
    }
}