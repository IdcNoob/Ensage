namespace PredictedCreepsLocation.Data
{
    using System.Collections.Generic;

    using Ensage;
    using Ensage.SDK.Extensions;

    using SharpDX;

    internal class LanePaths
    {
        public LanePaths(Team myTeam)
        {
            var topPath = new List<Vector3>();
            var midPath = new List<Vector3>();
            var botPath = new List<Vector3>();

            if (myTeam == Team.Radiant)
            {
                //dire paths

                topPath.Add(new Vector3(3154, 5811, 384));
                topPath.Add(new Vector3(2903, 5818, 299));
                topPath.Add(new Vector3(-248, 5852, 384));
                topPath.Add(new Vector3(-1585, 5979, 384));
                topPath.Add(new Vector3(-3253, 5981, 384));
                topPath.Add(new Vector3(-3587, 5954, 384));
                topPath.Add(new Vector3(-3954, 5860, 384));
                topPath.Add(new Vector3(-4988, 5618, 384));
                topPath.Add(new Vector3(-5714, 5514, 384));
                topPath.Add(new Vector3(-6051, 5092, 384));
                topPath.Add(new Vector3(-6299, 4171, 384));
                topPath.Add(new Vector3(-6342, 3806, 384));
                topPath.Add(new Vector3(-6419, 2888, 384));
                topPath.Add(new Vector3(-6434, -2797, 256));

                midPath.Add(new Vector3(4101, 3652, 384));
                midPath.Add(new Vector3(3549, 3041, 256));
                midPath.Add(new Vector3(2639, 2061, 256));
                midPath.Add(new Vector3(2091, 1605, 256));
                midPath.Add(new Vector3(1091, 845, 256));
                midPath.Add(new Vector3(-163, -62, 256));
                midPath.Add(new Vector3(-789, -579, 178));
                midPath.Add(new Vector3(-1327, -1017, 256));
                midPath.Add(new Vector3(-2211, -1799, 256));
                midPath.Add(new Vector3(-2988, -2522, 256));
                midPath.Add(new Vector3(-4103, -3532, 256));

                botPath.Add(new Vector3(6239, 3717, 384));
                botPath.Add(new Vector3(6166, 3097, 384));
                botPath.Add(new Vector3(6407, 635, 384));
                botPath.Add(new Vector3(6422, -816, 384));
                botPath.Add(new Vector3(6477, -2134, 384));
                botPath.Add(new Vector3(5995, -3054, 384));
                botPath.Add(new Vector3(6071, -3601, 384));
                botPath.Add(new Vector3(6127, -4822, 384));
                botPath.Add(new Vector3(5460, -5893, 384));
                botPath.Add(new Vector3(5070, -5915, 384));
                botPath.Add(new Vector3(4412, -6048, 384));
                botPath.Add(new Vector3(3388, -6125, 384));
                botPath.Add(new Vector3(2641, -6135, 384));
                botPath.Add(new Vector3(1320, -6374, 384));
                botPath.Add(new Vector3(-174, -6369, 384));
                botPath.Add(new Vector3(-1201, -6308, 384));
                botPath.Add(new Vector3(-3036, -6072, 257));
            }
            else
            {
                //radiant paths

                topPath.Add(new Vector3(-6604, -3979, 384));
                topPath.Add(new Vector3(-6613, -3679, 384));
                topPath.Add(new Vector3(-6775, -3420, 384));
                topPath.Add(new Vector3(-6743, -3042, 369));
                topPath.Add(new Vector3(-6682, -2124, 256));
                topPath.Add(new Vector3(-6640, -1758, 384));
                topPath.Add(new Vector3(-6411, -226, 384));
                topPath.Add(new Vector3(-6370, 2523, 384));
                topPath.Add(new Vector3(-6198, 3997, 384));
                topPath.Add(new Vector3(-6015, 4932, 384));
                topPath.Add(new Vector3(-5888, 5204, 384));
                topPath.Add(new Vector3(-5389, 5548, 384));
                topPath.Add(new Vector3(-4757, 5740, 384));
                topPath.Add(new Vector3(-4066, 5873, 384));
                topPath.Add(new Vector3(-3068, 6009, 384));
                topPath.Add(new Vector3(-2139, 6080, 384));
                topPath.Add(new Vector3(-1327, 6052, 384));
                topPath.Add(new Vector3(-82, 6011, 384));
                topPath.Add(new Vector3(1682, 5993, 311));
                topPath.Add(new Vector3(2404, 6051, 256));

                midPath.Add(new Vector3(-5000, -4458, 384));
                midPath.Add(new Vector3(-4775, -4020, 384));
                midPath.Add(new Vector3(-4242, -3594, 256));
                midPath.Add(new Vector3(-3294, -2941, 256));
                midPath.Add(new Vector3(-2771, -2454, 256));
                midPath.Add(new Vector3(-2219, -1873, 256));
                midPath.Add(new Vector3(-1193, -1006, 256));
                midPath.Add(new Vector3(-573, -449, 128));
                midPath.Add(new Vector3(-46, -49, 256));
                midPath.Add(new Vector3(679, 461, 256));
                midPath.Add(new Vector3(979, 692, 256));
                midPath.Add(new Vector3(2167, 1703, 256));
                midPath.Add(new Vector3(2919, 2467, 256));
                midPath.Add(new Vector3(3785, 3132, 256));

                botPath.Add(new Vector3(-3671, -6077, 384));
                botPath.Add(new Vector3(-3138, -6168, 256));
                botPath.Add(new Vector3(-2126, -6234, 256));
                botPath.Add(new Vector3(-1064, -6383, 384));
                botPath.Add(new Vector3(196, -6602, 384));
                botPath.Add(new Vector3(1771, -6374, 384));
                botPath.Add(new Vector3(3740, -6281, 384));
                botPath.Add(new Vector3(5328, -5813, 384));
                botPath.Add(new Vector3(5766, -5602, 384));
                botPath.Add(new Vector3(6265, -4992, 384));
                botPath.Add(new Vector3(6112, -3603, 384));
                botPath.Add(new Vector3(6103, 1724, 256));
                botPath.Add(new Vector3(6133, 2167, 256));
            }

            Lanes.Add(LanePosition.Middle, midPath.ToArray());
            Lanes.Add(myTeam == Team.Radiant ? LanePosition.Hard : LanePosition.Easy, botPath.ToArray());
            Lanes.Add(myTeam == Team.Radiant ? LanePosition.Easy : LanePosition.Hard, topPath.ToArray());
        }

        public Dictionary<LanePosition, Vector3[]> Lanes { get; } = new Dictionary<LanePosition, Vector3[]>();

        public KeyValuePair<LanePosition, Vector3[]>? GetLaneData(Vector3 position)
        {
            KeyValuePair<LanePosition, Vector3[]>? laneData = null;

            foreach (var lane in Lanes)
            {
                if (lane.Value[0].Distance(position) > 300)
                {
                    continue;
                }

                laneData = lane;
                break;
            }

            return laneData;
        }
    }
}