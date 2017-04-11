namespace ItemManager.Core.Modules.GoldSpender
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using Menus.Modules.GoldSpender;

    using Utils;

    internal class GoldSpender
    {
        private readonly Team enemyTeam;

        private readonly Hero hero;

        private readonly ItemManager items;

        private readonly GoldSpenderMenu menu;

        private readonly Sleeper sleeper = new Sleeper();

        public GoldSpender(Hero myHero, ItemManager itemManager, GoldSpenderMenu goldSpenderMenu)
        {
            hero = myHero;
            menu = goldSpenderMenu;
            items = itemManager;
            enemyTeam = hero.GetEnemyTeam();

            Game.OnUpdate += OnUpdate;
        }

        public void BuyItems()
        {
            var enabledItems = menu.ItemsToBuy.OrderByDescending(x => menu.ItemPriority(x.Key))
                .Where(x => menu.ItemEnabled(x.Key))
                .Select(x => x.Value)
                .ToList();

            var quickBuy = enabledItems.FindIndex(x => x == 0);
            if (quickBuy >= 0)
            {
                enabledItems.RemoveAt(quickBuy);
                enabledItems.InsertRange(
                    quickBuy,
                    Player.QuickBuyItems.OrderByDescending(x => Ability.GetAbilityDataById(x).Cost));
            }

            var itemsToBuy = new List<Tuple<Unit, AbilityId>>();

            int unreliableGold, reliableGold;
            GetAvailableGold(menu.SaveForBuyback, out reliableGold, out unreliableGold);

            var courier = ObjectManager.GetEntitiesParallel<Courier>()
                .FirstOrDefault(x => x.Team == hero.Team && x.IsAlive);

            foreach (var item in enabledItems)
            {
                Unit unit;

                if (item.IsPurchasable(hero))
                {
                    unit = hero;
                }
                else if (item.IsPurchasable(courier))
                {
                    unit = courier;
                }
                else
                {
                    continue;
                }

                switch (item)
                {
                    case AbilityId.item_dust:
                        if (items.GetItemCharges(item) >= 2 || !ObjectManager.GetEntitiesParallel<Ability>()
                                .Any(x => x.IsValid && x.Owner?.Team == enemyTeam && x.IsInvis()))
                        {
                            continue;
                        }
                        break;
                    case AbilityId.item_ward_observer:
                    case AbilityId.item_ward_sentry:
                        if (items.GetItemCharges(item) >= 2)
                        {
                            continue;
                        }
                        break;
                    case AbilityId.item_tpscroll:
                        if (items.GetItemCharges(item) >= 2 || items.AllItems.Any(
                                x => x.Id == AbilityId.item_travel_boots || x.Id == AbilityId.item_travel_boots_2))
                        {
                            continue;
                        }
                        break;
                    case AbilityId.item_energy_booster:
                        if (items.AllItems.Any(x => x.Id == AbilityId.item_arcane_boots))
                        {
                            continue;
                        }
                        break;
                    case AbilityId.item_smoke_of_deceit:
                        if (items.GetItemCharges(item) >= 1)
                        {
                            continue;
                        }
                        break;
                    case AbilityId.item_tome_of_knowledge:
                        if (hero.Level >= 25)
                        {
                            continue;
                        }
                        break;
                }

                var cost = Ability.GetAbilityDataById(item).Cost;

                if (unreliableGold >= cost)
                {
                    itemsToBuy.Add(Tuple.Create(unit, item));
                    unreliableGold -= (int)cost;
                }
                else if (reliableGold + unreliableGold >= cost)
                {
                    itemsToBuy.Add(Tuple.Create(unit, item));
                    break;
                }
            }

            if (!itemsToBuy.Any() || unreliableGold > hero.GoldLossOnDeath(items) || !hero.IsAlive)
            {
                return;
            }

            itemsToBuy.ForEach(x => Player.BuyItem(x.Item1, x.Item2));
            sleeper.Sleep(20000);
        }

        public void OnClose()
        {
            Game.OnUpdate -= OnUpdate;
        }

        private void GetAvailableGold(float time, out int reliable, out int unreliable)
        {
            reliable = hero.Player.ReliableGold;
            unreliable = hero.Player.UnreliableGold;

            if (time <= 0 || Game.GameTime / 60 < time || hero.Player.BuybackCooldownTime >= hero.RespawnTime())
            {
                return;
            }

            var requiredGold = hero.BuybackCost() + hero.GoldLossOnDeath(items);

            if (unreliable + reliable >= requiredGold)
            {
                if (requiredGold - reliable > 0)
                {
                    requiredGold -= reliable;
                    if (requiredGold - unreliable <= 0)
                    {
                        reliable = 0;
                        unreliable -= requiredGold;
                    }
                }
                else
                {
                    reliable -= requiredGold;
                }
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping || !menu.SpendGold || hero.GoldLossOnDeath(items) <= 20 || !hero.IsAlive)
            {
                return;
            }

            sleeper.Sleep(100);

            if (!Utils.SleepCheck("GoldSpender.ForceSpend"))
            {
                BuyItems();
                return;
            }

            if (hero.Health <= menu.HpThreshold || (float)hero.Health / hero.MaximumHealth * 100 <= menu.HpThresholdPct)
            {
                var distance = menu.EnemyDistance;

                if (distance <= 0 || ObjectManager.GetEntitiesParallel<Hero>()
                        .Any(
                            x => x.IsValid && x.Team == enemyTeam && !x.IsIllusion && x.IsAlive
                                 && x.Distance2D(hero) <= distance))
                {
                    BuyItems();
                }
            }
        }
    }
}