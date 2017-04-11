namespace Evader.Common
{
    using Core;

    using Ensage;
    using Ensage.Common.Extensions;

    using SharpDX;

    internal static class MapDrawer
    {
        private static Hero Hero => Variables.Hero;

        private static Pathfinder Pathfinder => Variables.Pathfinder;

        public static void Draw()
        {
            var center = Game.MousePosition;
            const int CellCount = 40;
            for (var i = 0; i < CellCount; ++i)
            {
                for (var j = 0; j < CellCount; ++j)
                {
                    Vector2 p;
                    p.X = Pathfinder.Pathfinding.CellSize * (i - CellCount / 2) + center.X;
                    p.Y = Pathfinder.Pathfinding.CellSize * (j - CellCount / 2) + center.Y;

                    int heroX, heroY;
                    Pathfinder.Pathfinding.GetCellPosition(center, out heroX, out heroY);
                    Color c;

                    var isFlying = Hero.MoveCapability == MoveCapability.Fly || Hero.IsUnitState(UnitState.Flying);
                    var flag = Pathfinder.Pathfinding.GetCellFlags(p);
                    if (!isFlying && flag.HasFlag(NavMeshCellFlags.Walkable))
                    {
                        c = flag.HasFlag(NavMeshCellFlags.Tree) ? Color.Purple : Color.Green;
                        if (flag.HasFlag(NavMeshCellFlags.GridFlagObstacle))
                        {
                            c = Color.Pink;
                        }
                    }

                    else if (isFlying && !flag.HasFlag(NavMeshCellFlags.MovementBlocker))
                    {
                        c = Color.Green;
                    }
                    else
                    {
                        c = Color.Red;
                    }

                    Drawing.DrawRect(new Vector2(i * 10, 50 + (CellCount - j - 1) * 10), new Vector2(9), c, false);
                }
            }

            int heroCellX, heroCellY;
            Pathfinder.Pathfinding.GetCellPosition(center, out heroCellX, out heroCellY);

            int x, y;
            //Pathfinder.Pathfinding.GetCellPosition(Hero.InFront(150) - center, out x, out y);
            //x += CellCount / 2;
            //y += CellCount / 2;
            //Drawing.DrawRect(new Vector2(x * 10, 50 + (CellCount - y - 1) * 10), new Vector2(9), Color.AliceBlue, false);

            Pathfinder.Pathfinding.GetCellPosition(new Vector3(), out x, out y);
            x += CellCount / 2;
            y += CellCount / 2;
            Drawing.DrawRect(new Vector2(x * 10, 50 + (CellCount - y - 1) * 10), new Vector2(9), Color.Orange, false);

            Pathfinder.Pathfinding.GetCellPosition(Hero.Position - center, out x, out y);
            x += CellCount / 2;
            y += CellCount / 2;
            Drawing.DrawRect(new Vector2(x * 10, 50 + (CellCount - y - 1) * 10), new Vector2(9), Color.Blue, false);
        }
    }
}