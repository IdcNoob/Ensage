namespace ItemManager.Core.Modules.RecoveryAbuse
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abilities;
    using Abilities.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using EventArgs;

    using Menus.Modules.AbilityHelper;
    using Menus.Modules.Recovery;

    using Utils;

    internal class RecoveryAbuse : IDisposable
    {
        private readonly Manager manager;

        private readonly RecoveryMenu menu;

        private readonly MultiSleeper sleeper = new MultiSleeper();

        private readonly Tranquil tranquilDropMenu;

        private bool usingItems;

        public RecoveryAbuse(Manager manager, RecoveryMenu menu, Tranquil tranquilDropMenu)
        {
            this.manager = manager;
            this.menu = menu;
            this.tranquilDropMenu = tranquilDropMenu;

            Game.OnUpdate += OnUpdate;
            Player.OnExecuteOrder += OnExecuteOrder;

            menu.OnAbuseChange += OnAbuseChange;
        }

        public void Dispose()
        {
            Game.OnUpdate -= OnUpdate;
            Player.OnExecuteOrder -= OnExecuteOrder;

            menu.OnAbuseChange -= OnAbuseChange;
        }

        private void OnAbuseChange(object sender, BoolEventArgs boolEventArgs)
        {
            if (boolEventArgs.Enabled)
            {
                manager.DroppedItems.RemoveAll(
                    x => x == null || !x.IsValid || manager
                             .GetMyItems(ItemUtils.StoredPlace.Inventory | ItemUtils.StoredPlace.Backpack)
                             .Contains(x));
            }
            else if (usingItems && !menu.ItemsToBackpack)
            {
                sleeper.Sleep(manager.DroppedItems.Count * 100, "blockEarlyKeyRelease");
            }

            sleeper.Sleep(0, this);
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!args.Entities.Contains(manager.MyHero) || !menu.IsEnabled || !args.IsPlayerInput)
            {
                return;
            }

            if (menu.IsActive || (sleeper.Sleeping("pickingItems") || sleeper.Sleeping("blockEarlyKeyRelease"))
                && args.OrderId != OrderId.PickItem && args.OrderId != OrderId.MoveItem)
            {
                args.Process = false;
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping(this) || Game.IsPaused || !menu.IsEnabled)
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
                var recoveryAbilities = manager.UsableAbilities.OfType<IRecoveryAbility>()
                    .Where(x => menu.IsAbilityEnabled(x.Name))
                    .OrderByDescending(x => menu.GetAbilityPriority(x.Name))
                    .ToList();
                var usableRecoveryAbilities = recoveryAbilities.Where(x => x.CanBeCasted()).ToList();

                if (usableRecoveryAbilities.Any(x => x.CanBeCasted())
                    && (manager.MyHero.Mana < manager.MyHero.MaximumMana
                        || manager.MyHero.Health < manager.MyHero.MaximumHealth))
                {
                    usingItems = true;
                    manager.MyHero.Stop();

                    var mpRestore = usableRecoveryAbilities.Sum(x => x.ManaRestore);
                    var hpRestore = usableRecoveryAbilities.Sum(x => x.HealthRestore);

                    var missingHp = manager.MyMissingHealth;
                    var missingMp = manager.MyMissingMana;

                    var powerTreads =
                        manager.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId.item_power_treads) as PowerTreads;

                    var usedAbilities = new List<uint>();

                    foreach (var ability in usableRecoveryAbilities)
                    {
                        if (missingMp <= 0 && missingHp <= 0)
                        {
                            sleeper.Sleep(500, this);
                            menu.ForceDisable();
                            usingItems = false;
                            break;
                        }

                        switch (ability.Id)
                        {
                            case AbilityId.item_soul_ring:
                            {
                                if (missingMp < menu.ItemSettingsMenu.SoulRing.MpThreshold || manager.MyHealthPercentage
                                    < menu.ItemSettingsMenu.SoulRing.HpThreshold)
                                {
                                    continue;
                                }
                                break;
                            }
                            case AbilityId.item_bottle:
                            {
                                if (menu.ItemSettingsMenu.BottleSettings.OverhealEnabled)
                                {
                                    if (missingMp < menu.ItemSettingsMenu.BottleSettings.MpThreshold && missingHp
                                        < menu.ItemSettingsMenu.BottleSettings.HpThreshold)
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (missingMp < menu.ItemSettingsMenu.BottleSettings.MpThreshold || missingHp
                                        < menu.ItemSettingsMenu.BottleSettings.HpThreshold)
                                    {
                                        continue;
                                    }
                                }
                                break;
                            }
                            case AbilityId.item_arcane_boots:
                            {
                                if (missingMp < menu.ItemSettingsMenu.ArcaneBootsSettings.MpThreshold)
                                {
                                    continue;
                                }
                                break;
                            }
                            case AbilityId.item_guardian_greaves:
                            {
                                if (missingMp < menu.ItemSettingsMenu.GuardianGreaves.MpThreshold && missingHp
                                    < menu.ItemSettingsMenu.GuardianGreaves.HpThreshold)
                                {
                                    continue;
                                }
                                break;
                            }
                            case AbilityId.item_magic_wand:
                            case AbilityId.item_magic_stick:
                            {
                                if (missingMp < menu.ItemSettingsMenu.MagicStick.MpThreshold
                                    && missingHp < menu.ItemSettingsMenu.MagicStick.HpThreshold)
                                {
                                    continue;
                                }
                                break;
                            }
                            case AbilityId.item_mekansm:
                            {
                                if (missingHp < menu.ItemSettingsMenu.Mekansm.HpThreshold)
                                {
                                    continue;
                                }
                                break;
                            }
                            case AbilityId.item_urn_of_shadows:
                            {
                                if (missingHp < menu.ItemSettingsMenu.UrnOfShadows.HpThreshold)
                                {
                                    continue;
                                }
                                break;
                            }
                        }

                        if (menu.ItemSettingsMenu.PowerTreads.IsEnabled && powerTreads != null
                            && powerTreads.CanBeCasted())
                        {
                            powerTreads.SwitchTo(ability.PowerTreadsAttribute, true);
                        }

                        switch (ability.ItemRestoredStats)
                        {
                            case ItemUtils.Stats.Mana:
                            {
                                if (mpRestore < missingMp)
                                {
                                    manager.DropItems(
                                        ItemUtils.Stats.Mana,
                                        menu.ItemsToBackpack,
                                        usableRecoveryAbilities.Where(x => usedAbilities.All(z => z != x.Handle))
                                            .ToArray());
                                }

                                missingMp -= ability.ManaRestore;
                                break;
                            }
                            case ItemUtils.Stats.Health:
                            {
                                if (hpRestore < missingHp)
                                {
                                    manager.DropItems(
                                        ItemUtils.Stats.Health,
                                        menu.ItemsToBackpack,
                                        usableRecoveryAbilities.Where(x => usedAbilities.All(z => z != x.Handle))
                                            .ToArray());
                                }

                                missingHp -= ability.HealthRestore;
                                break;
                            }
                            case ItemUtils.Stats.All:
                            {
                                if (mpRestore < missingMp)
                                {
                                    manager.DropItems(
                                        ItemUtils.Stats.Mana,
                                        menu.ItemsToBackpack,
                                        usableRecoveryAbilities.Where(x => usedAbilities.All(z => z != x.Handle))
                                            .ToArray());
                                }
                                if (hpRestore < missingHp)
                                {
                                    manager.DropItems(
                                        ItemUtils.Stats.Health,
                                        menu.ItemsToBackpack,
                                        usableRecoveryAbilities.Where(x => usedAbilities.All(z => z != x.Handle))
                                            .ToArray());
                                }

                                missingHp -= ability.HealthRestore;
                                missingMp -= ability.ManaRestore;
                                break;
                            }
                        }

                        ability.Use(true);
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
                var sleepTime = manager.PickUpItems();
                if (sleepTime > 0)
                {
                    sleeper.Sleep(sleepTime, "pickingItems");
                }
            }
        }

        private bool ShouldPickUpItems(IEnumerable<IRecoveryAbility> usableAbilities)
        {
            if (ObjectManager.GetEntitiesParallel<Hero>()
                .Any(
                    x => x.IsValid && x.IsVisible && x.IsAlive && x.Team != manager.MyTeam
                         && x.Distance2D(manager.MyHero) < 800))
            {
                return true;
            }

            return usableAbilities.All(x => !x.IsSleeping)
                   && !manager.MyHero.HasModifier(ModifierUtils.BottleRegeneration);
        }
    }
}