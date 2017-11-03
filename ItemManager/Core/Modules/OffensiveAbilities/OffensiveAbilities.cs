namespace ItemManager.Core.Modules.OffensiveAbilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abilities.Interfaces;

    using Attributes;

    using Ensage;
    using Ensage.Common.Objects;
    using Ensage.Common.Objects.UtilityObjects;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;

    using Menus;
    using Menus.Modules.OffensiveAbilities;

    using AbilityEventArgs = EventArgs.AbilityEventArgs;

    [Module]
    internal class OffensiveAbilities : IDisposable
    {
        private readonly Dictionary<AbilityId, string> forcedNames = new Dictionary<AbilityId, string>
        {
            { AbilityId.item_bloodthorn, "item_orchid" },
            { AbilityId.item_solar_crest, "item_medallion_of_courage" }
        };

        private readonly Manager manager;

        private readonly OffensiveAbilitiesMenu menu;

        private readonly List<IOffensiveAbility> offensiveAbilities = new List<IOffensiveAbility>();

        private readonly Dictionary<AbilityId, string> offensiveAbilityNames = new Dictionary<AbilityId, string>
        {
            { AbilityId.item_abyssal_blade, "Abyssal blade" },
            { AbilityId.item_diffusal_blade, "Diffusal blade" },
            { AbilityId.item_ethereal_blade, "Ethereal blade" },
            { AbilityId.item_cyclone, "Euls scepter of divinity" },
            { AbilityId.item_heavens_halberd, "Heavens halberd" },
            { AbilityId.item_medallion_of_courage, "Medallion of courage" },
            { AbilityId.item_solar_crest, "Medallion of courage" },
            { AbilityId.item_orchid, "Orchid malevolence" },
            { AbilityId.item_bloodthorn, "Orchid malevolence" },
            { AbilityId.item_rod_of_atos, "Rod of atos" },
            { AbilityId.item_sheepstick, "Scythe of vyse" },
            { AbilityId.item_urn_of_shadows, "Urn of shadows" },
            { AbilityId.item_spirit_vessel, "Spirit vessel" },
            { AbilityId.item_veil_of_discord, "Veil of discord" },
            { AbilityId.item_armlet, "Armlet" },
            { AbilityId.item_satanic, "Satanic" },
            { AbilityId.item_nullifier, "Nullifier" }
        };

        private readonly MultiSleeper sleeper = new MultiSleeper();

        private readonly IUpdateHandler updateHandler;

        public OffensiveAbilities(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.OffensiveAbilitiesMenu;

            updateHandler = UpdateManager.Subscribe(OnUpdate, 100, false);
            manager.OnAbilityAdd += OnAbilityAdd;
            manager.OnAbilityRemove += OnAbilityRemove;
        }

        public void Dispose()
        {
            UpdateManager.Unsubscribe(OnUpdate);
            manager.OnAbilityAdd -= OnAbilityAdd;
            manager.OnAbilityRemove -= OnAbilityRemove;

            offensiveAbilities.Clear();
        }

        private void OnAbilityAdd(object sender, AbilityEventArgs abilityEventArgs)
        {
            if (!abilityEventArgs.IsMine)
            {
                return;
            }

            var ability = abilityEventArgs.Ability;

            var usableAbility = manager.MyHero.UsableAbilities.OfType<IOffensiveAbility>().FirstOrDefault(x => x.Handle == ability.Handle);

            if (usableAbility == null)
            {
                return;
            }

            var forcedName = forcedNames.FirstOrDefault(x => x.Key == ability.Id).Value;
            if (!string.IsNullOrEmpty(forcedName))
            {
                usableAbility.ChangeName(forcedName);
            }

            var menuName = offensiveAbilityNames.FirstOrDefault(x => x.Key == ability.Id).Value;

            menu.CreateMenu(usableAbility, menuName);
            offensiveAbilities.Add(usableAbility);
            updateHandler.IsEnabled = true;
        }

        private void OnAbilityRemove(object sender, AbilityEventArgs abilityEventArgs)
        {
            if (!abilityEventArgs.IsMine)
            {
                return;
            }

            offensiveAbilities.RemoveAll(x => x.Handle == abilityEventArgs.Ability.Handle);

            if (!offensiveAbilities.Any())
            {
                updateHandler.IsEnabled = false;
            }
        }

        private void OnUpdate()
        {
            if (Game.IsPaused || !manager.MyHero.IsAlive)
            {
                return;
            }

            var canUseItems = manager.MyHero.CanUseItems();
            var canUseAbilities = manager.MyHero.CanUseAbilities();

            foreach (var offensiveAbility in offensiveAbilities
                .Where(x => menu.IsAbilityEnabled(x.Name) && x.CanBeCasted() && (x.IsItem ? canUseItems : canUseAbilities))
                .OrderByDescending(x => menu.GetPriority(x.Name)))
            {
                var target = manager.MyHero.Target;

                if (!offensiveAbility.CanBeCasted(target) || sleeper.Sleeping(target) || !menu.IsHeroEnabled(target.StoredName()))
                {
                    target = null;
                }

                if (offensiveAbility.Menu.AlwaysUse && target == null)
                {
                    target = EntityManager<Hero>.Entities
                        .Where(
                            x => x.IsValid && x.IsVisible && !sleeper.Sleeping(x) && x.Team != manager.MyHero.Team
                                 && menu.IsHeroEnabled(x.StoredName()) && !x.IsIllusion)
                        .OrderBy(x => manager.MyHero.Hero.FindRotationAngle(x.Position))
                        .FirstOrDefault(x => offensiveAbility.CanBeCasted(x));
                }

                if (target == null)
                {
                    return;
                }

                UpdateManager.BeginInvoke(
                    () =>
                        {
                            if (!offensiveAbility.CanBeCasted() || !offensiveAbility.CanBeCasted(target))
                            {
                                return;
                            }

                            offensiveAbility.Use(target);
                        },
                    offensiveAbility.Menu.Delay);

                sleeper.Sleep(Math.Max(500, offensiveAbility.Menu.Delay), target);
            }
        }
    }
}