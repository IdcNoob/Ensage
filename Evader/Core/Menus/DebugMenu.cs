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

            var drawMap = new MenuItem("debugMap", "Draw obstacles map").SetValue(false);
            menu.AddItem(drawMap);
            drawMap.ValueChanged += (sender, args) => DrawMap = args.GetNewValue<bool>();
            DrawMap = drawMap.IsActive();

            var logRandom = new MenuItem("debugConsoleRandom", "Log random").SetValue(false);
            menu.AddItem(logRandom);
            logRandom.ValueChanged += (sender, args) => LogRandom = args.GetNewValue<bool>();
            LogRandom = logRandom.IsActive();

            var logIntersection = new MenuItem("debugConsoleIntersection", "Log intersections").SetValue(false);
            menu.AddItem(logIntersection);
            logIntersection.ValueChanged += (sender, args) => LogIntersection = args.GetNewValue<bool>();
            LogIntersection = logIntersection.IsActive();

            var logModifiers = new MenuItem("debugConsoleModifiers", "Log modifiers").SetValue(false);
            menu.AddItem(logModifiers);
            logModifiers.ValueChanged += (sender, args) => LogModifiers = args.GetNewValue<bool>();
            LogModifiers = logModifiers.IsActive();

            var logParticles = new MenuItem("debugConsoleParticles", "Log particles").SetValue(false);
            menu.AddItem(logParticles);
            logParticles.ValueChanged += (sender, args) => LogParticles = args.GetNewValue<bool>();
            LogParticles = logParticles.IsActive();

            var logProjectiles = new MenuItem("debugConsoleProjectiles", "Log projectiles").SetValue(false);
            menu.AddItem(logProjectiles);
            logProjectiles.ValueChanged += (sender, args) => LogProjectiles = args.GetNewValue<bool>();
            LogProjectiles = logProjectiles.IsActive();

            var logUnits = new MenuItem("debugConsoleUnits", "Log units").SetValue(false);
            menu.AddItem(logUnits);
            logUnits.ValueChanged += (sender, args) => { LogUnits = args.GetNewValue<bool>(); };
            LogUnits = logUnits.IsActive();

            rootMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Properties

        public bool DrawAbilities { get; private set; }

        public bool DrawMap { get; private set; }

        public bool LogIntersection { get; private set; }

        public bool LogModifiers { get; private set; }

        public bool LogParticles { get; private set; }

        public bool LogProjectiles { get; private set; }

        public bool LogRandom { get; private set; }

        public bool LogUnits { get; private set; }

        #endregion
    }
}