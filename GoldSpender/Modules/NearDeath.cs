namespace GoldSpender.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    using Utils;

    internal class NearDeath : GoldManager
    {
        #region Fields

        private readonly Team enemyTeam;

        #endregion

        #region Constructors and Destructors

        public NearDeath()
        {
            enemyTeam = Hero.GetEnemyTeam();
        }

        #endregion

        #region Public Methods and Operators

        public override void BuyItems()
        {
            var enabledItems =
                Variables.ItemsToBuyNearDeath.OrderByDescending(x => Menu.GetNearDeathItemPriority(x.Key))
                    .Where(x => Menu.IsNearDeathItemActive(x.Key))
                    .Select(x => x.Value)
                    .ToList();

            if (enabledItems.Contains(0))
            {
                enabledItems.InsertRange(
                    enabledItems.FindIndex(x => x == 0),
                    Player.QuickBuyItems.OrderByDescending(x => Ability.GetAbilityDataByID((uint)x).Cost));
                enabledItems.Remove(0);
            }

            var itemsToBuy = new List<Tuple<Unit, uint>>();
            var unreliableGold = UnreliableGold;
            var reliableGold = ReliableGold;

            if (ShouldSaveForBuyback(Menu.NearDeathSaveBuybackTime))
            {
                SaveBuyBackGold(out reliableGold, out unreliableGold);
            }

            var courier = ObjectManager.GetEntitiesFast<Courier>().FirstOrDefault(x => x.Team != enemyTeam && x.IsAlive);

            foreach (var itemID in enabledItems)
            {
                Unit unit;

                if (ItemsData.IsPurchasable(itemID, Hero.ActiveShop))
                {
                    unit = Hero;
                }
                else if (courier != null && ItemsData.IsPurchasable(itemID, courier.ActiveShop))
                {
                    unit = courier;
                }
                else
                {
                    continue;
                }

                switch (itemID)
                {
                    case AbilityId.item_dust:
                        if (GetItemCount(itemID) >= 2
                            || !ObjectManager.GetEntitiesParallel<Ability>()
                                .Any(x => x.Owner?.Team == enemyTeam && x.IsInvis()))
                        {
                            continue;
                        }
                        break;
                    case AbilityId.item_ward_observer:
                    case AbilityId.item_ward_sentry:
                        if (GetWardsCount(itemID) >= 2)
                        {
                            continue;
                        }
                        break;
                    case AbilityId.item_tpscroll:
                        if (GetItemCount(itemID) >= 2 || Hero.FindItem("item_travel_boots", true) != null
                            || Hero.FindItem("item_travel_boots_2", true) != null)
                        {
                            continue;
                        }
                        break;
                    case AbilityId.item_energy_booster:
                        if (Hero.FindItem("item_arcane_boots") != null)
                        {
                            continue;
                        }
                        break;
                    case AbilityId.item_smoke_of_deceit:
                        if (GetItemCount(itemID) >= 1)
                        {
                            continue;
                        }
                        break;
                    case AbilityId.item_tome_of_knowledge:
                        if (Hero.Level >= 25)
                        {
                            continue;
                        }
                        break;
                }

                var cost = Ability.GetAbilityDataByID((uint)itemID).Cost;

                if (unreliableGold >= cost)
                {
                    itemsToBuy.Add(Tuple.Create(unit, (uint)itemID));
                    unreliableGold -= (int)cost;
                }
                else if (reliableGold + unreliableGold >= cost)
                {
                    itemsToBuy.Add(Tuple.Create(unit, (uint)itemID));
                    break;
                }
            }

            if (!itemsToBuy.Any() || unreliableGold > GoldLossOnDeath || !Hero.IsAlive)
            {
                return;
            }

            itemsToBuy.ForEach(x => Player.BuyItem(x.Item1, x.Item2));
            Sleeper.Sleep(20000);
        }

        public override bool ShouldSpendGold()
        {
            if (Sleeper.Sleeping)
            {
                return false;
            }

            if (!Menu.NearDeathEnabled || GoldLossOnDeath <= 20)
            {
                return false;
            }

            if (!Utils.SleepCheck("GoldSpender.ForceSpend"))
            {
                return true;
            }

            var distance = Menu.NearDeathEnemyDistance;

            return (Hero.Health <= Menu.NearDeathHpThreshold
                    || (float)Hero.Health / Hero.MaximumHealth * 100 <= Menu.NearDeathHpPercentageThreshold)
                   && (distance <= 0
                       || ObjectManager.GetEntitiesParallel<Hero>()
                           .Any(
                               x =>
                                   x.IsValid && x.Team == enemyTeam && !x.IsIllusion && x.IsAlive
                                   && x.Distance2D(Hero) <= distance));
        }

        #endregion

        #region Methods

        private uint GetItemCount(AbilityId id)
        {
            return (Hero.Inventory.Items.FirstOrDefault(x => x.AbilityId == id)?.CurrentCharges ?? 0)
                   + (Hero.Inventory.Stash.FirstOrDefault(x => x.AbilityId == id)?.CurrentCharges ?? 0)
                   + (Hero.Inventory.Backpack.FirstOrDefault(x => x.AbilityId == id)?.CurrentCharges ?? 0);
        }

        private uint GetWardsCount(AbilityId id)
        {
            var inventoryDispenser = Hero.Inventory.Items.FirstOrDefault(x => x.ID == 218);
            var stashDispenser = Hero.Inventory.Stash.FirstOrDefault(x => x.ID == 218);
            var backpackDispenser = Hero.Inventory.Backpack.FirstOrDefault(x => x.ID == 218);

            return GetItemCount(id)
                   + (id == AbilityId.item_ward_observer
                          ? (inventoryDispenser?.CurrentCharges ?? 0) + (stashDispenser?.CurrentCharges ?? 0)
                            + (backpackDispenser?.CurrentCharges ?? 0)
                          : (inventoryDispenser?.SecondaryCharges ?? 0) + (stashDispenser?.SecondaryCharges ?? 0)
                            + (backpackDispenser?.SecondaryCharges ?? 0));
        }

        #endregion
    }
}