namespace Evader.Core
{
    using System.Collections.Generic;
    using System.Linq;

    using Ensage.Common.Menu;

    using EvadableAbilities.Base;
    using EvadableAbilities.Base.Interfaces;

    using Utils;

    internal class MenuManager
    {
        #region Fields

        private readonly Dictionary<string, bool> blinkAbilities = new Dictionary<string, bool>();

        private readonly MenuItem blockPlayerMovement;

        private readonly Dictionary<string, bool> counterAbilities = new Dictionary<string, bool>();

        private readonly MenuItem debugAbilities;

        private readonly MenuItem debugConsoleIntersection;

        private readonly MenuItem debugConsoleModifiers;

        private readonly MenuItem debugConsoleParticles;

        private readonly MenuItem debugConsoleProjectiles;

        private readonly MenuItem debugConsoleRandom;

        private readonly MenuItem debugConsoleUnits;

        private readonly MenuItem debugMap;

        private readonly MenuItem defaultPriority;

        private readonly MenuItem defaultToggler;

        private readonly Dictionary<string, bool> disableAbilities = new Dictionary<string, bool>();

        private readonly MenuItem enabled;

        private readonly MenuItem enabledPathfinder;

        private readonly MenuItem helpAllies;

        private readonly Menu menu;

        //private readonly MenuItem mouseEmulation;

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
            var hotkeys = new Menu("Hotkeys", "hotkeys");

            hotkeys.AddItem(
                enabled = new MenuItem("enable", "Enable evader").SetValue(new KeyBind(36, KeyBindType.Toggle, true)))
                .ValueChanged += (sender, args) => { Enabled = args.GetNewValue<KeyBind>().Active; };
            hotkeys.AddItem(
                enabledPathfinder =
                new MenuItem("enablePathfinder", "Enable pathfinder").SetValue(
                    new KeyBind(35, KeyBindType.Toggle, true))).ValueChanged +=
                (sender, args) => { EnabledPathfinder = args.GetNewValue<KeyBind>().Active; };
            hotkeys.AddItem(
                new MenuItem("forceBlink", "Force blink").SetValue(new KeyBind(46, KeyBindType.Press))
                    .SetTooltip("Blink in front of your hero")).ValueChanged +=
                (sender, args) => { ForceBlink = args.GetNewValue<KeyBind>().Active; };

            var usableAbilitiesMenu = new Menu("Abilities", "usableAbilities");
            usableAbilitiesMenu.AddItem(usableBlinkAbilities = new MenuItem("usableBlinkAbilities", "Blink:"));
            usableAbilitiesMenu.AddItem(usableCounterAbilities = new MenuItem("usableCounterAbilities", "Counter:"));
            usableAbilitiesMenu.AddItem(usableDiasbleAbilities = new MenuItem("usableDiasbleAbilities", "Disable:"));

            // temp =>
            menu.AddItem(
                defaultPriority =
                new MenuItem("defaultPriorityFix", "Default priority").SetValue(
                    new PriorityChanger(
                    new List<string>
                        {
                            "item_sheepstick",
                            "item_cyclone",
                            "item_blink",
                            "centaur_stampede"
                        },
                    "defaultPriorityChangerFix")));

            defaultPriority.ValueChanged += (sender, args) =>
                {
                    var changer = args.GetNewValue<PriorityChanger>();
                    DefaultPriority.Clear();

                    foreach (var item in
                        changer.Dictionary.OrderByDescending(x => x.Value)
                            .Select(x => x.Key)
                            .Where(x => defaultToggler.GetValue<AbilityToggler>().IsEnabled(x)))
                    {
                        switch (item)
                        {
                            case "item_sheepstick":
                                DefaultPriority.Add(Priority.Disable);
                                break;
                            case "item_cyclone":
                                DefaultPriority.Add(Priority.Counter);
                                break;
                            case "item_blink":
                                DefaultPriority.Add(Priority.Blink);
                                break;
                            case "centaur_stampede":
                                DefaultPriority.Add(Priority.Walk);
                                break;
                        }
                    }

                    Debugger.Write("Priority changed: ");
                    for (var i = 0; i < DefaultPriority.Count; i++)
                    {
                        Debugger.Write(DefaultPriority.ElementAt(i).ToString());

                        if (DefaultPriority.Count - 1 > i)
                        {
                            Debugger.Write(" => ");
                        }
                    }

                    Debugger.WriteLine();
                };

            menu.AddItem(
                defaultToggler =
                new MenuItem("defaultTogglerFix", "Enabled priority").SetValue(
                    new AbilityToggler(
                    new Dictionary<string, bool>
                        {
                            { "item_sheepstick", false },
                            { "item_cyclone", true },
                            { "item_blink", true },
                            { "centaur_stampede", true }
                        })));

            defaultToggler.ValueChanged += (sender, args) =>
                {
                    var changer = defaultPriority.GetValue<PriorityChanger>();
                    DefaultPriority.Clear();

                    foreach (var item in
                        changer.Dictionary.OrderByDescending(x => x.Value)
                            .Select(x => x.Key)
                            .Where(x => args.GetNewValue<AbilityToggler>().IsEnabled(x)))
                    {
                        switch (item)
                        {
                            case "item_sheepstick":
                                DefaultPriority.Add(Priority.Disable);
                                break;
                            case "item_cyclone":
                                DefaultPriority.Add(Priority.Counter);
                                break;
                            case "item_blink":
                                DefaultPriority.Add(Priority.Blink);
                                break;
                            case "centaur_stampede":
                                DefaultPriority.Add(Priority.Walk);
                                break;
                        }
                    }

                    Debugger.Write("Priority changed: ");
                    for (var i = 0; i < DefaultPriority.Count; i++)
                    {
                        Debugger.Write(DefaultPriority.ElementAt(i).ToString());

                        if (DefaultPriority.Count - 1 > i)
                        {
                            Debugger.Write(" => ");
                        }
                    }

                    Debugger.WriteLine();
                };
            // <= temp

            //menu.AddItem(
            //    defaultPriority =
            //    new MenuItem("defaultPriority", "Default priority").SetValue(
            //        new PriorityChanger(
            //        new List<string>
            //            {
            //                "item_sheepstick",
            //                "item_cyclone",
            //                "item_blink",
            //                "centaur_stampede"
            //            },
            //        new AbilityToggler(
            //        new Dictionary<string, bool>
            //            {
            //                { "item_sheepstick", false },
            //                { "item_cyclone", true },
            //                { "item_blink", true },
            //                { "centaur_stampede", true }
            //            }),
            //        "defaultPriorityChanger"))).ValueChanged += (sender, args) =>
            //            {
            //                var changer = args.GetNewValue<PriorityChanger>();
            //                DefaultPriority.Clear();

            //                foreach (var item in
            //                    changer.Dictionary.Select(x => x.Key)
            //                        .Where(x => changer.AbilityToggler.IsEnabled(x))
            //                        .Reverse())
            //                {
            //                    switch (item)
            //                    {
            //                        case "item_sheepstick":
            //                        DefaultPriority.Add(Priority.Disable);
            //                        break;
            //                        case "item_cyclone":
            //                        DefaultPriority.Add(Priority.Counter);
            //                        break;
            //                        case "item_blink":
            //                        DefaultPriority.Add(Priority.Blink);
            //                        break;
            //                        case "centaur_stampede":
            //                        DefaultPriority.Add(Priority.Walk);
            //                        break;
            //                    }
            //                }

            //                Debugger.Write("Priority changed: ");
            //                for (var i = 0; i < DefaultPriority.Count; i++)
            //                {
            //                    Debugger.Write(DefaultPriority.ElementAt(i).ToString());

            //                    if (DefaultPriority.Count - 1 > i)
            //                    {
            //                        Debugger.Write(" => ");
            //                    }
            //                }

            //                Debugger.WriteLine();
            //            };

            menu.AddItem(helpAllies = new MenuItem("helpAllies", "Help allies").SetValue(false)).ValueChanged +=
                (sender, args) => { HelpAllies = args.GetNewValue<bool>(); };
            menu.AddItem(
                blockPlayerMovement =
                new MenuItem("blockPlayerMovement", "Block player movement").SetValue(true)
                    .SetTooltip("Player movement will be blocked while avoiding ability")).ValueChanged +=
                (sender, args) => { BlockPlayerMovement = args.GetNewValue<bool>(); };

            //todo add ?
            //menu.AddItem(mouseEmulation = new MenuItem("mouseEmulation", "Mouse emulation").SetValue(false))
            //    .ValueChanged += (sender, args) =>
            //        {
            //            MouseEmulation = args.GetNewValue<bool>();
            //        };

            settingsMenu = new Menu("Enemy settings", "settings");

            var debugMenu = new Menu("Debug", "debug");
            debugMenu.AddItem(debugAbilities = new MenuItem("debugAbilities", "Draw abilities").SetValue(false))
                .ValueChanged += (sender, args) => { DebugAbilities = args.GetNewValue<bool>(); };
            debugMenu.AddItem(debugMap = new MenuItem("debugMap", "Draw map").SetValue(false)).ValueChanged +=
                (sender, args) => { DebugMap = args.GetNewValue<bool>(); };
            debugMenu.AddItem(debugConsoleRandom = new MenuItem("debugConsoleRandom", "Console other").SetValue(false))
                .ValueChanged += (sender, args) => { DebugConsoleRandom = args.GetNewValue<bool>(); };
            debugMenu.AddItem(
                debugConsoleIntersection =
                new MenuItem("debugConsoleIntersection", "Console intersections").SetValue(false)).ValueChanged +=
                (sender, args) => { DebugConsoleIntersection = args.GetNewValue<bool>(); };
            debugMenu.AddItem(
                debugConsoleModifiers = new MenuItem("debugConsoleModifiers", "Console modifiers").SetValue(false))
                .ValueChanged += (sender, args) => { DebugConsoleModifiers = args.GetNewValue<bool>(); };
            debugMenu.AddItem(
                debugConsoleParticles = new MenuItem("debugConsoleParticles", "Console particles").SetValue(false))
                .ValueChanged += (sender, args) => { DebugConsoleParticles = args.GetNewValue<bool>(); };
            debugMenu.AddItem(
                debugConsoleProjectiles = new MenuItem("debugConsoleProjectiles", "Console projectiles").SetValue(false))
                .ValueChanged += (sender, args) => { DebugConsoleProjectiles = args.GetNewValue<bool>(); };
            debugMenu.AddItem(debugConsoleUnits = new MenuItem("debugConsoleUnits", "Console units").SetValue(false))
                .ValueChanged += (sender, args) => { DebugConsoleUnits = args.GetNewValue<bool>(); };

            menu.AddSubMenu(settingsMenu);
            menu.AddSubMenu(usableAbilitiesMenu);
            menu.AddSubMenu(hotkeys);
            menu.AddSubMenu(debugMenu);
            menu.AddToMainMenu();

            InitializeValues();
        }

        #endregion

        #region Public Properties

        public bool BlockPlayerMovement { get; private set; }

        public bool DebugAbilities { get; private set; }

        public bool DebugConsoleIntersection { get; private set; }

        public bool DebugConsoleModifiers { get; private set; }

        public bool DebugConsoleParticles { get; private set; }

        public bool DebugConsoleProjectiles { get; private set; }

        public bool DebugConsoleRandom { get; private set; }

        public bool DebugConsoleUnits { get; private set; }

        public bool DebugMap { get; private set; }

        public List<Priority> DefaultPriority { get; } = new List<Priority>();

        public bool Enabled { get; private set; }

        public bool EnabledPathfinder { get; private set; }

        public bool ForceBlink { get; private set; }

        public bool HelpAllies { get; private set; }

        public bool MouseEmulation { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void AddEvadableAbility(EvadableAbility ability)
        {
            Menu heroMenu;
            var ownerName = ability.AbilityOwner.Name;
            var abilityName = ability.Name;

            if (!unitMenus.TryGetValue(ownerName, out heroMenu))
            {
                heroMenu = new Menu(string.Empty, ownerName, textureName: ownerName);
                settingsMenu.AddSubMenu(heroMenu);
                unitMenus.Add(ownerName, heroMenu);
            }

            var abilityMenu = new Menu(string.Empty, ownerName + abilityName, false, abilityName, true);
            var abilityEnabled = new MenuItem(ownerName + abilityName + "enabled", "Enabled").SetValue(true);
            var customPriority =
                new MenuItem(ownerName + abilityName + "customPriority", "Use custom priority").SetValue(false);

            abilityEnabled.ValueChanged += (sender, args) => { ability.Enabled = args.GetNewValue<bool>(); };
            customPriority.ValueChanged += (sender, args) => { ability.UseCustomPriority = args.GetNewValue<bool>(); };

            // temp =>
            var abilityPriority =
                new MenuItem(ownerName + abilityName + "priorityFix", "Custom priority").SetValue(
                    new PriorityChanger(
                        new List<string>
                            {
                                "item_sheepstick",
                                "item_cyclone",
                                "item_blink",
                                "centaur_stampede"
                            },
                        ownerName + abilityName + "changerFix"));

            var abilityToggler =
                new MenuItem(ownerName + abilityName + "togglerFix", "Custom enabled priority").SetValue(
                    new AbilityToggler(
                        new Dictionary<string, bool>
                            {
                                { "item_sheepstick", false },
                                { "item_cyclone", true },
                                { "item_blink", true },
                                { "centaur_stampede", true }
                            }));

            abilityPriority.ValueChanged += (sender, args) =>
                {
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
                                ability.Priority.Add(Priority.Disable);
                                break;
                            case "item_cyclone":
                                ability.Priority.Add(Priority.Counter);
                                break;
                            case "item_blink":
                                ability.Priority.Add(Priority.Blink);
                                break;
                            case "centaur_stampede":
                                ability.Priority.Add(Priority.Walk);
                                break;
                        }
                    }

                    Debugger.Write(ability.Name + " priority changed: ");
                    for (var i = 0; i < ability.Priority.Count; i++)
                    {
                        Debugger.Write(ability.Priority.ElementAt(i).ToString());

                        if (ability.Priority.Count - 1 > i)
                        {
                            Debugger.Write(" => ");
                        }
                    }

                    Debugger.WriteLine();
                };

            abilityToggler.ValueChanged += (sender, args) =>
                {
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
                                ability.Priority.Add(Priority.Disable);
                                break;
                            case "item_cyclone":
                                ability.Priority.Add(Priority.Counter);
                                break;
                            case "item_blink":
                                ability.Priority.Add(Priority.Blink);
                                break;
                            case "centaur_stampede":
                                ability.Priority.Add(Priority.Walk);
                                break;
                        }
                    }

                    Debugger.Write(ability.Name + " priority changed: ");
                    for (var i = 0; i < ability.Priority.Count; i++)
                    {
                        Debugger.Write(ability.Priority.ElementAt(i).ToString());

                        if (ability.Priority.Count - 1 > i)
                        {
                            Debugger.Write(" => ");
                        }
                    }

                    Debugger.WriteLine();
                };
            // <= temp

            //var abilityPriority =
            //    new MenuItem(ownerName + abilityName + "priority", "Custom priority").SetValue(
            //        new PriorityChanger(
            //            new List<string>
            //                {
            //                    "item_sheepstick",
            //                    "item_cyclone",
            //                    "item_blink",
            //                    "centaur_stampede"
            //                },
            //            new AbilityToggler(
            //                new Dictionary<string, bool>
            //                    {
            //                        { "item_sheepstick", false },
            //                        { "item_cyclone", true },
            //                        { "item_blink", true },
            //                        { "centaur_stampede", true }
            //                    }),
            //            ownerName + abilityName + "priorityChanger"));

            //abilityPriority.ValueChanged += (sender, args) =>
            //    {
            //        var changer = args.GetNewValue<PriorityChanger>();
            //        ability.Priority.Clear();

            //        foreach (var item in
            //            changer.Dictionary.Select(x => x.Key).Where(x => changer.AbilityToggler.IsEnabled(x)).Reverse())
            //        {
            //            switch (item)
            //            {
            //                case "item_sheepstick":
            //                    ability.Priority.Add(Priority.Disable);
            //                    break;
            //                case "item_cyclone":
            //                    ability.Priority.Add(Priority.Counter);
            //                    break;
            //                case "item_blink":
            //                    ability.Priority.Add(Priority.Blink);
            //                    break;
            //                case "centaur_stampede":
            //                    ability.Priority.Add(Priority.Walk);
            //                    break;
            //            }
            //        }

            //        Debugger.Write(ability.Name + " priority changed: ");
            //        for (var i = 0; i < ability.Priority.Count; i++)
            //        {
            //            Debugger.Write(ability.Priority.ElementAt(i).ToString());

            //            if (ability.Priority.Count - 1 > i)
            //            {
            //                Debugger.Write(" => ");
            //            }
            //        }

            //        Debugger.WriteLine();
            //    };

            ability.Enabled = abilityEnabled.IsActive();
            ability.UseCustomPriority = customPriority.GetValue<bool>();

            var abilityChanger = abilityPriority.GetValue<PriorityChanger>();

            // temp =>
            foreach (var priority in
                abilityChanger.Dictionary.OrderByDescending(x => x.Value)
                    .Select(x => x.Key)
                    .Where(x => abilityToggler.GetValue<AbilityToggler>().IsEnabled(x)))
            {
                switch (priority)
                {
                    case "item_sheepstick":
                        ability.Priority.Add(Priority.Disable);
                        break;
                    case "item_cyclone":
                        ability.Priority.Add(Priority.Counter);
                        break;
                    case "item_blink":
                        ability.Priority.Add(Priority.Blink);
                        break;
                    case "centaur_stampede":
                        ability.Priority.Add(Priority.Walk);
                        break;
                }
            }
            // <= temp

            //foreach (var priority in
            //    abilityChanger.Dictionary.Select(x => x.Key)
            //        .Where(x => abilityChanger.AbilityToggler.IsEnabled(x))
            //        .Reverse())
            //{
            //    switch (priority)
            //    {
            //        case "item_sheepstick":
            //            ability.Priority.Add(Priority.Disable);
            //            break;
            //        case "item_cyclone":
            //            ability.Priority.Add(Priority.Counter);
            //            break;
            //        case "item_blink":
            //            ability.Priority.Add(Priority.Blink);
            //            break;
            //        case "centaur_stampede":
            //            ability.Priority.Add(Priority.Walk);
            //            break;
            //    }
            //}

            abilityMenu.AddItem(abilityEnabled);

            ////////if (ability is IModifier)
            ////////{
            ////////    var modiferCounter = new MenuItem(ownerName + abilityName + "modifier", "Modifer counter").SetValue(
            ////////        true);
            ////////    modiferCounter.ValueChanged +=
            ////////        (sender, args) => { ability.ModifierCounterEnabled = args.GetNewValue<bool>(); };
            ////////    abilityMenu.AddItem(modiferCounter);
            ////////    ability.ModifierCounterEnabled = modiferCounter.IsActive();

            ////////    abilityMenu.DisplayName = "  *";
            ////////}

            abilityMenu.AddItem(customPriority);
            abilityMenu.AddItem(abilityPriority);

            // temp =>
            abilityMenu.AddItem(abilityToggler);
            // <= temp

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

        public bool UsableAbilityEnabled(string abilityName, AbilityType type)
        {
            var abilityEnabled = false;

            switch (type)
            {
                case AbilityType.Counter:
                    counterAbilities.TryGetValue(abilityName, out abilityEnabled);
                    break;
                case AbilityType.Blink:
                    blinkAbilities.TryGetValue(abilityName, out abilityEnabled);
                    break;
                case AbilityType.Disable:
                    disableAbilities.TryGetValue(abilityName, out abilityEnabled);
                    break;
            }

            return abilityEnabled;
        }

        #endregion

        #region Methods

        private void InitializeValues()
        {
            Enabled = enabled.IsActive();
            EnabledPathfinder = enabledPathfinder.IsActive();
            HelpAllies = helpAllies.IsActive();
            BlockPlayerMovement = blockPlayerMovement.IsActive();
            DebugMap = debugMap.IsActive();
            DebugAbilities = debugAbilities.IsActive();
            DebugConsoleIntersection = debugConsoleIntersection.IsActive();
            DebugConsoleRandom = debugConsoleRandom.IsActive();
            DebugConsoleModifiers = debugConsoleModifiers.IsActive();
            DebugConsoleParticles = debugConsoleParticles.IsActive();
            DebugConsoleProjectiles = debugConsoleProjectiles.IsActive();
            DebugConsoleUnits = debugConsoleUnits.IsActive();
            // MouseEmulation = mouseEmulation.IsActive();

            var changer = defaultPriority.GetValue<PriorityChanger>();

            // temp =>
            foreach (var priority in
                changer.Dictionary.OrderByDescending(x => x.Value)
                    .Select(x => x.Key)
                    .Where(x => defaultToggler.GetValue<AbilityToggler>().IsEnabled(x)))
            {
                switch (priority)
                {
                    case "item_sheepstick":
                        DefaultPriority.Add(Priority.Disable);
                        break;
                    case "item_cyclone":
                        DefaultPriority.Add(Priority.Counter);
                        break;
                    case "item_blink":
                        DefaultPriority.Add(Priority.Blink);
                        break;
                    case "centaur_stampede":
                        DefaultPriority.Add(Priority.Walk);
                        break;
                }
            }
            // <= temp

            //foreach (
            //    var priority in
            //        changer.Dictionary.Select(x => x.Key).Where(x => changer.AbilityToggler.IsEnabled(x)).Reverse())
            //{
            //    switch (priority)
            //    {
            //        case "item_sheepstick":
            //            DefaultPriority.Add(Priority.Disable);
            //            break;
            //        case "item_cyclone":
            //            DefaultPriority.Add(Priority.Counter);
            //            break;
            //        case "item_blink":
            //            DefaultPriority.Add(Priority.Blink);
            //            break;
            //        case "centaur_stampede":
            //            DefaultPriority.Add(Priority.Walk);
            //            break;
            //    }
            //}
        }

        #endregion
    }
}