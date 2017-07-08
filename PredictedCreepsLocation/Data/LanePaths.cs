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
                topPath.Add(new Vector3(-480, 5793, 384));
                topPath.Add(new Vector3(-1617, 5804, 384));
                topPath.Add(new Vector3(-3599, 5793, 384));
                topPath.Add(new Vector3(-4981, 5590, 384));
                topPath.Add(new Vector3(-5488, 5404, 384));
                topPath.Add(new Vector3(-5813, 5058, 384));
                topPath.Add(new Vector3(-6111, 4176, 384));
                topPath.Add(new Vector3(-6112, 2898, 384));
                topPath.Add(new Vector3(-6157, 2380, 384));
                topPath.Add(new Vector3(-6161, 2001, 384));
                topPath.Add(new Vector3(-6192, 1362, 384));
                topPath.Add(new Vector3(-6189, 994, 384));
                topPath.Add(new Vector3(-6210, 334, 384));
                topPath.Add(new Vector3(-6229, -22, 384));
                topPath.Add(new Vector3(-6272, -1202, 384));
                topPath.Add(new Vector3(-6336, -2420, 256));

                midPath.Add(new Vector3(4101, 3652, 384));
                midPath.Add(new Vector3(3598, 3092, 256));
                midPath.Add(new Vector3(2570, 2098, 256));
                midPath.Add(new Vector3(2095, 1608, 256));
                midPath.Add(new Vector3(1216, 904, 256));
                midPath.Add(new Vector3(966, 746, 256));
                midPath.Add(new Vector3(765, 671, 256));
                midPath.Add(new Vector3(300, 353, 256));
                midPath.Add(new Vector3(-242, -4, 254));
                midPath.Add(new Vector3(-904, -510, 196));
                midPath.Add(new Vector3(-1170, -764, 256));
                midPath.Add(new Vector3(-1964, -1501, 256));
                midPath.Add(new Vector3(-2208, -1747, 256));
                midPath.Add(new Vector3(-3003, -2514, 256));
                midPath.Add(new Vector3(-3260, -2779, 256));
                midPath.Add(new Vector3(-3523, -3021, 256));
                midPath.Add(new Vector3(-3963, -3453, 256));

                botPath.Add(new Vector3(6239, 3717, 384));
                botPath.Add(new Vector3(6153, 3043, 384));
                botPath.Add(new Vector3(6198, 2493, 351));
                botPath.Add(new Vector3(6219, 2147, 256));
                botPath.Add(new Vector3(6248, 1813, 261));
                botPath.Add(new Vector3(6408, 632, 384));
                botPath.Add(new Vector3(6422, -786, 384));
                botPath.Add(new Vector3(6478, -2137, 384));
                botPath.Add(new Vector3(6170, -2841, 384));
                botPath.Add(new Vector3(6193, -3544, 384));
                botPath.Add(new Vector3(6150, -4820, 384));
                botPath.Add(new Vector3(5713, -5516, 384));
                botPath.Add(new Vector3(5297, -5749, 384));
                botPath.Add(new Vector3(4962, -5873, 384));
                botPath.Add(new Vector3(4616, -5962, 384));
                botPath.Add(new Vector3(3895, -6111, 384));
                botPath.Add(new Vector3(3398, -6154, 384));
                botPath.Add(new Vector3(2694, -6163, 384));
                botPath.Add(new Vector3(1336, -6291, 384));
                botPath.Add(new Vector3(989, -6281, 384));
                botPath.Add(new Vector3(-1912, -6246, 256));
                botPath.Add(new Vector3(-2939, -6243, 256));
            }
            else
            {
                //radiant paths

                topPath.Add(new Vector3(-6641, -4072, 384));
                topPath.Add(new Vector3(-6664, -3573, 384));
                topPath.Add(new Vector3(-6771, -3388, 384));
                topPath.Add(new Vector3(-6725, -2805, 256));
                topPath.Add(new Vector3(-6682, -2124, 256));
                topPath.Add(new Vector3(-6642, -1780, 384));
                topPath.Add(new Vector3(-6589, -1448, 384));
                topPath.Add(new Vector3(-6410, -227, 384));
                topPath.Add(new Vector3(-6402, 108, 384));
                topPath.Add(new Vector3(-6366, 4061, 384));
                topPath.Add(new Vector3(-6204, 4966, 384));
                topPath.Add(new Vector3(-5910, 5405, 384));
                topPath.Add(new Vector3(-5410, 5582, 384));
                topPath.Add(new Vector3(-4765, 5666, 384));
                topPath.Add(new Vector3(-3111, 5766, 384));
                topPath.Add(new Vector3(-2192, 5809, 384));
                topPath.Add(new Vector3(948, 5794, 384));
                topPath.Add(new Vector3(1620, 5809, 342));
                topPath.Add(new Vector3(2301, 5768, 264));

                midPath.Add(new Vector3(-5000, -4458, 384));
                midPath.Add(new Vector3(-4820, -4157, 384));
                midPath.Add(new Vector3(-4255, -3605, 256));
                midPath.Add(new Vector3(-3331, -2905, 256));
                midPath.Add(new Vector3(-1289, -904, 256));
                midPath.Add(new Vector3(-660, -373, 129));
                midPath.Add(new Vector3(-137, -5, 256));
                midPath.Add(new Vector3(669, 479, 256));
                midPath.Add(new Vector3(946, 688, 256));
                midPath.Add(new Vector3(1216, 906, 256));
                midPath.Add(new Vector3(2169, 1708, 256));
                midPath.Add(new Vector3(2419, 1948, 256));
                midPath.Add(new Vector3(2921, 2456, 256));
                midPath.Add(new Vector3(3460, 2937, 264));

                botPath.Add(new Vector3(-3671, -6077, 384));
                botPath.Add(new Vector3(-3145, -6175, 256));
                botPath.Add(new Vector3(-2684, -6198, 256));
                botPath.Add(new Vector3(-2123, -6239, 256));
                botPath.Add(new Vector3(-1759, -6297, 256));
                botPath.Add(new Vector3(-1425, -6335, 311));
                botPath.Add(new Vector3(-1070, -6368, 384));
                botPath.Add(new Vector3(207, -6597, 384));
                botPath.Add(new Vector3(581, -6531, 384));
                botPath.Add(new Vector3(1761, -6373, 384));
                botPath.Add(new Vector3(3014, -6119, 384));
                botPath.Add(new Vector3(4323, -5940, 384));
                botPath.Add(new Vector3(5315, -5771, 384));
                botPath.Add(new Vector3(5674, -5550, 384));
                botPath.Add(new Vector3(6132, -4854, 384));
                botPath.Add(new Vector3(6240, -3820, 384));
                botPath.Add(new Vector3(6240, 944, 384));
                botPath.Add(new Vector3(6333, 1787, 264));
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