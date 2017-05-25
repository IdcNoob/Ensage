namespace ItemManager.Core.Modules.GoldSpender
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Attributes;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;
    using Ensage.SDK.Helpers;

    using EventArgs;

    using Menus;
    using Menus.Modules.GoldSpender;

    using Utils;

    [Module]
    internal class GoldSpender : IDisposable
    {
        private readonly List<Courier> couriers = new List<Courier>();

        private readonly List<Ability> invisAbilities = new List<Ability>();

        private readonly Manager manager;

        private readonly GoldSpenderMenu menu;

        private readonly Sleeper sleeper = new Sleeper();

        public GoldSpender(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.GoldSpenderMenu;

            UpdateManager.Subscribe(OnUpdate, 100);
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

                if (item.IsPurchasable(manager.MyHero.Hero))
                {
                    unit = manager.MyHero.Hero;
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
                        if (manager.MyHero.GetItemCharges(item) >= 2 || !invisAbilities.Any())
                        {
                            continue;
                        }
                        break;
                    case AbilityId.item_ward_observer:
                    case AbilityId.item_ward_sentry:
                        if (manager.MyHero.GetItemCharges(item) >= 2)
                        {
                            continue;
                        }
                        break;
                    case AbilityId.item_tpscroll:
                        if (manager.MyHero.GetItemCharges(item) >= 2 || manager.MyHero.Items.Any(
                                x => x.Id == AbilityId.item_travel_boots || x.Id == AbilityId.item_travel_boots_2))
                        {
                            continue;
                        }
                        break;
                    case AbilityId.item_energy_booster:
                        if (manager.MyHero.Items.Any(x => x.Id == AbilityId.item_arcane_boots))
                        {
                            continue;
                        }
                        break;
                    case AbilityId.item_smoke_of_deceit:
                        if (manager.MyHero.GetItemCharges(item) >= 1)
                        {
                            continue;
                        }
                        break;
                    case AbilityId.item_tome_of_knowledge:
                        if (manager.MyHero.Hero.Level >= 25)
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

            if (!itemsToBuy.Any() || unreliableGold > GoldLossOnDeath() || !manager.MyHero.IsAlive)
            {
                return;
            }

            itemsToBuy.ForEach(x => Player.BuyItem(x.Item1, x.Item2));
            sleeper.Sleep(20000);
        }

        public void Dispose()
        {
            UpdateManager.Unsubscribe(OnUpdate);
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
                >= manager.MyHero.RespawnTime)
            {
                return;
            }

            var requiredGold = manager.MyHero.BuybackCost + GoldLossOnDeath();

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

        private int GoldLossOnDeath()
        {
            return Math.Min(
                manager.MyHero.Player.UnreliableGold,
                50 + (manager.MyHero.Player.ReliableGold + manager.MyHero.Player.UnreliableGold
                      + manager.MyHero.Items.Sum(x => (int)x.Cost)) / 40);
        }

        private void OnAbilityAdd(object sender, AbilityEventArgs abilityEventArgs)
        {
            if (!abilityEventArgs.Ability.IsInvis())
            {
                return;
            }

            var item = abilityEventArgs.Ability as Item;
            if (item != null && item.Purchaser.Team != manager.MyHero.Team)
            {
                invisAbilities.Add(item);
                return;
            }

            var ability = abilityEventArgs.Ability;
            if (ability.Owner?.Team != manager.MyHero.Team)
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
            if (courier != null && courier.Team == manager.MyHero.Team)
            {
                couriers.Add(courier);
            }
        }

        private void OnUpdate()
        {
            if (sleeper.Sleeping || !menu.SpendGold || Game.IsPaused || !manager.MyHero.IsAlive)
            {
                return;
            }

            if (!Utils.SleepCheck("GoldSpender.ForceSpend"))
            {
                BuyItems();
                return;
            }

            if (manager.MyHero.Health > menu.HpThreshold && manager.MyHero.HealthPercentage > menu.HpThresholdPct)
            {
                return;
            }

            var distance = menu.EnemyDistance;
            if (distance <= 0 || ObjectManager.GetEntitiesParallel<Hero>()
                    .Any(
                        x => x.IsValid && x.Team == manager.MyHero.EnemyTeam && !x.IsIllusion && x.IsAlive
                             && x.Distance2D(manager.MyHero.Position) <= distance))
            {
                BuyItems();
            }
        }
    }
}