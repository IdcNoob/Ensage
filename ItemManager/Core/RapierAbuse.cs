namespace ItemManager.Core
{
    using System.Collections.Generic;
    using System.Linq;

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

        private readonly Sleeper delayDisassemble;

        private readonly string[] disableModifiers =
        {
            "modifier_shadow_demon_disruption",
            "modifier_obsidian_destroyer_astral_imprisonment_prison",
            "modifier_eul_cyclone",
            "modifier_invoker_tornado",
            "modifier_bane_nightmare",
            "modifier_shadow_shaman_shackles",
            "modifier_axe_berserkers_call",
            "modifier_storm_spirit_electric_vortex_pull",
            "modifier_cyclone",
            "modifier_sheepstick_debuff",
            "modifier_shadow_shaman_voodoo",
            "modifier_lion_voodoo",
            "modifier_sheepstick",
            "modifier_brewmaster_storm_cyclone",
            "modifier_invoker_deafening_blast_knockback",
            "modifier_pudge_meat_hook",
            "modifier_heavens_halberd_debuff",
            "modifier_legion_commander_duel"
        };

        private readonly List<AbilityId> enabledAbilities = new List<AbilityId>
        {
            AbilityId.kunkka_tidebringer,
            AbilityId.sniper_assassinate,
            AbilityId.monkey_king_boundless_strike,
            AbilityId.phantom_assassin_stifling_dagger
        };

        private readonly Hero hero;

        private readonly Items items;

        private readonly RapierAbuseMenu menu;

        private readonly List<ItemId> requiredItems = new List<ItemId>
        {
            ItemId.item_demon_edge,
            ItemId.item_relic
        };

        private bool manualModeEnabled;

        #endregion

        #region Constructors and Destructors

        public RapierAbuse(Hero myHero, Items myItems, RapierAbuseMenu rapierAbuseMenu)
        {
            hero = myHero;
            items = myItems;
            menu = rapierAbuseMenu;
            delayDisassemble = new Sleeper();

            if (items.GetMyItems(Items.StoredPlace.All).ToList().Exists(x => (ItemId)x.ID == ItemId.item_rapier))
            {
                Player.OnExecuteOrder += OnExecuteOrder;
                Entity.OnInt32PropertyChange += OnInt32PropertyChange;
                Unit.OnModifierAdded += OnModifierAdded;
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
                        DelayAction.Add(8000, TimeCheck);
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

                    DelayAction.Add(delay, () => items.Disassemble(ItemId.item_rapier));
                    break;
                default:
                    items.Disassemble(ItemId.item_rapier);
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
                DelayAction.Add(8000, TimeCheck);
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

            if (modifier.IsStunDebuff || disableModifiers.Contains(modifier.Name))
            {
                items.Disassemble(ItemId.item_rapier);
            }
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
                    DelayAction.Add(8000, TimeCheck);
                });
        }

        #endregion
    }
}