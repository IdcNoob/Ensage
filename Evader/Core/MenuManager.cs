namespace Evader.Core
{
    using System.Collections.Generic;
    using System.Linq;

    using Ensage.Common.Menu;

    using EvadableAbilities.Base;

    using Utils;

    internal class MenuManager
    {
        #region Fields

        private readonly Dictionary<string, bool> blinkAbilities = new Dictionary<string, bool>();

        private readonly MenuItem blockPlayerMovement;

        private readonly Dictionary<string, bool> counterAbilities = new Dictionary<string, bool>();

        private readonly MenuItem debugAbilities;

        private readonly MenuItem debugConsoleModifiers;

        private readonly MenuItem debugConsoleParticles;

        private readonly MenuItem debugConsoleProjectiles;

        private readonly MenuItem debugConsoleRandom;

        private readonly MenuItem debugConsoleUnits;

        private readonly MenuItem debugMap;

        private readonly MenuItem defaultPriority;

        private readonly Dictionary<string, bool> disableAbilities = new Dictionary<string, bool>();

        private readonly MenuItem enable;

        private readonly MenuItem helpAllies;

        private readonly Menu menu;

        private readonly MenuItem mouseEmulation;

        private readonly Menu settingsMenu;

        private readonly Dictionary<string, Menu> unitMenus = new Dictionary<string, Menu>();

        private readonly MenuItem usableBlinkAbilities;

        private readonly MenuItem usableCounterAbilities;

        private readonly MenuItem usableDiasbleAbilities;

        #endregion

        #region Constructors and Destructors

        public MenuManager()
        {
            menu = new Menu("Evader", "evader", true, "techies_minefield_sign", true);
            enable = new MenuItem("enable", "Enable").SetValue(new KeyBind(36, KeyBindType.Toggle, true));
            var usableAbilitiesMenu = new Menu("Abilities", "usableAbilities");
            usableAbilitiesMenu.AddItem(usableBlinkAbilities = new MenuItem("usableBlinkAbilities", "Blink:"));
            usableAbilitiesMenu.AddItem(usableCounterAbilities = new MenuItem("usableCounterAbilities", "Counter:"));
            usableAbilitiesMenu.AddItem(usableDiasbleAbilities = new MenuItem("usableDiasbleAbilities", "Disable:"));
            menu.AddItem(enable);

            menu.AddItem(
                defaultPriority =
                new MenuItem("defaultPriority", "Default priority").SetValue(
                    new PriorityChanger(
                    new List<string>
                        {
                            "item_sheepstick",
                            "item_cyclone",
                            "item_blink",
                            "centaur_stampede"
                        },
                    new AbilityToggler(
                    new Dictionary<string, bool>
                        {
                            { "item_sheepstick", false },
                            { "item_cyclone", true },
                            { "item_blink", true },
                            { "centaur_stampede", true }
                        }),
                    "defaultPriorityChanger")));

            defaultPriority.ValueChanged += (sender, args) =>
                {
                    var changer = args.GetNewValue<PriorityChanger>();

                    var list =
                        changer.Dictionary.Select(x => x.Key)
                            .Where(x => changer.AbilityToggler.IsEnabled(x))
                            .Reverse()
                            .ToList();

                    Debugger.Write("Priority changed: ");

                    for (var i = 0; i < list.Count; i++)
                    {
                        switch (list[i])
                        {
                            case "item_sheepstick":
                                Debugger.Write("Disable");
                                break;
                            case "item_cyclone":
                                Debugger.Write("Counter");
                                break;
                            case "item_blink":
                                Debugger.Write("Blink");
                                break;
                            case "centaur_stampede":
                                Debugger.Write("Walk");
                                break;
                        }

                        if (list.Count - 1 > i)
                        {
                            Debugger.Write(" => ");
                        }
                    }

                    Debugger.WriteLine();
                };

            menu.AddItem(helpAllies = new MenuItem("helpAllies", "Help allies").SetValue(false));
            menu.AddItem(
                blockPlayerMovement =
                new MenuItem("blockPlayerMovement", "Block player movement").SetValue(true)
                    .SetTooltip("Player movement will be blocked while avoiding ability"));

            //todo add ?
            // menu.AddItem(mouseEmulation = new MenuItem("mouseEmulation", "Mouse emulation").SetValue(false));

            settingsMenu = new Menu("Enemy settings", "settings");

            var debugMenu = new Menu("Debug", "debug");
            debugMenu.AddItem(debugMap = new MenuItem("debugMap", "Draw map").SetValue(false));
            debugMenu.AddItem(debugAbilities = new MenuItem("debugAbilities", "Draw abilities").SetValue(false));
            debugMenu.AddItem(debugConsoleRandom = new MenuItem("debugConsoleR", "Console other").SetValue(false));
            debugMenu.AddItem(
                debugConsoleModifiers = new MenuItem("debugConsoleM", "Console modifiers").SetValue(false));
            debugMenu.AddItem(
                debugConsoleParticles = new MenuItem("debugConsoleP", "Console particles").SetValue(false));
            debugMenu.AddItem(
                debugConsoleProjectiles = new MenuItem("debugConsolePr", "Console projectiles").SetValue(false));
            debugMenu.AddItem(debugConsoleUnits = new MenuItem("debugConsoleU", "Console units").SetValue(false));

            menu.AddSubMenu(settingsMenu);
            menu.AddSubMenu(usableAbilitiesMenu);
            menu.AddSubMenu(debugMenu);
            menu.AddToMainMenu();
        }

        #endregion

        #region Public Properties

        public bool BlockPlayerMovement => blockPlayerMovement.IsActive();

        public bool DebugAbilities => debugAbilities.IsActive();

        public bool DebugConsoleModifiers => debugConsoleModifiers.IsActive();

        public bool DebugConsoleParticles => debugConsoleParticles.IsActive();

        public bool DebugConsoleProjectiles => debugConsoleProjectiles.IsActive();

        public bool DebugConsoleRandom => debugConsoleRandom.IsActive();

        public bool DebugConsoleUnits => debugConsoleUnits.IsActive();

        public bool DebugMap => debugMap.IsActive();

        public bool Enabled => enable.IsActive();

        public bool HelpAllies => helpAllies.IsActive();

        public bool MouseEmulation => mouseEmulation.IsActive();

        #endregion

        #region Public Methods and Operators

        public void AddEvadableAbility(EvadableAbility ability)
        {
            Menu heroMenu;
            var ownerName = ability.Owner.Name;
            var abilityName = ability.Name;

            if (!unitMenus.TryGetValue(ownerName, out heroMenu))
            {
                heroMenu = new Menu(string.Empty, ownerName, textureName: ownerName);
                settingsMenu.AddSubMenu(heroMenu);
                unitMenus.Add(ownerName, heroMenu);
            }

            var abilityMenu = new Menu(string.Empty, ownerName + abilityName, false, abilityName);
            var abilityEnabled = new MenuItem(ownerName + abilityName + "enabled", "Enabled").SetValue(true);
            var customPriority =
                new MenuItem(ownerName + abilityName + "customPriority", "Use custom priority").SetValue(false);

            var abilityPriority =
                new MenuItem(ownerName + abilityName + "priority", "Custom priority").SetValue(
                    new PriorityChanger(
                        new List<string>
                            {
                                "item_sheepstick",
                                "item_cyclone",
                                "item_blink",
                                "centaur_stampede"
                            },
                        new AbilityToggler(
                            new Dictionary<string, bool>
                                {
                                    { "item_sheepstick", false },
                                    { "item_cyclone", true },
                                    { "item_blink", true },
                                    { "centaur_stampede", true }
                                }),
                        ownerName + abilityName + "priorityChanger"));

            abilityEnabled.ValueChanged += (sender, args) => { ability.Enabled = args.GetNewValue<bool>(); };
            customPriority.ValueChanged += (sender, args) => { ability.UseCustomPriority = args.GetNewValue<bool>(); };
            abilityPriority.ValueChanged += (sender, args) =>
                {
                    var changer = args.GetNewValue<PriorityChanger>();

                    var list =
                        changer.Dictionary.Select(x => x.Key)
                            .Where(x => changer.AbilityToggler.IsEnabled(x))
                            .Reverse()
                            .ToList();

                    Debugger.Write(abilityName + " priority changed: ");

                    for (var i = 0; i < list.Count; i++)
                    {
                        switch (list[i])
                        {
                            case "item_sheepstick":
                                Debugger.Write("Disable");
                                break;
                            case "item_cyclone":
                                Debugger.Write("Counter");
                                break;
                            case "item_blink":
                                Debugger.Write("Blink");
                                break;
                            case "centaur_stampede":
                                Debugger.Write("Walk");
                                break;
                        }

                        if (list.Count - 1 > i)
                        {
                            Debugger.Write(" => ");
                        }
                    }

                    Debugger.WriteLine();

                    ability.PriorityChanger = changer;
                };

            ability.Enabled = abilityEnabled.IsActive();
            ability.UseCustomPriority = customPriority.GetValue<bool>();
            ability.PriorityChanger = abilityPriority.GetValue<PriorityChanger>();

            abilityMenu.AddItem(abilityEnabled);
            abilityMenu.AddItem(customPriority);
            abilityMenu.AddItem(abilityPriority);

            heroMenu.AddSubMenu(abilityMenu);
        }

        public void AddUsableBlinkAbility(string abilityName)
        {
            if (blinkAbilities.ContainsKey(abilityName))
            {
                return;
            }

            blinkAbilities.Add(abilityName, true);
            usableBlinkAbilities.SetValue(new AbilityToggler(blinkAbilities));
        }

        public void AddUsableCounterAbility(string abilityName)
        {
            if (counterAbilities.ContainsKey(abilityName))
            {
                return;
            }

            counterAbilities.Add(abilityName, true);
            usableCounterAbilities.SetValue(new AbilityToggler(counterAbilities));
        }

        public void AddUsableDisableAbility(string abilityName)
        {
            if (disableAbilities.ContainsKey(abilityName))
            {
                return;
            }

            disableAbilities.Add(abilityName, true);
            usableDiasbleAbilities.SetValue(new AbilityToggler(disableAbilities));
        }

        public void Close()
        {
            menu.RemoveFromMainMenu();
        }

        public IEnumerable<Priority> GetDefaultPriority()
        {
            //todo optimize

            var changer = defaultPriority.GetValue<PriorityChanger>();
            var priority = new List<Priority>();

            foreach (var item in
                changer.Dictionary.Select(x => x.Key).Where(x => changer.AbilityToggler.IsEnabled(x)).Reverse())
            {
                switch (item)
                {
                    case "item_sheepstick":
                        priority.Add(Priority.Disable);
                        break;
                    case "item_cyclone":
                        priority.Add(Priority.Counter);
                        break;
                    case "item_blink":
                        priority.Add(Priority.Blink);
                        break;
                    case "centaur_stampede":
                        priority.Add(Priority.Walk);
                        break;
                }
            }

            return priority;
        }

        public bool UsableAbilityEnabled(string abilityName, AbilityType type)
        {
            switch (type)
            {
                case AbilityType.Counter:
                    return usableCounterAbilities.GetValue<AbilityToggler>().IsEnabled(abilityName);
                case AbilityType.Blink:
                    return usableBlinkAbilities.GetValue<AbilityToggler>().IsEnabled(abilityName);
                case AbilityType.Disable:
                    return usableDiasbleAbilities.GetValue<AbilityToggler>().IsEnabled(abilityName);
            }

            return false;
        }

        #endregion
    }
}