namespace Evader.Core
{
    using Ensage;

    using Menus;

    internal static class Variables
    {
        #region Public Properties

        public static Hero Hero { get; set; }

        public static Team HeroTeam { get; set; }

        public static MenuManager Menu { get; set; }

        public static Pathfinder Pathfinder { get; set; }

        #endregion
    }
}