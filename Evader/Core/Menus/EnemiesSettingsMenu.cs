namespace Evader.Core.Menus
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Common;

    using Data;

    using Ensage.Common.Menu;

    using EvadableAbilities.Base;
    using EvadableAbilities.Base.Interfaces;

    internal class EnemiesSettingsMenu
    {
        #region Fields

        private readonly List<string> addedAbilities = new List<string>();

        private readonly Menu menu;

        private readonly Dictionary<string, Menu> unitMenus = new Dictionary<string, Menu>();

        #endregion

        #region Constructors and Destructors

        public EnemiesSettingsMenu(Menu rootMenu)
        {
            menu = new Menu("Enemies settings", "enemySettings");
            rootMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Methods and Operators

        public async Task AddAbility(EvadableAbility ability)
        {
            var ownerName = ability.Name.StartsWith("item_") ? "items" : ability.AbilityOwner.Name;
            if (ownerName == "npc_dota_neutral_mud_golem_split")
            {
                ownerName = "npc_dota_neutral_mud_golem";
            }
            var abilityName = ability.Name;
            var menuItemName = ownerName + abilityName;
            var modifierOnly = ability.GetType().BaseType?.Name == "EvadableAbility";

            if (addedAbilities.Contains(menuItemName))
            {
                var abilityEnabledItem = menu.Item(menuItemName + "enabled");
                ability.Enabled = abilityEnabledItem.GetValue<bool>();
                abilityEnabledItem.ValueChanged += (sender, args) => { ability.Enabled = args.GetNewValue<bool>(); };

                if (ability is IModifier)
                {
                    var modiferCounterItem = menu.Item(menuItemName + "modifier");
                    modiferCounterItem.ValueChanged +=
                        (sender, args) => { ability.ModifierCounterEnabled = args.GetNewValue<bool>(); };
                    ability.ModifierCounterEnabled = modiferCounterItem.IsActive();
                }

                if (!modifierOnly)
                {
                    var customPriorityItem = menu.Item(menuItemName + "customPriority");
                    customPriorityItem.ValueChanged +=
                        (sender, args) => { ability.UseCustomPriority = args.GetNewValue<bool>(); };
                    ability.UseCustomPriority = customPriorityItem.IsActive();

                    var abilityPriorityItem = menu.Item(menuItemName + "priorityFix");
                    var abilityTogglerItem = menu.Item(menuItemName + "togglerFix");

                    abilityPriorityItem.ValueChanged += (sender, args) => {
                        var changer = args.GetNewValue<PriorityChanger>();
                        ability.Priority.Clear();

                        foreach (var item in
                            changer.Dictionary.OrderByDescending(x => x.Value)
                                .Select(x => x.Key)
                                .Where(x => abilityTogglerItem.GetValue<AbilityToggler>().IsEnabled(x)))
                        {
                            switch (item)
                            {
                                case "item_sheepstick":
                                    ability.Priority.Add(EvadePriority.Disable);
                                    break;
                                case "item_cyclone":
                                    ability.Priority.Add(EvadePriority.Counter);
                                    break;
                                case "item_blink":
                                    ability.Priority.Add(EvadePriority.Blink);
                                    break;
                                case "centaur_stampede":
                                    ability.Priority.Add(EvadePriority.Walk);
                                    break;
                            }
                        }
                    };

                    abilityTogglerItem.ValueChanged += (sender, args) => {
                        var changer = abilityPriorityItem.GetValue<PriorityChanger>();
                        ability.Priority.Clear();

                        foreach (var item in
                            changer.Dictionary.OrderByDescending(x => x.Value)
                                .Select(x => x.Key)
                                .Where(x => args.GetNewValue<AbilityToggler>().IsEnabled(x)))
                        {
                            switch (item)
                            {
                                case "item_sheepstick":
                                    ability.Priority.Add(EvadePriority.Disable);
                                    break;
                                case "item_cyclone":
                                    ability.Priority.Add(EvadePriority.Counter);
                                    break;
                                case "item_blink":
                                    ability.Priority.Add(EvadePriority.Blink);
                                    break;
                                case "centaur_stampede":
                                    ability.Priority.Add(EvadePriority.Walk);
                                    break;
                            }
                        }
                    };

                    var hpIgnoreItem = menu.Item(menuItemName + "hpIgnore");
                    hpIgnoreItem.ValueChanged +=
                        (sender, args) => ability.AllyHpIgnore = args.GetNewValue<Slider>().Value;
                    ability.AllyHpIgnore = hpIgnoreItem.GetValue<Slider>().Value;

                    var abilityChangerItem = abilityPriorityItem.GetValue<PriorityChanger>();
                    foreach (var priority in
                        abilityChangerItem.Dictionary.OrderByDescending(x => x.Value)
                            .Select(x => x.Key)
                            .Where(x => abilityTogglerItem.GetValue<AbilityToggler>().IsEnabled(x)))
                    {
                        switch (priority)
                        {
                            case "item_sheepstick":
                                ability.Priority.Add(EvadePriority.Disable);
                                break;
                            case "item_cyclone":
                                ability.Priority.Add(EvadePriority.Counter);
                                break;
                            case "item_blink":
                                ability.Priority.Add(EvadePriority.Blink);
                                break;
                            case "centaur_stampede":
                                ability.Priority.Add(EvadePriority.Walk);
                                break;
                        }
                    }
                }

                return;
            }

            Menu heroMenu;
            if (!unitMenus.TryGetValue(ownerName, out heroMenu))
            {
                heroMenu = ownerName == "items"
                               ? new Menu("Items", "items")
                               : new Menu(ability.AbilityOwner.GetName(), ownerName, false, ownerName, true);
                menu.AddSubMenu(heroMenu);
                unitMenus.Add(ownerName, heroMenu);
            }

            var abilityMenu = new Menu(string.Empty, menuItemName, false, abilityName, true);
            await Task.Delay(100);

            var abilityEnabled = new MenuItem(menuItemName + "enabled", "Enabled").SetValue(true);
            abilityMenu.AddItem(abilityEnabled);
            abilityEnabled.ValueChanged += (sender, args) => { ability.Enabled = args.GetNewValue<bool>(); };
            ability.Enabled = abilityEnabled.IsActive();
            await Task.Delay(100);

            if (ability is IModifier)
            {
                var modiferCounter = new MenuItem(menuItemName + "modifier", "Modifer counter").SetValue(true);
                abilityMenu.AddItem(modiferCounter);
                modiferCounter.ValueChanged +=
                    (sender, args) => { ability.ModifierCounterEnabled = args.GetNewValue<bool>(); };
                ability.ModifierCounterEnabled = modiferCounter.IsActive();
                abilityMenu.DisplayName = "  *" + (modifierOnly ? "*" : string.Empty);
                await Task.Delay(100);
            }

            if (!modifierOnly)
            {
                var customPriority = new MenuItem(menuItemName + "customPriority", "Use custom priority").SetValue(
                    false);
                customPriority.SetTooltip("Enable this if you want to use custom priority");
                abilityMenu.AddItem(customPriority);
                customPriority.ValueChanged +=
                    (sender, args) => { ability.UseCustomPriority = args.GetNewValue<bool>(); };
                ability.UseCustomPriority = customPriority.IsActive();
                await Task.Delay(100);

                var abilityPriority =
                    new MenuItem(menuItemName + "priorityFix", "Custom priority").SetValue(
                        new PriorityChanger(
                            new List<string>
                            {
                                "item_sheepstick",
                                "item_cyclone",
                                "item_blink",
                                "centaur_stampede"
                            },
                            menuItemName + "changerFix"));
                abilityMenu.AddItem(abilityPriority);
                await Task.Delay(100);

                var abilityToggler =
                    new MenuItem(menuItemName + "togglerFix", "Custom enabled priority").SetValue(
                        new AbilityToggler(
                            new Dictionary<string, bool>
                            {
                                { "item_sheepstick", false },
                                { "item_cyclone", true },
                                { "item_blink", true },
                                { "centaur_stampede", true }
                            }));
                abilityMenu.AddItem(abilityToggler);
                await Task.Delay(100);

                var abilityLevelIgnore =
                    new MenuItem(menuItemName + "levelIgnore", "Ignore if ability level is equal or lower than")
                        .SetValue(new Slider(0, 0, 3));
                abilityMenu.AddItem(abilityLevelIgnore);
                abilityLevelIgnore.ValueChanged +=
                    (sender, args) => ability.AbilityLevelIgnore = args.GetNewValue<Slider>().Value;
                ability.AbilityLevelIgnore = abilityLevelIgnore.GetValue<Slider>().Value;
                await Task.Delay(100);

                abilityPriority.ValueChanged += (sender, args) => {
                    var changer = args.GetNewValue<PriorityChanger>();
                    ability.Priority.Clear();

                    foreach (var item in
                        changer.Dictionary.OrderByDescending(x => x.Value)
                            .Select(x => x.Key)
                            .Where(x => abilityToggler.GetValue<AbilityToggler>().IsEnabled(x)))
                    {
                        switch (item)
                        {
                            case "item_sheepstick":
                                ability.Priority.Add(EvadePriority.Disable);
                                break;
                            case "item_cyclone":
                                ability.Priority.Add(EvadePriority.Counter);
                                break;
                            case "item_blink":
                                ability.Priority.Add(EvadePriority.Blink);
                                break;
                            case "centaur_stampede":
                                ability.Priority.Add(EvadePriority.Walk);
                                break;
                        }
                    }

                    Debugger.Write(ability.Name + " priority changed: ");
                    for (var i = 0; i < ability.Priority.Count; i++)
                    {
                        Debugger.Write(ability.Priority.ElementAt(i).ToString(), showType: false);

                        if (ability.Priority.Count - 1 > i)
                        {
                            Debugger.Write(" => ", showType: false);
                        }
                    }

                    Debugger.WriteLine(showType: false);
                };

                abilityToggler.ValueChanged += (sender, args) => {
                    var changer = abilityPriority.GetValue<PriorityChanger>();
                    ability.Priority.Clear();

                    foreach (var item in
                        changer.Dictionary.OrderByDescending(x => x.Value)
                            .Select(x => x.Key)
                            .Where(x => args.GetNewValue<AbilityToggler>().IsEnabled(x)))
                    {
                        switch (item)
                        {
                            case "item_sheepstick":
                                ability.Priority.Add(EvadePriority.Disable);
                                break;
                            case "item_cyclone":
                                ability.Priority.Add(EvadePriority.Counter);
                                break;
                            case "item_blink":
                                ability.Priority.Add(EvadePriority.Blink);
                                break;
                            case "centaur_stampede":
                                ability.Priority.Add(EvadePriority.Walk);
                                break;
                        }
                    }

                    Debugger.Write(ability.Name + " priority changed: ");
                    for (var i = 0; i < ability.Priority.Count; i++)
                    {
                        Debugger.Write(ability.Priority.ElementAt(i).ToString(), showType: false);

                        if (ability.Priority.Count - 1 > i)
                        {
                            Debugger.Write(" => ", showType: false);
                        }
                    }

                    Debugger.WriteLine(showType: false);
                };

                var hpIgnore =
                    new MenuItem(menuItemName + "hpIgnore", "Ignore if ally has more hp than").SetValue(
                        new Slider(0, 0, 1000)).SetTooltip("If value is 0 check will be ignored");
                abilityMenu.AddItem(hpIgnore);
                hpIgnore.ValueChanged += (sender, args) => ability.AllyHpIgnore = args.GetNewValue<Slider>().Value;
                ability.AllyHpIgnore = hpIgnore.GetValue<Slider>().Value;
                await Task.Delay(100);

                //var mpIgnore =
                //    new MenuItem(menuName + "mpIgnore", "Ignore if your hero has less mp than").SetValue(
                //        new Slider(0, 0, 1000)).SetTooltip("If value is 0 check will be ignored");
                //await Task.Delay(100);
                //mpIgnore.ValueChanged += (sender, args) =>
                //{
                //    ability.HeroMpIgnore = args.GetNewValue<Slider>().Value;
                //};
                //ability.HeroMpIgnore = mpIgnore.GetValue<Slider>().Value;
                //abilityMenu.AddItem(mpIgnore);

                var abilityChanger = abilityPriority.GetValue<PriorityChanger>();
                foreach (var priority in
                    abilityChanger.Dictionary.OrderByDescending(x => x.Value)
                        .Select(x => x.Key)
                        .Where(x => abilityToggler.GetValue<AbilityToggler>().IsEnabled(x)))
                {
                    switch (priority)
                    {
                        case "item_sheepstick":
                            ability.Priority.Add(EvadePriority.Disable);
                            break;
                        case "item_cyclone":
                            ability.Priority.Add(EvadePriority.Counter);
                            break;
                        case "item_blink":
                            ability.Priority.Add(EvadePriority.Blink);
                            break;
                        case "centaur_stampede":
                            ability.Priority.Add(EvadePriority.Walk);
                            break;
                    }
                }
            }

            heroMenu.AddSubMenu(abilityMenu);
            addedAbilities.Add(menuItemName);
        }

        #endregion
    }
}