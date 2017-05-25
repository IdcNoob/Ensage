namespace ItemManager.Core.Modules.AutoActions.HpMpRestore
{
    using System.Linq;

    using Abilities;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;
    using Ensage.SDK.Helpers;

    using Interfaces;

    using Menus;
    using Menus.Modules.AutoActions.HpMpRestore;
    using Menus.Modules.Recovery;

    using SharpDX;

    using Utils;

    [AbilityBasedModule(AbilityId.item_bottle)]
    internal class AutoBottle : IAbilityBasedModule
    {
        private readonly Vector3 fountain;

        private readonly Manager manager;

        private readonly AutoBottleMenu menu;

        private readonly RecoveryMenu recoveryMenu;

        private readonly MultiSleeper sleeper = new MultiSleeper();

        private Bottle bottle;

        public AutoBottle(Manager manager, MenuManager menu, AbilityId abilityId)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.AutoHealsMenu.AutoBottleMenu;
            recoveryMenu = menu.RecoveryMenu;

            fountain = ObjectManager.GetEntitiesParallel<Unit>()
                .First(x => x.IsValid && x.ClassId == ClassId.CDOTA_Unit_Fountain && x.Team == manager.MyHero.Team)
                .Position;

            AbilityId = abilityId;
            Refresh();

            UpdateManager.Subscribe(OnUpdate, 300);
            Player.OnExecuteOrder += OnExecuteOrder;
        }

        public AbilityId AbilityId { get; }

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
            UpdateManager.Unsubscribe(OnUpdate);
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        public void Refresh()
        {
            bottle = manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId) as Bottle;
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
            if (Game.IsPaused || recoveryMenu.IsActive || !manager.MyHero.CanUseItems() || !bottle.CanBeAutoCasted()
                || !BottleCanBeRefilled())
            {
                return;
            }

            var useOnAllies = menu.AutoAllyBottle;
            var useOnSelf = menu.AutoSelfBottle;

            var bottleTarget = ObjectManager.GetEntitiesParallel<Hero>()
                .Where(
                    x => !x.IsIllusion && x.Distance2D(manager.MyHero.Position) <= bottle.GetCastRange() && x.IsAlive
                         && x.Team == manager.MyHero.Team && !x.IsInvul())
                .OrderBy(x => x.FindModifier(ModifierUtils.BottleRegeneration)?.RemainingTime)
                .FirstOrDefault(
                    x => (useOnAllies && x.Handle != manager.MyHero.Handle
                          || useOnSelf && x.Handle == manager.MyHero.Handle)
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