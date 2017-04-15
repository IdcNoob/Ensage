namespace ItemManager.Core.Modules.AutoActions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abilities;

    using Ensage;
    using Ensage.Common.AbilityInfo;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;
    using Ensage.Common.Objects.UtilityObjects;

    using EventArgs;

    using Menus.Modules.AutoActions.Actions;
    using Menus.Modules.Recovery;

    using Attribute = Ensage.Attribute;

    internal class PowerTreadsSwitcher : IDisposable
    {
        private readonly List<string> activeDelaySwitchModifiers = new List<string>();

        private readonly List<string> delaySwitchModifiers = new List<string>
        {
            // abilities
            "modifier_leshrac_pulse_nova",
            "modifier_morphling_morph_agi",
            "modifier_morphling_morph_str",
            "modifier_voodoo_restoration_aura",
            "modifier_brewmaster_primal_split",
            "modifier_eul_cyclone",
            "modifier_storm_spirit_ball_lightning",
            // heal
            "modifier_item_urn_heal",
            "modifier_flask_healing",
            "modifier_bottle_regeneration",
            "modifier_voodoo_restoration_heal",
            "modifier_tango_heal",
            "modifier_enchantress_natures_attendants",
            "modifier_oracle_purifying_flames",
            "modifier_warlock_shadow_word",
            "modifier_treant_living_armor",
            "modifier_filler_heal",
            // invis
            "modifier_invisible",
            "modifier_bounty_hunter_wind_walk",
            "modifier_clinkz_wind_walk",
            "modifier_item_glimmer_cape_fade",
            "modifier_invoker_ghost_walk_self",
            "modifier_mirana_moonlight_shadow",
            "modifier_nyx_assassin_vendetta",
            "modifier_sandking_sand_storm_invis",
            "modifier_rune_invis",
            "modifier_item_shadow_amulet_fade",
            "modifier_item_silver_edge_windwalk",
            "modifier_item_invisibility_edge_windwalk",
            "modifier_templar_assassin_meld",
            "modifier_weaver_shukuchi"
        };

        private readonly Manager manager;

        private readonly PowerTreadsMenu menu;

        private readonly Order order;

        private readonly RecoveryMenu recoveryMenu;

        private readonly MultiSleeper sleeper = new MultiSleeper();

        private PowerTreads powerTreads;

        private bool subscribed;

        public PowerTreadsSwitcher(Manager manager, PowerTreadsMenu menu, RecoveryMenu recoveryMenu)
        {
            this.manager = manager;
            this.menu = menu;
            this.recoveryMenu = recoveryMenu;

            order = new Order();

            manager.OnAbilityAdd += OnAbilityAdd;
            manager.OnAbilityRemove += OnAbilityRemove;
        }

        public void Dispose()
        {
            Player.OnExecuteOrder -= OnExecuteOrder;
            Game.OnUpdate -= OnUpdate;
            manager.OnAbilityAdd -= OnAbilityAdd;
            manager.OnAbilityRemove -= OnAbilityRemove;

            activeDelaySwitchModifiers.Clear();
        }

        private void OnAbilityAdd(object sender, AbilityEventArgs abilityEventArgs)
        {
            if (!abilityEventArgs.IsMine)
            {
                return;
            }

            if (abilityEventArgs.Ability.GetManaCost(0) > 0)
            {
                menu.AddAbility(abilityEventArgs.Ability.StoredName(), true);
            }

            if (abilityEventArgs.Ability.Id == AbilityId.item_power_treads)
            {
                powerTreads =
                    manager.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId.item_power_treads) as PowerTreads;

                if (powerTreads != null && !subscribed)
                {
                    subscribed = true;

                    Player.OnExecuteOrder += OnExecuteOrder;
                    Game.OnUpdate += OnUpdate;
                    Unit.OnModifierAdded += OnModifierAdded;
                    Unit.OnModifierRemoved += OnModifierRemoved;
                }
            }
        }

        private void OnAbilityRemove(object sender, AbilityEventArgs abilityEventArgs)
        {
            if (!abilityEventArgs.IsMine)
            {
                return;
            }

            if (abilityEventArgs.Ability.GetManaCost(0) > 0)
            {
                menu.RemoveAbility(abilityEventArgs.Ability.StoredName());
            }

            if (abilityEventArgs.Ability.Id == AbilityId.item_power_treads)
            {
                powerTreads =
                    manager.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId.item_power_treads) as PowerTreads;

                if (powerTreads == null)
                {
                    subscribed = false;

                    Player.OnExecuteOrder -= OnExecuteOrder;
                    Game.OnUpdate -= OnUpdate;
                }
            }
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!menu.IsEnabled)
            {
                return;
            }

            var orderId = args.OrderId;

            if (orderId != OrderId.Ability && orderId != OrderId.AbilityLocation && orderId != OrderId.AbilityTarget
                && orderId != OrderId.ToggleAbility)
            {
                return;
            }

            if (!args.Process || args.IsQueued || !args.Entities.Contains(manager.MyHero))
            {
                return;
            }

            var ability = args.Ability;

            if (!args.IsPlayerInput)
            {
                if (ability.Id == AbilityId.item_power_treads)
                {
                    powerTreads.SetSleep(300);
                    return;
                }

                if (!menu.UniversalUseEnabled)
                {
                    return;
                }
            }
            else if (ability.Id == AbilityId.item_power_treads)
            {
                powerTreads.ChangeDefaultAttribute();
                return;
            }

            if (!powerTreads.CanBeCasted() || manager.MyHero.IsInvisible())
            {
                return;
            }

            if (ability.ManaCost <= menu.MpAbilityThreshold || !menu.IsAbilityEnabled(ability.StoredName()))
            {
                return;
            }

            var sleep = Math.Max(ability.FindCastPoint() * 1000, 200) + 350;

            switch (args.OrderId)
            {
                case OrderId.AbilityTarget:
                {
                    var target = args.Target as Unit;
                    if (target != null && target.IsValid && target.IsAlive)
                    {
                        if (manager.MyHero.Distance2D(target) <= ability.GetCastRange() + 300)
                        {
                            powerTreads.SwitchTo(Attribute.Intelligence);
                            order.UseAbility(ability, target);
                            sleep += manager.MyHero.GetTurnTime(target) * 1000;
                            args.Process = false;
                        }
                    }
                    break;
                }
                case OrderId.AbilityLocation:
                {
                    var targetLocation = args.TargetPosition;
                    if (manager.MyHero.Distance2D(targetLocation) <= ability.GetCastRange() + 300
                        || !AbilityDatabase.Find(ability.StoredName()).FakeCastRange)
                    {
                        powerTreads.SwitchTo(Attribute.Intelligence);
                        order.UseAbility(ability, targetLocation);
                        sleep += manager.MyHero.GetTurnTime(targetLocation) * 1000;
                        args.Process = false;
                    }
                    break;
                }
                case OrderId.Ability:
                {
                    powerTreads.SwitchTo(Attribute.Intelligence);
                    order.UseAbility(ability);
                    args.Process = false;
                    break;
                }
                case OrderId.ToggleAbility:
                {
                    powerTreads.SwitchTo(Attribute.Intelligence);
                    order.ToggleAbility(ability);
                    args.Process = false;
                    break;
                }
            }

            if (!args.Process)
            {
                sleeper.Sleep((float)sleep, AbilityId.item_power_treads, true);
            }
        }

        private void OnModifierAdded(Unit sender, ModifierChangedEventArgs args)
        {
            if (sender.Handle != manager.MyHandle)
            {
                return;
            }

            var modifier = delaySwitchModifiers.FirstOrDefault(x => x == args.Modifier.Name);
            if (!string.IsNullOrEmpty(modifier))
            {
                activeDelaySwitchModifiers.Add(modifier);

                if (sleeper.Sleeping(AbilityId.item_power_treads) || recoveryMenu.IsActive)
                {
                    return;
                }

                switch (modifier)
                {
                    case "modifier_item_urn_heal":
                    case "modifier_flask_healing":
                    case "modifier_filler_heal":
                    case "modifier_bottle_regeneration":
                    {
                        powerTreads.SwitchTo(Attribute.Agility);
                        break;
                    }
                }
            }
        }

        private void OnModifierRemoved(Unit sender, ModifierChangedEventArgs args)
        {
            if (sender.Handle != manager.MyHandle)
            {
                return;
            }

            var modifier = delaySwitchModifiers.FirstOrDefault(x => x == args.Modifier.Name);
            if (!string.IsNullOrEmpty(modifier))
            {
                activeDelaySwitchModifiers.Remove(modifier);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping(this) || Game.IsPaused)
            {
                return;
            }

            sleeper.Sleep(200, this);

            if (!menu.IsEnabled || sleeper.Sleeping(AbilityId.item_power_treads) || !manager.MyHeroCanUseItems())
            {
                return;
            }

            if (!powerTreads.CanBeCasted() || powerTreads.ActiveAttribute == powerTreads.DefaultAttribute
                || !manager.MyHero.CanUseItems() || !powerTreads.CanBeCasted() || manager.MyHero.IsChanneling()
                || manager.MyHero.IsInvisible() || activeDelaySwitchModifiers.Any())
            {
                return;
            }

            powerTreads.SwitchTo(powerTreads.DefaultAttribute);
        }
    }
}