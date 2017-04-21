namespace ItemManager.Core.Modules.AutoActions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abilities;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using Interfaces;

    using Menus;
    using Menus.Modules.AutoActions.Actions;

    using Utils;

    [AbilityBasedModule(AbilityId.item_hand_of_midas)]
    internal class AutoMidas : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly MidasMenu menu;

        private readonly Sleeper sleeper = new Sleeper();

        private HandOfMidas handOfMidas;

        public AutoMidas(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.MidasMenu;

            Refresh();
        }

        public List<AbilityId> AbilityIds { get; } = new List<AbilityId>
        {
            AbilityId.item_hand_of_midas
        };

        public void Dispose()
        {
            Game.OnUpdate -= OnUpdate;
        }

        public void Refresh()
        {
            handOfMidas = manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityIds.First()) as HandOfMidas;
        }

        private void OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            sleeper.Sleep(500);

            if (!menu.IsEnabled || !manager.MyHero.CanUseItems() || !handOfMidas.CanBeCasted() || Game.IsPaused)
            {
                return;
            }

            var creep = ObjectManager.GetEntitiesParallel<Creep>()
                .Where(
                    x => x.IsValid && x.IsAlive && x.IsSpawned && x.IsVisible
                         && x.Distance2D(manager.MyHero.Position) <= handOfMidas.GetCastRange()
                         && x.Team != manager.MyHero.Team && !x.IsAncient
                         && x.HealthPercentage() >= menu.HealthThresholdPct
                         && x.GetGrantedExperience() >= menu.ExperienceThreshold)
                .OrderByDescending(x => x.Health)
                .FirstOrDefault();

            if (creep != null)
            {
                handOfMidas.Use(creep);
            }
        }
    }
}