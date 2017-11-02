namespace ItemManager.Core.Modules.AutoActions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;

    using EventArgs;

    using Menus;
    using Menus.Modules.AutoActions.Actions;

    using Utils;

    [Module]
    internal class TechiesMinesDestroyer : IDisposable
    {
        private readonly Sleeper block = new Sleeper();

        private readonly float chopRange;

        private readonly Manager manager;

        private readonly TechiesMinesDestroyerMenu menu;

        private readonly Sleeper sleeper = new Sleeper();

        private readonly HashSet<AbilityId> usableItems = new HashSet<AbilityId>
        {
            AbilityId.item_quelling_blade,
            AbilityId.item_bfury
        };

        private IUpdateHandler updateHandler;

        public TechiesMinesDestroyer(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.TechiesMinesDestroyerMenu;

            chopRange = Ability.GetAbilityDataById(AbilityId.item_quelling_blade)
                .AbilitySpecialData.First(x => x.Name == "cast_range_ward")
                .Value;

            manager.OnUnitAdd += OnUnitAdd;
            this.menu.OnEnabledChange += MenuOnEnabledChange;
            this.menu.OnUpdateRateChange += MenuOnUpdateRateChange;
        }

        public void Dispose()
        {
            menu.OnEnabledChange -= MenuOnEnabledChange;
            menu.OnUpdateRateChange -= MenuOnUpdateRateChange;
            manager.OnUnitAdd -= OnUnitAdd;
            UpdateManager.Unsubscribe(OnUpdate);
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        private void MenuOnEnabledChange(object sender, BoolEventArgs boolEventArgs)
        {
            if (updateHandler == null)
            {
                return;
            }

            if (boolEventArgs.Enabled)
            {
                updateHandler.IsEnabled = true;
                Player.OnExecuteOrder += OnExecuteOrder;
            }
            else
            {
                updateHandler.IsEnabled = false;
                Player.OnExecuteOrder -= OnExecuteOrder;
            }
        }

        private void MenuOnUpdateRateChange(object sender, IntEventArgs intEventArgs)
        {
            updateHandler?.SetUpdateRate(intEventArgs.Time);
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!args.Entities.Contains(manager.MyHero.Hero) || !block.Sleeping || !args.IsPlayerInput)
            {
                return;
            }

            args.Process = false;
        }

        private void OnUnitAdd(object sender, UnitEventArgs unitEventArgs)
        {
            var hero = unitEventArgs.Unit as Hero;
            if (hero != null && (hero.HeroId == HeroId.npc_dota_hero_techies && hero.Team != manager.MyHero.Team
                                 || hero.HeroId == HeroId.npc_dota_hero_rubick && EntityManager<Hero>.Entities.Any(
                                     x => x.IsValid && x.HeroId == HeroId.npc_dota_hero_techies && x.Team == manager.MyHero.Team)))
            {
                manager.OnUnitAdd -= OnUnitAdd;
                updateHandler = UpdateManager.Subscribe(OnUpdate, menu.UpdateRate, menu.IsEnabled);
                if (menu.IsEnabled)
                {
                    Player.OnExecuteOrder += OnExecuteOrder;
                }
            }
        }

        private void OnUpdate()
        {
            if (Game.IsPaused || sleeper.Sleeping)
            {
                return;
            }

            var canUseItems = manager.MyHero.CanUseItems();
            var canAttack = manager.MyHero.CanAttack();

            if (!canAttack && !canUseItems)
            {
                return;
            }

            var techiesMines = EntityManager<Unit>.Entities.Where(
                    x => x.IsValid && x.IsVisible && x.IsTechiesMine() && !x.IsInvul() && x.IsAlive && x.Team != manager.MyHero.Team
                         && x.Distance2D(manager.MyHero.Position) <= 1000)
                .ToList();

            var attackRange = manager.MyHero.Hero.GetAttackRange();
            var isInvisible = manager.MyHero.IsInvisible();

            foreach (var mine in techiesMines)
            {
                var distance = manager.MyHero.Distance2D(mine);

                if (canUseItems && mine.Health > 1 && distance <= chopRange)
                {
                    var item = manager.MyHero.GetItems(ItemStoredPlace.Inventory)
                        .FirstOrDefault(x => usableItems.Contains(x.Id) && x.CanBeCasted());

                    if (item != null)
                    {
                        item.UseAbility(mine);
                        sleeper.Sleep(1000);
                        return;
                    }
                }

                if (!menu.AttackMines || !canAttack || isInvisible && !menu.AttackMinesInvisible)
                {
                    continue;
                }

                if (distance < attackRange + 200 && !techiesMines.Any(x => !x.Equals(mine) && x.Distance2D(mine) < 400))
                {
                    if (mine.Health <= manager.MyHero.Damage)
                    {
                        if (manager.MyHero.Hero.Attack(mine))
                        {
                            block.Sleep(
                                ((float)((Math.Max(0, distance - attackRange) / manager.MyHero.Hero.MovementSpeed)
                                         + manager.MyHero.Hero.AttackPoint() + manager.MyHero.Hero.GetTurnTime(mine)) * 1000) + Game.Ping);
                            sleeper.Sleep(1000);
                            return;
                        }
                    }
                }
            }
        }
    }
}