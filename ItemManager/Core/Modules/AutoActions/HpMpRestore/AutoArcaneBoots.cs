namespace ItemManager.Core.Modules.AutoActions.HpMpRestore
{
    using System.Linq;

    using Abilities;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.SDK.Helpers;

    using Interfaces;

    using Menus;
    using Menus.Modules.AutoActions.HpMpRestore;

    using SharpDX;

    [AbilityBasedModule(AbilityId.item_arcane_boots)]
    internal class AutoArcaneBoots : IAbilityBasedModule
    {
        private readonly Vector3 fountain;

        private readonly Manager manager;

        private readonly AutoArcaneBootsMenu menu;

        private ArcaneBoots arcaneBoots;

        private bool notified;

        public AutoArcaneBoots(Manager manager, MenuManager menu, AbilityId abilityId)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.AutoHealsMenu.AutoArcaneBootsMenu;

            fountain = ObjectManager.GetEntitiesParallel<Unit>()
                .First(x => x.IsValid && x.ClassId == ClassId.CDOTA_Unit_Fountain && x.Team == manager.MyHero.Team)
                .Position;

            AbilityId = abilityId;
            Refresh();

            UpdateManager.Subscribe(OnUpdate, 500);
            Player.OnExecuteOrder += OnExecuteOrder;
        }

        public AbilityId AbilityId { get; }

        public void Dispose()
        {
            UpdateManager.Unsubscribe(OnUpdate);
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        public void Refresh()
        {
            arcaneBoots = manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId) as ArcaneBoots;
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!args.Entities.Contains(manager.MyHero.Hero) || !args.Process || args.OrderId != OrderId.Ability)
            {
                return;
            }

            if (args.Ability?.Id == AbilityId)
            {
                notified = false;
            }
        }

        private void OnUpdate()
        {
            if (!menu.AutoUse || Game.IsPaused || !manager.MyHero.CanUseItems() || !arcaneBoots.CanBeCasted()
                || manager.MyHero.MissingMana < arcaneBoots.ManaRestore
                || manager.MyHero.Distance2D(fountain) < menu.FountainRange)
            {
                return;
            }

            if (manager.Units.OfType<Hero>()
                .Any(
                    x => x.IsValid && x.Handle != manager.MyHero.Handle && x.IsAlive && !x.IsIllusion
                         && x.Team == manager.MyHero.Team
                         && x.Distance2D(manager.MyHero.Position) <= menu.AllySearchRange
                         && x.Distance2D(manager.MyHero.Position) > arcaneBoots.GetCastRange()
                         && x.MaximumMana - x.Mana > arcaneBoots.ManaRestore))
            {
                if (!notified && menu.NotifyAllies)
                {
                    Network.ItemAlert(manager.MyHero.Position, AbilityId);
                    UpdateManager.BeginInvoke(() => { Network.ItemAlert(manager.MyHero.Position, AbilityId); }, 200);
                    notified = true;
                }

                return;
            }

            arcaneBoots.Use();
        }
    }
}