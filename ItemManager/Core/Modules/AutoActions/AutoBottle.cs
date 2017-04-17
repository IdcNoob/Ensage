namespace ItemManager.Core.Modules.AutoActions
{
    using System;
    using System.Linq;

    using Abilities;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using EventArgs;

    using Menus;
    using Menus.Modules.AutoActions.Actions;
    using Menus.Modules.Recovery;

    using SharpDX;

    using Utils;

    [Module]
    internal class AutoBottle : IDisposable
    {
        private readonly Vector3 fountain;

        private readonly Manager manager;

        private readonly AutoBottleMenu menu;

        private readonly RecoveryMenu recoveryMenu;

        private readonly MultiSleeper sleeper = new MultiSleeper();

        private Bottle bottle;

        private bool subscribed;

        public AutoBottle(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.AutoBottleMenu;
            recoveryMenu = menu.RecoveryMenu;

            fountain = ObjectManager.GetEntities<Unit>()
                .First(x => x.ClassId == ClassId.CDOTA_Unit_Fountain && x.Team == manager.MyTeam)
                .Position;

            manager.OnItemAdd += OnItemAdd;
            manager.OnItemRemove += OnItemRemove;
        }

        public bool BottleCanBeRefilled()
        {
            if (manager.MyHero.Distance2D(fountain) < 1300)
            {
                return true;
            }

            if (!manager.MyHero.HasModifier(ModifierUtils.FountainRegeneration))
            {
                return false;
            }

            if (!sleeper.Sleeping("FountainRegeneration"))
            {
                sleeper.Sleep(5000, "FountainRegeneration");
                sleeper.Sleep(2000, "CanRefill");
            }

            return sleeper.Sleeping("CanRefill");
        }

        public void Dispose()
        {
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!args.Entities.Contains(manager.MyHero) || args.IsQueued || !args.Process)
            {
                return;
            }

            switch (args.OrderId)
            {
                case OrderId.AbilityTarget:
                case OrderId.AbilityLocation:
                case OrderId.Ability:
                case OrderId.ToggleAbility:
                {
                    if (args.Ability.IsAbilityBehavior(AbilityBehavior.Channeled))
                    {
                        sleeper.Sleep(500, this);
                    }
                    break;
                }
            }
        }

        private void OnItemAdd(object sender, ItemEventArgs itemEventArgs)
        {
            if (!itemEventArgs.IsMine || itemEventArgs.Item.Id != AbilityId.item_bottle || subscribed)
            {
                return;
            }

            bottle = manager.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId.item_bottle) as Bottle;

            if (bottle == null)
            {
                return;
            }

            subscribed = true;
            Game.OnUpdate += OnUpdate;
            Player.OnExecuteOrder += OnExecuteOrder;
        }

        private void OnItemRemove(object sender, ItemEventArgs itemEventArgs)
        {
            if (!itemEventArgs.IsMine || itemEventArgs.Item.Id != AbilityId.item_bottle || subscribed)
            {
                return;
            }

            bottle = manager.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId.item_bottle) as Bottle;

            if (bottle != null)
            {
                return;
            }

            subscribed = false;
            Game.OnUpdate -= OnUpdate;
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        private void OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping(this) || Game.IsPaused || recoveryMenu.IsActive)
            {
                return;
            }

            sleeper.Sleep(300, this);

            if (!manager.MyHeroCanUseItems() || !bottle.CanBeAutoCasted() || !BottleCanBeRefilled())
            {
                return;
            }

            var useOnAllies = menu.AutoAllyBottle;
            var useOnSelf = menu.AutoSelfBottle;

            var bottleTarget = ObjectManager.GetEntitiesParallel<Hero>()
                .Where(
                    x => !x.IsIllusion && x.Distance2D(manager.MyHero) <= bottle.GetCastRange() && x.IsAlive
                         && x.Team == manager.MyTeam && !x.IsInvul())
                .OrderBy(x => x.FindModifier(ModifierUtils.BottleRegeneration)?.RemainingTime)
                .FirstOrDefault(
                    x => (useOnAllies && !x.Equals(manager.MyHero) || useOnSelf && x.Equals(manager.MyHero))
                         && (x.Health < x.MaximumHealth || x.Mana < x.MaximumMana));

            if (bottleTarget != null)
            {
                if (bottleTarget.Equals(manager.MyHero))
                {
                    bottle.Use();
                }
                else
                {
                    bottle.Use(bottleTarget);
                }
                bottle.SetSleep(200);
                sleeper.Sleep(190, this);
            }
        }
    }
}