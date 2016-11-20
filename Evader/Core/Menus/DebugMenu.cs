namespace Evader.Core.Menus
{
    using Ensage.Common.Menu;

    using SharpDX;

    internal class DebugMenu
    {
        #region Constructors and Destructors

        public DebugMenu(Menu rootMenu)
        {
            var menu = new Menu("Debug", "debug").SetFontColor(Color.PaleVioletRed);

            var drawAbilities =
                new MenuItem("debugAbilities", "Draw abilities").SetValue(false)
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

            var logModifiers = new MenuItem("debugLogModifiers", "Log modifiers").SetValue(false);
            menu.AddItem(logModifiers);
            logModifiers.ValueChanged += (sender, args) => LogModifiers = args.GetNewValue<bool>();
            LogModifiers = logModifiers.IsActive();

            var logParticles = new MenuItem("debugLogParticles", "Log particles").SetValue(false);
            menu.AddItem(logParticles);
            logParticles.ValueChanged += (sender, args) => LogParticles = args.GetNewValue<bool>();
            LogParticles = logParticles.IsActive();

            var logProjectiles = new MenuItem("debugLogProjectiles", "Log projectiles").SetValue(false);
            menu.AddItem(logProjectiles);
            logProjectiles.ValueChanged += (sender, args) => LogProjectiles = args.GetNewValue<bool>();
            LogProjectiles = logProjectiles.IsActive();

            var logUnits = new MenuItem("debugLogUnits", "Log units").SetValue(false);
            menu.AddItem(logUnits);
            logUnits.ValueChanged += (sender, args) => { LogUnits = args.GetNewValue<bool>(); };
            LogUnits = logUnits.IsActive();

            rootMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Properties

        public bool DrawAbilities { get; private set; }

        public bool DrawMap { get; private set; }

        public bool LogAbilityUsage { get; private set; }

        public bool LogInformation { get; private set; }

        public bool LogIntersection { get; private set; }

        public bool LogModifiers { get; private set; }

        public bool LogParticles { get; private set; }

        public bool LogProjectiles { get; private set; }

        public bool LogUnits { get; private set; }

        #endregion
    }
}