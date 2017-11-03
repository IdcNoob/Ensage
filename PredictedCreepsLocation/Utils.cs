namespace PredictedCreepsLocation
{
    using System;

    using Ensage;
    using Ensage.SDK.Extensions;

    using SharpDX;

    public enum LanePosition
    {
        Easy,

        Middle,

        Hard
    }

    internal static class Utils
    {
        public static float GetLaneDelay(LanePosition lane, Team team)
        {
            switch (lane)
            {
                case LanePosition.Easy:
                    return team == Team.Radiant ? 0.25f : 0.3f;
                case LanePosition.Middle:
                    return team == Team.Radiant ? 0.5f : 0.4f;
                case LanePosition.Hard:
                    return team == Team.Radiant ? 0.5f : 0.8f;
                default:
                    return 0;
            }
        }

        // copy pasta :kappajail:
        public static Vector3 PositionAfter(this Vector3[] path, float time, float speed, float delay = 0)
        {
            var distance = Math.Max(0, time - delay) * speed;
            for (var i = 0; i <= path.Length - 2; i++)
            {
                var from = path[i];
                var to = path[i + 1];
                var d = to.Distance(from);
                if (d > distance)
                {
                    return from + (distance * (to - @from).Normalized());
                }

                distance -= d;
            }

            return path[path.Length - 1];
        }
    }
}