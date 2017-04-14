namespace ItemManager.Core.Modules.GoldSpender
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using EventArgs;

    using Menus.Modules.GoldSpender;

    using Utils;

    internal class GoldSpender : IDisposable
    {
        private readonly List<Courier> couriers = new List<Courier>();

        private readonly Team enemyTeam;

        private readonly List<Ability> invisAbilities = new List<Ability>();

        private readonly Manager manager;

        private readonly GoldSpenderMenu menu;

        private readonly Sleeper sleeper = new Sleeper();

        public GoldSpender(Manager manager, GoldSpenderMenu menu)
        {
            this.menu = menu;
            this.manager = manager;
            enemyTeam = manager.MyHero.GetEnemyTeam();

            Game.OnUpdate += OnUpdate;

            manager.OnUnitAdd += OnUnitAdd;
            manager.OnAbilityAdd += OnAbilityAdd;
            manager.OnAbilityRemove += OnAbilityRemove;
        }

        public void BuyItems()
        {
            var enabledItems = menu.ItemsToBuy.OrderByDescending(x => menu.GetAbilityPriority(x.Key))
                .Where(x => menu.IsAbilityEnabled(x.Key))
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

            var courier = couriers.FirstOrDefault(x => x.IsValid && x.IsAlive);

            foreach (var item in enabledItems)
            {
                Unit unit;

                if (item.IsPurchasable(manager.MyHero))
                {
                    unit = manager.MyHero;
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
                        if (manager.GetItemCharges(item) >= 2 || !invisAbilities.Any())
                        {
                            continue;
                        }
                        break;
                    case AbilityId.item_ward_observer:
                    case AbilityId.item_ward_sentry:
                        if (manager.GetItemCharges(item) >= 2)
                        {
                            continue;
                        }
                        break;
                    case AbilityId.item_tpscroll:
                        if (manager.GetItemCharges(item) >= 2 || manager.MyItems.Any(
                                x => x.Id == AbilityId.item_travel_boots || x.Id == AbilityId.item_travel_boots_2))
                        {
                            continue;
                        }
                        break;
                    case AbilityId.item_energy_booster:
                        if (manager.MyItems.Any(x => x.Id == AbilityId.item_arcane_boots))
                        {
                            continue;
                        }
                        break;
                    case AbilityId.item_smoke_of_deceit:
                        if (manager.GetItemCharges(item) >= 1)
                        {
                            continue;
                        }
                        break;
                    case AbilityId.item_tome_of_knowledge:
                        if (manager.MyHero.Level >= 25)
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

            if (!itemsToBuy.Any() || unreliableGold > manager.MyHero.GoldLossOnDeath(manager)
                || !manager.MyHero.IsAlive)
            {
                return;
            }

            itemsToBuy.ForEach(x => Player.BuyItem(x.Item1, x.Item2));
            sleeper.Sleep(20000);
        }

        public void Dispose()
        {
            Game.OnUpdate -= OnUpdate;

            manager.OnUnitAdd -= OnUnitAdd;
            manager.OnAbilityAdd -= OnAbilityAdd;
            manager.OnAbilityRemove -= OnAbilityRemove;

            couriers.Clear();
            invisAbilities.Clear();
        }

        private void GetAvailableGold(float time, out int reliable, out int unreliable)
        {
            reliable = manager.MyHero.Player.ReliableGold;
            unreliable = manager.MyHero.Player.UnreliableGold;

            if (time <= 0 || Game.GameTime / 60 < time || manager.MyHero.Player.BuybackCooldownTime
                >= manager.MyHero.RespawnTime())
            {
                return;
            }

            var requiredGold = manager.MyHero.BuybackCost() + manager.MyHero.GoldLossOnDeath(manager);

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

        private void OnAbilityAdd(object sender, AbilityEventArgs abilityEventArgs)
        {
            if (!abilityEventArgs.Ability.IsInvis())
            {
                return;
            }

            var item = abilityEventArgs.Ability as Item;
            if (item != null && item.Purchaser.Team != manager.MyTeam)
            {
                invisAbilities.Add(item);
                return;
            }

            var ability = abilityEventArgs.Ability;
            if (ability.Owner?.Team != manager.MyTeam)
            {
                invisAbilities.Add(item);
            }
        }

        private void OnAbilityRemove(object sender, AbilityEventArgs abilityEventArgs)
        {
            invisAbilities.Remove(abilityEventArgs.Ability);
        }

        private void OnUnitAdd(object sender, UnitEventArgs unitEventArgs)
        {
            var courier = unitEventArgs.Unit as Courier;
            if (courier != null && courier.Team == manager.MyTeam)
            {
                couriers.Add(courier);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping || !menu.SpendGold || manager.MyHero.GoldLossOnDeath(manager) <= 20
                || !manager.MyHero.IsAlive || Game.IsPaused)
            {
                return;
            }

            sleeper.Sleep(100);

            if (!Utils.SleepCheck("GoldSpender.ForceSpend"))
            {
                BuyItems();
                return;
            }

            if (manager.MyHero.Health <= menu.HpThreshold
                || (float)manager.MyHero.Health / manager.MyHero.MaximumHealth * 100 <= menu.HpThresholdPct)
            {
                var distance = menu.EnemyDistance;

                if (distance <= 0 || ObjectManager.GetEntitiesParallel<Hero>()
                        .Any(
                            x => x.IsValid && x.Team == enemyTeam && !x.IsIllusion && x.IsAlive
                                 && x.Distance2D(manager.MyHero) <= distance))
                {
                    BuyItems();
                }
            }
        }
    }
}