namespace ItemManager.Core.Modules.AutoActions.HpMpRestore
{
    using System.Linq;

    using Abilities;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;

    using EventArgs;

    using Interfaces;

    using Menus;
    using Menus.Modules.AutoActions.HpMpRestore;
    using Menus.Modules.Recovery;

    using Utils;

    [AbilityBasedModule(AbilityId.item_bottle)]
    internal class AutoBottle : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly AutoBottleMenu menu;

        private readonly RecoveryMenu recoveryMenu;

        private readonly MultiSleeper sleeper = new MultiSleeper();

        private readonly IUpdateHandler updateHandler;

        private Bottle bottle;

        private Unit fountain;

        public AutoBottle(Manager manager, MenuManager menu, AbilityId abilityId)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.AutoHealsMenu.AutoBottleMenu;
            recoveryMenu = menu.RecoveryMenu;

            AbilityId = abilityId;
            Refresh();

            updateHandler = UpdateManager.Subscribe(OnUpdate, 300, this.menu.IsEnabled);
            if (this.menu.IsEnabled)
            {
                Player.OnExecuteOrder += OnExecuteOrder;
            }
            this.menu.OnEnabledChange += MenuOnEnabledChange;
        }

        public AbilityId AbilityId { get; }

        private Unit Fountain
        {
            get
            {
                return fountain ?? (fountain = EntityManager<Building>.Entities.FirstOrDefault(
                                        x => x.IsValid && x.Name == "dota_fountain" && x.Team == manager.MyHero.Team));
            }
        }

        public bool BottleCanBeRefilled()
        {
            if (Fountain != null && manager.MyHero.Distance2D(Fountain) < 1300)
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
            menu.OnEnabledChange -= MenuOnEnabledChange;
            UpdateManager.Unsubscribe(OnUpdate);
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        public void Refresh()
        {
            bottle = manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId) as Bottle;
        }

        private void MenuOnEnabledChange(object sender, BoolEventArgs boolEventArgs)
        {
            if (boolEventArgs.Enabled)
            {
                Player.OnExecuteOrder += OnExecuteOrder;
                updateHandler.IsEnabled = true;
            }
            else
            {
                Player.OnExecuteOrder -= OnExecuteOrder;
                updateHandler.IsEnabled = false;
            }
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!args.Entities.Contains(manager.MyHero.Hero) || args.IsQueued || !args.Process)
            {
                return;
            }

            switch (args.OrderId)
            {
                case OrderId.AbilityTarget:
                case OrderId.AbilityLocation:
                case OrderId.Ability:
                {
                    if (args.Ability.IsAbilityBehavior(AbilityBehavior.Channeled) || args.Ability.IsInvis())
                    {
                        sleeper.Sleep(1000, this);
                    }
                    break;
                }
            }
        }

        private void OnUpdate()
        {
            if (Game.IsPaused || recoveryMenu.IsActive || !bottle.CanBeAutoCasted() || !BottleCanBeRefilled()
                || !manager.MyHero.CanUseItems() || sleeper.Sleeping(this))
            {
                return;
            }

            var useOnAllies = menu.AutoAllyBottle;
            var useOnSelf = menu.AutoSelfBottle;

            var bottleTarget = EntityManager<Hero>.Entities
                .Where(
                    x => x.IsValid && x.IsAlive && x.Team == manager.MyHero.Team && !x.IsIllusion
                         && x.Distance2D(manager.MyHero.Position) <= bottle.GetCastRange() && !x.IsInvul())
                .OrderBy(x => x.FindModifier(ModifierUtils.BottleRegeneration)?.RemainingTime)
                .FirstOrDefault(
                    x => (useOnAllies && x.Handle != manager.MyHero.Handle || useOnSelf && x.Handle == manager.MyHero.Handle)
                         && (x.Health < x.MaximumHealth || x.Mana < x.MaximumMana));

            if (bottleTarget != null)
            {
                if (bottleTarget.Handle == manager.MyHero.Handle)
                {
                    bottle.Use();
                }
                else
                {
                    bottle.Use(bottleTarget);
                }

                bottle.SetSleep(200);
            }
        }
    }
}