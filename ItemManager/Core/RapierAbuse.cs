namespace ItemManager.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Args;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Enums;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using Menus.RapierAbuse;

    internal class RapierAbuse
    {
        #region Fields

        private readonly Sleeper delayDisassemble = new Sleeper();

        private readonly string[] disableModifiers =
        {
            "modifier_shadow_demon_disruption",
            "modifier_obsidian_destroyer_astral_imprisonment_prison",
            "modifier_eul_cyclone",
            "modifier_invoker_tornado",
            "modifier_sheepstick_debuff",
            "modifier_shadow_shaman_voodoo",
            "modifier_lion_voodoo",
            "modifier_brewmaster_storm_cyclone",
            "modifier_invoker_deafening_blast_knockback",
            "modifier_heavens_halberd_debuff"
        };

        private readonly List<AbilityId> enabledAbilities = new List<AbilityId>
        {
            AbilityId.tusk_walrus_punch,
            AbilityId.ember_spirit_sleight_of_fist,
            AbilityId.kunkka_tidebringer,
            AbilityId.sniper_assassinate,
            AbilityId.monkey_king_boundless_strike,
            AbilityId.phantom_assassin_stifling_dagger
        };

        private readonly Sleeper evaderCheck = new Sleeper();

        private readonly Hero hero;

        private readonly Items items;

        private readonly RapierAbuseMenu menu;

        private readonly List<ItemId> requiredItems = new List<ItemId>
        {
            ItemId.item_demon_edge,
            ItemId.item_relic
        };

        private CancellationTokenSource cts = new CancellationTokenSource();

        private bool manualModeEnabled;

        #endregion

        #region Constructors and Destructors

        public RapierAbuse(Hero myHero, Items myItems, RapierAbuseMenu rapierAbuseMenu)
        {
            hero = myHero;
            items = myItems;
            menu = rapierAbuseMenu;

            var currentItems = items.GetMyItems(Items.StoredPlace.All).Select(x => (ItemId)x.ID).ToList();
            if (currentItems.Contains(ItemId.item_rapier) || requiredItems.All(x => currentItems.Contains(x)))
            {
                Player.OnExecuteOrder += OnExecuteOrder;
                Entity.OnInt32PropertyChange += OnInt32PropertyChange;
                Unit.OnModifierAdded += OnModifierAdded;
                Game.OnUpdate += OnUpdate;
                menu.OnManualRapierAbuse += OnManualRapierAbuse;
            }
            else
            {
                items.OnItemChange += OnItemChange;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            Player.OnExecuteOrder -= OnExecuteOrder;
            Entity.OnInt32PropertyChange -= OnInt32PropertyChange;
            Unit.OnModifierAdded -= OnModifierAdded;
            Game.OnUpdate -= OnUpdate;
            items.OnItemChange -= OnItemChange;
            menu.OnManualRapierAbuse -= OnManualRapierAbuse;
        }

        #endregion

        #region Methods

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!args.Entities.Contains(hero) || !menu.AbuseEnabled || delayDisassemble.Sleeping || manualModeEnabled)
            {
                return;
            }

            switch (args.Order)
            {
                case Order.AttackLocation:
                case Order.AttackTarget:
                    if (hero.Health > menu.HpThreshold)
                    {
                        items.UnlockCombining(requiredItems);
                        if (menu.AttackFix)
                        {
                            var target = args.Target as Unit;
                            if (target != null)
                            {
                                args.Process = false;
                                hero.Attack(target);
                            }
                        }

                        if (cts.Token.IsCancellationRequested)
                        {
                            cts = new CancellationTokenSource();
                            DelayAction.Add(8000, TimeCheck, cts.Token);
                        }
                    }
                    break;
                case Order.AbilityTarget:
                case Order.AbilityLocation:
                    if (!enabledAbilities.Contains((AbilityId)args.Ability.ID) || hero.Health < menu.HpThreshold)
                    {
                        items.Disassemble(ItemId.item_rapier);
                        return;
                    }

                    items.UnlockCombining(requiredItems);

                    var delay = (float)args.Ability.FindCastPoint() + Game.Ping + 500;
                    var speed = args.Ability.GetProjectileSpeed();

                    if (speed > 0)
                    {
                        delay += hero.Distance2D(args.Target) / speed * 1000;
                        delayDisassemble.Sleep(delay);
                    }

                    var distance =
                        Math.Max(
                            (args.Target?.Position ?? args.TargetPosition).Distance2D(hero)
                            - args.Ability.GetCastRange(),
                            0) / hero.MovementSpeed * 1000;

                    cts.Cancel();
                    cts = new CancellationTokenSource();
                    DelayAction.Add(delay + distance, () => items.Disassemble(ItemId.item_rapier), cts.Token);
                    break;
                default:
                    if (!cts.Token.IsCancellationRequested)
                    {
                        cts.Cancel();
                    }

                    items.Disassemble(ItemId.item_rapier);
                    delayDisassemble.Sleep(Game.Ping);
                    break;
            }
        }

        private void OnInt32PropertyChange(Entity sender, Int32PropertyChangeEventArgs args)
        {
            if (!menu.AbuseEnabled || args.OldValue == args.NewValue || sender?.Handle != hero.Handle
                || args.PropertyName != "m_iHealth")
            {
                return;
            }

            if (args.NewValue <= menu.HpThreshold)
            {
                items.Disassemble(ItemId.item_rapier);
            }
        }

        private void OnItemChange(object sender, ItemEventArgs itemEventArgs)
        {
            if (itemEventArgs.ItemId == ItemId.item_rapier && itemEventArgs.Bought)
            {
                Player.OnExecuteOrder += OnExecuteOrder;
                Entity.OnInt32PropertyChange += OnInt32PropertyChange;
                Unit.OnModifierAdded += OnModifierAdded;
                Game.OnUpdate += OnUpdate;
                menu.OnManualRapierAbuse += OnManualRapierAbuse;
                items.OnItemChange -= OnItemChange;
            }
        }

        private void OnManualRapierAbuse(object sender, BoolEventArgs boolEventArgs)
        {
            if (boolEventArgs.Enabled && hero.Health > menu.HpThreshold)
            {
                manualModeEnabled = true;
                items.UnlockCombining(requiredItems);
                cts = new CancellationTokenSource();
                DelayAction.Add(8000, TimeCheck, cts.Token);
            }
            else
            {
                manualModeEnabled = false;
                items.Disassemble(ItemId.item_rapier);
            }
        }

        private void OnModifierAdded(Unit sender, ModifierChangedEventArgs args)
        {
            if (!menu.AbuseEnabled || sender?.Handle != hero.Handle)
            {
                return;
            }

            var modifier = args.Modifier;
            if (modifier == null || !modifier.IsValid)
            {
                return;
            }

            if (disableModifiers.Contains(modifier.Name))
            {
                items.Disassemble(ItemId.item_rapier);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (evaderCheck.Sleeping || Utils.SleepCheck("ItemManager.ForceRapierDisassemble"))
            {
                return;
            }

            items.Disassemble(ItemId.item_rapier);
            evaderCheck.Sleep(1000);
        }

        private void TimeCheck()
        {
            var rapiers =
                items.GetMyItems(Items.StoredPlace.Backpack | Items.StoredPlace.Inventory)
                    .Where(
                        x =>
                            (ItemId)x.ID == ItemId.item_rapier && x.AssembledTime + 10 > Game.RawGameTime
                            && x.AssembledTime + 7 < Game.RawGameTime)
                    .ToList();

            if (!rapiers.Any())
            {
                return;
            }

            rapiers.ForEach(x => x.DisassembleItem());

            DelayAction.Add(
                500,
                () => {
                    if (hero.Health <= menu.HpThreshold)
                    {
                        return;
                    }

                    items.UnlockCombining(new[] { ItemId.item_demon_edge, ItemId.item_relic });
                    cts = new CancellationTokenSource();
                    DelayAction.Add(8000, TimeCheck, cts.Token);
                });
        }

        #endregion
    }
}