namespace Evader.Core
{
    using Ensage;
    using Ensage.Common.Objects.UtilityObjects;

    using Menus;

    internal static class Variables
    {
        public static Team EnemyTeam { get; set; }

        public static Hero Hero { get; set; }

        public static Team HeroTeam { get; set; }

        public static MenuManager Menu { get; set; }

        public static Pathfinder Pathfinder { get; set; }

        public static MultiSleeper Sleeper { get; set; }
    }
}