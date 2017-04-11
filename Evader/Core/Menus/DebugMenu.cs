namespace Evader.Core.Menus
{
    using Ensage.Common.Menu;

    using SharpDX;

    internal class DebugMenu
    {
        public DebugMenu(Menu rootMenu)
        {
            var menu = new Menu("Debug", "debug").SetFontColor(Color.PaleVioletRed);

            var drawAbilities = new MenuItem("debugAbilities", "Draw abilities").SetValue(false)
                .SetTooltip("Draw enemy ability ranges, timings and projectiles when casted");
            menu.AddItem(drawAbilities);
            drawAbilities.ValueChanged += (sender, args) => DrawAbilities = args.GetNewValue<bool>();
            DrawAbilities = drawAbilities.IsActive();

            var drawMap = new MenuItem("debugDrawMap", "Draw obstacles map").SetValue(false);
            menu.AddItem(drawMap);
            drawMap.ValueChanged += (sender, args) => DrawMap = args.GetNewValue<bool>();
            DrawMap = drawMap.IsActive();

            var logAbilityUsage = new MenuItem("debugLogAbilityUsage", "Log ability usage").SetValue(false);
            menu.AddItem(logAbilityUsage);
            logAbilityUsage.ValueChanged += (sender, args) => LogAbilityUsage = args.GetNewValue<bool>();
            LogAbilityUsage = logAbilityUsage.IsActive();

            var logInformation = new MenuItem("debugLogInformation", "Log information").SetValue(false);
            menu.AddItem(logInformation);
            logInformation.ValueChanged += (sender, args) => LogInformation = args.GetNewValue<bool>();
            LogInformation = logInformation.IsActive();

            var logIntersection = new MenuItem("debugLogIntersection", "Log intersections").SetValue(false);
            menu.AddItem(logIntersection);
            logIntersection.ValueChanged += (sender, args) => LogIntersection = args.GetNewValue<bool>();
            LogIntersection = logIntersection.IsActive();

            var armletToggler = new MenuItem("debugArmlet", "Armlet toggler").SetValue(false);
            menu.AddItem(armletToggler);
            armletToggler.ValueChanged += (sender, args) => ArmletToggler = args.GetNewValue<bool>();
            ArmletToggler = armletToggler.IsActive();

            var fastAbilityAdd = new MenuItem("fastAbilityAdd", "Fast ability add").SetValue(false);
            fastAbilityAdd.SetTooltip("Don't enable this");
            menu.AddItem(fastAbilityAdd);
            fastAbilityAdd.ValueChanged += (sender, args) => FastAbilityAdd = args.GetNewValue<bool>();
            FastAbilityAdd = fastAbilityAdd.IsActive();

            rootMenu.AddSubMenu(menu);
        }

        public bool ArmletToggler { get; private set; }

        public bool DrawAbilities { get; private set; }

        public bool DrawMap { get; private set; }

        public bool FastAbilityAdd { get; private set; }

        public bool LogAbilityUsage { get; private set; }

        public bool LogInformation { get; private set; }

        public bool LogIntersection { get; private set; }
    }
}