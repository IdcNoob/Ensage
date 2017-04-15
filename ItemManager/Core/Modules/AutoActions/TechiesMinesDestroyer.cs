namespace ItemManager.Core.Modules.AutoActions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using EventArgs;

    using Menus.Modules.AutoActions.Actions;

    using Utils;

    internal class TechiesMinesDestroyer : IDisposable
    {
        private readonly Sleeper block = new Sleeper();

        private readonly float chopRange;

        private readonly Manager manager;

        private readonly TechiesMinesDestroyerMenu menu;

        private readonly Sleeper sleeper = new Sleeper();

        private readonly List<AbilityId> usableItems = new List<AbilityId>
        {
            AbilityId.item_iron_talon,
            AbilityId.item_quelling_blade,
            AbilityId.item_bfury
        };

        public TechiesMinesDestroyer(Manager manager, TechiesMinesDestroyerMenu menu)
        {
            this.manager = manager;
            this.menu = menu;

            chopRange = Ability.GetAbilityDataById(AbilityId.item_quelling_blade)
                .AbilitySpecialData.First(x => x.Name == "cast_range_ward")
                .Value;

            manager.OnUnitAdd += OnUnitAdd;
        }

        public void Dispose()
        {
            manager.OnUnitAdd -= OnUnitAdd;
            Game.OnUpdate -= OnUpdate;
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!args.Entities.Contains(manager.MyHero) || !block.Sleeping)
            {
                return;
            }

            args.Process = false;
        }

        private void OnUnitAdd(object sender, UnitEventArgs unitEventArgs)
        {
            var hero = unitEventArgs.Unit as Hero;
            if (hero != null && hero.HeroId == HeroId.npc_dota_hero_techies && hero.Team != manager.MyTeam)
            {
                manager.OnUnitAdd -= OnUnitAdd;
                Game.OnUpdate += OnUpdate;
                Player.OnExecuteOrder += OnExecuteOrder;
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping || Game.IsPaused)
            {
                return;
            }

            sleeper.Sleep(200);

            if (!menu.DestroyMines || !manager.MyHero.IsAlive || !manager.MyHero.CanAttack()
                || manager.MyHero.IsChanneling())
            {
                return;
            }

            var techiesMines = ObjectManager.GetEntitiesParallel<Unit>()
                .Where(
                    x => x.IsValid && x.IsTechiesMine() && x.IsAlive && x.Team != manager.MyTeam
                         && x.Distance2D(manager.MyHero) <= 1000)
                .ToList();

            var item = manager.GetMyItems(ItemUtils.StoredPlace.Inventory)
                .FirstOrDefault(x => usableItems.Contains(x.Id) && x.CanBeCasted());

            var canUseItems = manager.MyHero.CanUseItems();
            var attackRange = manager.MyHero.GetAttackRange();

            foreach (var mine in techiesMines)
            {
                var distance = manager.MyHero.Distance2D(mine);

                if (canUseItems && mine.Health > 1 && distance <= chopRange)
                {
                    if (item != null)
                    {
                        item.UseAbility(mine);
                        return;
                    }
                }

                if (!menu.AttackMines)
                {
                    continue;
                }

                if (distance < attackRange + 200 && !techiesMines.Any(x => !x.Equals(mine) && x.Distance2D(mine) < 400))
                {
                    if (mine.Health <= manager.MyHero.MinimumDamage + manager.MyHero.BonusDamage)
                    {
                        if (manager.MyHero.Attack(mine))
                        {
                            block.Sleep(
                                (float)(Math.Max(0, distance - attackRange) / manager.MyHero.MovementSpeed
                                        + manager.MyHero.AttackPoint() + manager.MyHero.GetTurnTime(mine)) * 1000
                                + Game.Ping);
                            sleeper.Sleep(500);
                            return;
                        }
                    }
                }
            }
        }
    }
}