namespace ItemManager.Core.Modules.RecoveryAbuse
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abilities;
    using Abilities.Interfaces;

    using Attributes;

    using Ensage;
    using Ensage.Common.Objects.UtilityObjects;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;

    using EventArgs;

    using Menus;
    using Menus.Modules.AbilityHelper;
    using Menus.Modules.Recovery;

    using Utils;

   // [Module]
    internal class RecoveryAbuse : IDisposable
    {
        private readonly Manager manager;

        private readonly RecoveryMenu menu;

        private readonly MultiSleeper sleeper = new MultiSleeper();

        private readonly TranquilMenu tranquilDropMenu;

        private readonly IUpdateHandler updateHandler;

        private bool usingItems;

        public RecoveryAbuse(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.RecoveryMenu;
            tranquilDropMenu = menu.AbilityHelperMenu.TranquilMenu;

            updateHandler = UpdateManager.Subscribe(OnUpdate, 0, this.menu.IsEnabled);
            if (this.menu.IsEnabled)
            {
                Player.OnExecuteOrder += OnExecuteOrder;
                this.menu.OnAbuseChange += OnAbuseChange;
            }
            this.menu.OnEnabledChange += MenuOnEnabledChange;
        }

        public void Dispose()
        {
            menu.OnEnabledChange -= MenuOnEnabledChange;
            UpdateManager.Unsubscribe(OnUpdate);
            Player.OnExecuteOrder -= OnExecuteOrder;
            menu.OnAbuseChange -= OnAbuseChange;
        }

        private void MenuOnEnabledChange(object sender, BoolEventArgs boolEventArgs)
        {
            if (boolEventArgs.Enabled)
            {
                Player.OnExecuteOrder += OnExecuteOrder;
                menu.OnAbuseChange += OnAbuseChange;
                updateHandler.IsEnabled = true;
            }
            else
            {
                Player.OnExecuteOrder -= OnExecuteOrder;
                menu.OnAbuseChange -= OnAbuseChange;
                updateHandler.IsEnabled = false;
            }
        }

        private void OnAbuseChange(object sender, BoolEventArgs boolEventArgs)
        {
            if (boolEventArgs.Enabled)
            {
                manager.MyHero.DroppedItems.RemoveAll(
                    x => x == null || !x.IsValid
                         || manager.MyHero.GetItems(ItemStoredPlace.Inventory | ItemStoredPlace.Backpack).Contains(x));
            }
            else if (usingItems && !menu.ItemsToBackpack)
            {
                sleeper.Sleep(manager.MyHero.DroppedItems.Count * 100, "blockEarlyKeyRelease");
            }

            sleeper.Sleep(0, this);
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!args.Entities.Contains(manager.MyHero.Hero) || !args.IsPlayerInput)
            {
                return;
            }

            if (menu.IsActive || (sleeper.Sleeping("pickingItems") || sleeper.Sleeping("blockEarlyKeyRelease"))
                && args.OrderId != OrderId.PickItem && args.OrderId != OrderId.MoveItem)
            {
                args.Process = false;
            }
        }

        private void OnUpdate()
        {
            if (sleeper.Sleeping(this) || Game.IsPaused)
            {
                return;
            }

            sleeper.Sleep(300, this);

            if (!manager.MyHero.IsAlive)
            {
                return;
            }

            if (menu.IsActive)
            {
                var recoveryAbilities = manager.MyHero.UsableAbilities.OfType<IRecoveryAbility>()
                    .Where(x => menu.IsAbilityEnabled(x.Name))
                    .OrderByDescending(x => menu.GetAbilityPriority(x.Name))
                    .ToList();

                var usableRecoveryAbilities = recoveryAbilities.Where(x => x.CanBeCasted()).ToList();

                if (usableRecoveryAbilities.Any() && (manager.MyHero.Mana < manager.MyHero.MaximumMana
                                                      || manager.MyHero.Health < manager.MyHero.MaximumHealth))
                {
                    usingItems = true;
                    manager.MyHero.Hero.Stop();

                    var totalManaRestore = usableRecoveryAbilities.Sum(x => x.ManaRestore);
                    var totalHealthRestore = usableRecoveryAbilities.Sum(x => x.HealthRestore);

                    var missingHealth = manager.MyHero.MissingHealth;
                    var missingMana = manager.MyHero.MissingMana;

                    var powerTreads =
                        manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId.item_power_treads) as PowerTreads;

                    var usedAbilities = new List<uint>();

                    foreach (var ability in usableRecoveryAbilities)
                    {
                        if (missingMana <= 0 && missingHealth <= 0)
                        {
                            sleeper.Sleep(500, this);
                            menu.ForceDisable();
                            usingItems = false;
                            break;
                        }

                        if (!ability.ShouldBeUsed(manager.MyHero, menu, missingHealth, missingMana))
                        {
                            continue;
                        }

                        if (menu.ItemSettingsMenu.PowerTreads.IsEnabled && powerTreads != null && powerTreads.CanBeCasted())
                        {
                            powerTreads.SwitchTo(ability.PowerTreadsAttribute, true);
                        }

                        switch (ability.RestoredStats)
                        {
                            case RestoredStats.Mana:
                            {
                                if (totalManaRestore < missingMana)
                                {
                                    manager.MyHero.DropItems(
                                        ItemStats.Mana,
                                        menu.ItemsToBackpack,
                                        usableRecoveryAbilities.Where(x => usedAbilities.All(z => z != x.Handle)).ToArray());
                                }

                                missingMana -= ability.ManaRestore;
                                break;
                            }
                            case RestoredStats.Health:
                            {
                                if (totalHealthRestore < missingHealth)
                                {
                                    manager.MyHero.DropItems(
                                        ItemStats.Health,
                                        menu.ItemsToBackpack,
                                        usableRecoveryAbilities.Where(x => usedAbilities.All(z => z != x.Handle)).ToArray());
                                }

                                missingHealth -= ability.HealthRestore;
                                break;
                            }
                            case RestoredStats.All:
                            {
                                if (totalManaRestore < missingMana)
                                {
                                    manager.MyHero.DropItems(
                                        ItemStats.Mana,
                                        menu.ItemsToBackpack,
                                        usableRecoveryAbilities.Where(x => usedAbilities.All(z => z != x.Handle)).ToArray());
                                }
                                if (totalHealthRestore < missingHealth)
                                {
                                    manager.MyHero.DropItems(
                                        ItemStats.Health,
                                        menu.ItemsToBackpack,
                                        usableRecoveryAbilities.Where(x => usedAbilities.All(z => z != x.Handle)).ToArray());
                                }

                                missingHealth -= ability.HealthRestore;
                                missingMana -= ability.ManaRestore;
                                break;
                            }
                        }

                        ability.Use(queue: true);
                        usedAbilities.Add(ability.Handle);
                    }
                }
                else if (ShouldPickUpItems(recoveryAbilities))
                {
                    menu.ForceDisable();
                    PickItems();
                }
            }
            else
            {
                PickItems();
            }
        }

        private void PickItems()
        {
            if (menu.ItemsToBackpack && manager.MyHero.ItemsCanBeDisabled() || tranquilDropMenu.IsActive)
            {
                return;
            }

            usingItems = false;

            if (!sleeper.Sleeping("pickingItems"))
            {
                var sleepTime = manager.MyHero.PickUpItems();
                if (sleepTime > 0)
                {
                    sleeper.Sleep(sleepTime, "pickingItems");
                }
            }
        }

        private bool ShouldPickUpItems(IEnumerable<IRecoveryAbility> usableAbilities)
        {
            if (EntityManager<Hero>.Entities.Any(
                x => x.IsValid && x.IsVisible && x.IsAlive && x.Team != manager.MyHero.Team && x.Distance2D(manager.MyHero.Position) < 800))
            {
                return true;
            }

            return usableAbilities.All(x => !x.IsSleeping) && !manager.MyHero.Hero.HasAnyModifiers(
                       ModifierUtils.BottleRegeneration,
                       ModifierUtils.UrnRegeneration,
                       ModifierUtils.SpiritVesselRegeneration);
        }
    }
}