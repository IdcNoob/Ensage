namespace PredictedCreepsLocation
{
    using System.Collections.Generic;

    using Ensage;
    using Ensage.Common.Objects;

    using SharpDX;

    internal class CreepsData
    {
        #region Constructors and Destructors

        public CreepsData(Team team)
        {
            var topPath = new List<Vector3>();
            var midPath = new List<Vector3>();
            var botPath = new List<Vector3>();

            if (team == Team.Radiant)
            {
                topPath.Add(new Vector3(3176, 5792, 384));
                topPath.Add(new Vector3(-259, 5794, 384));
                topPath.Add(new Vector3(-1656, 5805, 384));
                topPath.Add(new Vector3(-3595, 5792, 384));
                topPath.Add(new Vector3(-3937, 5738, 384));
                topPath.Add(new Vector3(-4987, 5586, 384));
                topPath.Add(new Vector3(-5540, 5354, 384));
                topPath.Add(new Vector3(-5825, 5033, 384));
                topPath.Add(new Vector3(-6113, 4095, 384));
                topPath.Add(new Vector3(-6314, -1229, 384));
                topPath.Add(new Vector3(-6394, -2772, 256));

                midPath.Add(new Vector3(4074, 3582, 384));
                midPath.Add(new Vector3(3702, 3223, 256));
                midPath.Add(new Vector3(2599, 1958, 256));
                midPath.Add(new Vector3(2134, 1537, 256));
                midPath.Add(new Vector3(1077, 818, 256));
                midPath.Add(new Vector3(203, 275, 256));
                midPath.Add(new Vector3(-321, -69, 256));
                midPath.Add(new Vector3(-993, -597, 256));
                midPath.Add(new Vector3(-1991, -1527, 256));
                midPath.Add(new Vector3(-3332, -2836, 256));
                midPath.Add(new Vector3(-3997, -3559, 256));

                botPath.Add(new Vector3(6281, 3698, 384));
                botPath.Add(new Vector3(6451, 3034, 384));
                botPath.Add(new Vector3(6324, 2280, 256));
                botPath.Add(new Vector3(6405, 649, 384));
                botPath.Add(new Vector3(6420, -1187, 384));
                botPath.Add(new Vector3(6283, -3628, 384));
                botPath.Add(new Vector3(5989, -4942, 384));
                botPath.Add(new Vector3(5663, -5525, 384));
                botPath.Add(new Vector3(4615, -5955, 384));
                botPath.Add(new Vector3(3880, -6105, 384));
                botPath.Add(new Vector3(2847, -6110, 384));
                botPath.Add(new Vector3(1293, -6279, 384));
                botPath.Add(new Vector3(-2950, -6197, 256));
            }
            else
            {
                topPath.Add(new Vector3(-6645, -4142, 384));
                topPath.Add(new Vector3(-6419, -3404, 384));
                topPath.Add(new Vector3(-6550, -1735, 384));
                topPath.Add(new Vector3(-6397, -226, 384));
                topPath.Add(new Vector3(-6366, 4061, 384));
                topPath.Add(new Vector3(-6192, 4985, 384));
                topPath.Add(new Vector3(-5879, 5416, 384));
                topPath.Add(new Vector3(-5409, 5582, 384));
                topPath.Add(new Vector3(-4765, 5666, 384));
                topPath.Add(new Vector3(-3111, 5766, 384));
                topPath.Add(new Vector3(-2170, 5809, 384));
                topPath.Add(new Vector3(948, 5794, 384));
                topPath.Add(new Vector3(2745, 5773, 256));

                midPath.Add(new Vector3(-5135, -4521, 384));
                midPath.Add(new Vector3(-4825, -4029, 384));
                midPath.Add(new Vector3(-4165, -3671, 256));
                midPath.Add(new Vector3(-3307, -2881, 256));
                midPath.Add(new Vector3(-3029, -2640, 256));
                midPath.Add(new Vector3(-1783, -1382, 256));
                midPath.Add(new Vector3(-1269, -885, 256));
                midPath.Add(new Vector3(-647, -363, 128));
                midPath.Add(new Vector3(-123, 3, 256));
                midPath.Add(new Vector3(933, 678, 256));
                midPath.Add(new Vector3(1228, 917, 256));
                midPath.Add(new Vector3(2164, 1704, 256));
                midPath.Add(new Vector3(2689, 2222, 256));
                midPath.Add(new Vector3(3242, 2708, 256));
                midPath.Add(new Vector3(3764, 3282, 256));

                botPath.Add(new Vector3(-3738, -6105, 384));
                botPath.Add(new Vector3(-3059, -6183, 256));
                botPath.Add(new Vector3(-2118, -6227, 256));
                botPath.Add(new Vector3(-1092, -6303, 384));
                botPath.Add(new Vector3(-109, -6415, 384));
                botPath.Add(new Vector3(776, -6354, 384));
                botPath.Add(new Vector3(4365, -5917, 384));
                botPath.Add(new Vector3(4845, -5693, 384));
                botPath.Add(new Vector3(5369, -5333, 384));
                botPath.Add(new Vector3(5729, -4684, 384));
                botPath.Add(new Vector3(6031, -3794, 384));
                botPath.Add(new Vector3(6048, 1664, 384));
                botPath.Add(new Vector3(6113, 2214, 256));
            }

            LaneData.Add(
                new LaneData
                    {
                        LanePosition = LanePosition.Top,
                        Points = topPath,
                        Team = team
                    });
            LaneData.Add(
                new LaneData
                    {
                        LanePosition = LanePosition.Middle,
                        Points = midPath,
                        Team = team
                    });
            LaneData.Add(
                new LaneData
                    {
                        LanePosition = LanePosition.Bottom,
                        Points = botPath,
                        Team = team
                    });
        }

        #endregion

        #region Public Properties

        public List<LaneData> LaneData { get; } = new List<LaneData>();

        #endregion
    }
}