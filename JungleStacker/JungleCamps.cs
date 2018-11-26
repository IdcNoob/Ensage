namespace JungleStacker
{
    using System.Collections.Generic;

    using Classes;

    using Ensage;

    using SharpDX;

    internal class JungleCamps
    {
        public JungleCamps(Team heroTeam)
        {
            if (heroTeam == Team.Radiant)
            {
                GetCamps.Add(
                    new Camp
                    {
                        OverlayPosition = new Vector3(-1411, -4284, 256),
                        CampPosition = new Vector3(-1858, -4138, 256),
                        StackPosition = new Vector3(-1805, -2930, 256),
                        WaitPosition = new Vector3(-1829, -3834, 256),
                        Id = 1,
                        StackTime = 56,
                        Team = Team.Radiant,
                        Ancients = false,
                        Name = "Hard Camp"
                    });
                GetCamps.Add(
                    new Camp
                    {
                        OverlayPosition = new Vector3(-794, -3424, 384),
                        CampPosition = new Vector3(-154, -3366, 384),
                        StackPosition = new Vector3(1252, -3555, 384),
                        WaitPosition = new Vector3(303, -3363, 256),
                        Id = 2,
                        StackTime = 56,
                        Team = Team.Radiant,
                        Ancients = true,
                        Name = "Ancient Camp"
                    });
                GetCamps.Add(
                    new Camp
                    {
                        OverlayPosition = new Vector3(135, -4793, 384),
                        CampPosition = new Vector3(431, -4637, 384),
                        StackPosition = new Vector3(751, -3479, 384),
                        WaitPosition = new Vector3(719, -4391, 384),
                        Id = 3,
                        StackTime = 55,
                        Team = Team.Radiant,
                        Ancients = false,
                        Name = "Bot Medium Camp"
                    });
                GetCamps.Add(
                    new Camp
                    {
                        OverlayPosition = new Vector3(4850, -4384, 256),
                        CampPosition = new Vector3(4502, -4319, 256),
                        StackPosition = new Vector3(2598, -3274, 269),
                        WaitPosition = new Vector3(4165, -4129, 256),
                        Id = 4,
                        StackTime = 54,
                        Team = Team.Radiant,
                        Ancients = false,
                        Name = "Bot Hard Camp",
                        StackCountTimeAdjustment = 3,
                        TimeAdjustment = 1,
                        MaxTimeAdjustment = 2,
                        PullTime = "x:18 / x:48",
                        DrawPullTime = true
                    });
                GetCamps.Add(
                    new Camp
                    {
                        OverlayPosition = new Vector3(3168, -4320, 256),
                        CampPosition = new Vector3(3030, -4555, 256),
                        StackPosition = new Vector3(3486, -6287, 384),
                        WaitPosition = new Vector3(3432, -4656, 256),
                        Id = 5,
                        StackTime = 54,
                        Team = Team.Radiant,
                        Ancients = false,
                        Name = "Easy Camp",
                        PullTime = "x:16 / x:46",
                        DrawPullTime = true
                    });
                GetCamps.Add(
                    new Camp
                    {
                        OverlayPosition = new Vector3(-2290, -342, 384),
                        CampPosition = new Vector3(-2551, -594, 384),
                        StackPosition = new Vector3(-1892, 981, 384),
                        WaitPosition = new Vector3(-2954, -128, 384),
                        Id = 6,
                        StackTime = 55,
                        Team = Team.Radiant,
                        Ancients = true,
                        Name = "Top Ancients Camp",
                        StackCountTimeAdjustment = 2,
                        TimeAdjustment = 1.5,
                        MaxTimeAdjustment = 4
                    });
                GetCamps.Add(
                    new Camp
                    {
                        OverlayPosition = new Vector3(-5121, -239, 256),
                        CampPosition = new Vector3(-4809, -408, 256),
                        StackPosition = new Vector3(-4658, 1403, 384),
                        WaitPosition = new Vector3(-4573, -111, 256),
                        Id = 7,
                        StackTime = 55.5,
                        Team = Team.Radiant,
                        Ancients = false,
                        Name = "Top Hard Camp",
                    });
                GetCamps.Add(
                    new Camp
                    {
                        OverlayPosition = new Vector3(-3324, 736, 256),
                        CampPosition = new Vector3(-3824, 766, 256),
                        StackPosition = new Vector3(-5985, 450, 384),
                        WaitPosition = new Vector3(-4049, 620, 256),
                        Id = 8,
                        StackTime = 55,
                        Team = Team.Radiant,
                        Ancients = false,
                        Name = "Top Medium Camp",
                        StackCountTimeAdjustment = 2,
                        TimeAdjustment = 1,
                        MaxTimeAdjustment = 3
                    });
                GetCamps.Add(
                    new Camp
                    {
                        OverlayPosition = new Vector3(567, -1908, 384),
                        CampPosition = new Vector3(105, -1864, 256),
                        StackPosition = new Vector3(2125, -2432, 384),
                        WaitPosition = new Vector3(528, -2283, 384),
                        Id = 9,
                        StackTime = 56,
                        Team = Team.Radiant,
                        Ancients = false,
                        Name = "Medium Camp",
                        StackCountTimeAdjustment = 3,
                        TimeAdjustment = 1,
                        MaxTimeAdjustment = 2
                    });
            }
            else if (heroTeam == Team.Dire)
            {
                GetCamps.Add(
                    new Camp
                    {
                        OverlayPosition = new Vector3(1700, 3400, 384),
                        CampPosition = new Vector3(1339, 3356, 384),
                        StackPosition = new Vector3(364, 4898, 384),
                        WaitPosition = new Vector3(1041, 3593, 383),
                        Id = 10,
                        StackTime = 54,
                        Team = Team.Dire,
                        Ancients = false,
                        Name = "Hard Camp",
                        StackCountTimeAdjustment = 2,
                        TimeAdjustment = 1,
                        MaxTimeAdjustment = 1
                    });
                GetCamps.Add(
                    new Camp
                    {
                        OverlayPosition = new Vector3(-352, 3070, 256),
                        CampPosition = new Vector3(-270, 3424, 256),
                        StackPosition = new Vector3(-380, 4910, 384),
                        WaitPosition = new Vector3(-551, 3556, 256),
                        Id = 11,
                        StackTime = 54,
                        Team = Team.Dire,
                        Ancients = true,
                        Name = "Top Ancient Camp",
                        StackCountTimeAdjustment = 2,
                        TimeAdjustment = 1,
                        MaxTimeAdjustment = 3
                    });
                GetCamps.Add(
                    new Camp
                    {
                        OverlayPosition = new Vector3(-1184, 2208, 384),
                        CampPosition = new Vector3(-827, 2278, 384),
                        StackPosition = new Vector3(982, 2251, 384),
                        WaitPosition = new Vector3(-419, 2473, 384),
                        Id = 12,
                        StackTime = 54,
                        Team = Team.Dire,
                        Ancients = false,
                        Name = "Top Medium Camp",
                        StackCountTimeAdjustment = 3,
                        TimeAdjustment = 1,
                        MaxTimeAdjustment = 2
                    });
                GetCamps.Add(
                    new Camp
                    {
                        OverlayPosition = new Vector3(-2498, 4921, 259),
                        CampPosition = new Vector3(-2470, 4770, 256),
                        StackPosition = new Vector3(-3408, 6349, 384),
                        WaitPosition = new Vector3(-2911, 4896, 256),
                        Id = 13,
                        StackTime = 54,
                        Team = Team.Dire,
                        Ancients = false,
                        Name = "Easy Camp",
                        PullTime = "x:16 / x:46",
                        DrawPullTime = true
                    });
                GetCamps.Add(
                    new Camp
                    {
                        OverlayPosition = new Vector3(-4100, 3300, 256),
                        CampPosition = new Vector3(-4171, 3500, 256),
                        StackPosition = new Vector3(-1773, 3678, 256),
                        WaitPosition = new Vector3(-4137, 4040, 256),
                        Id = 14,
                        StackTime = 54.5,
                        Team = Team.Dire,
                        Ancients = false,
                        Name = "Top Hard Camp",
                        StackCountTimeAdjustment = 3,
                        TimeAdjustment = 1,
                        MaxTimeAdjustment = 2,
                        PullTime = "x:18 / x:48",
                        DrawPullTime = true
                    });
                GetCamps.Add(
                    new Camp
                    {
                        OverlayPosition = new Vector3(4200, -400, 256),
                        CampPosition = new Vector3(4105, -365, 384),
                        StackPosition = new Vector3(3811, -2238, 256),
                        WaitPosition = new Vector3(4045, -797, 256),
                        Id = 15,
                        StackTime = 53.5,
                        Team = Team.Dire,
                        Ancients = false,
                        Name = "Bot Ancients Camp"
                    });
                GetCamps.Add(
                    new Camp
                    {
                        OverlayPosition = new Vector3(4696, 1120, 384),
                        CampPosition = new Vector3(4273, 791, 384),
                        StackPosition = new Vector3(3148, 687, 384),
                        WaitPosition = new Vector3(4001, 745, 384),
                        Id = 16,
                        StackTime = 55,
                        Team = Team.Dire,
                        Ancients = false,
                        Name = "Bot Hard Camp"
                    });
                GetCamps.Add(
                    new Camp
                    {
                        OverlayPosition = new Vector3(-1940, 4669, 384),
                        CampPosition = new Vector3(-1984, 4282, 256),
                        StackPosition = new Vector3(-961, 5011, 384),
                        WaitPosition = new Vector3(-1619, 4058, 256),
                        Id = 17,
                        StackTime = 55,
                        Team = Team.Dire,
                        Ancients = false,
                        Name = "Top Medium Camp"
                    });
                GetCamps.Add(
                    new Camp
                    {
                        OverlayPosition = new Vector3(2527, 478, 384),
                        CampPosition = new Vector3(2685, 110, 384),
                        StackPosition = new Vector3(4425, 11, 384),
                        WaitPosition = new Vector3(3039, 106, 384),
                        Id = 18,
                        StackTime = 54,
                        Team = Team.Dire,
                        Ancients = true,
                        Name = "Bot Medium Camp",
                        StackCountTimeAdjustment = 3,
                        TimeAdjustment = 1,
                        MaxTimeAdjustment = 1
                    });
            }
        }

        public List<Camp> GetCamps { get; } = new List<Camp>();

        public void OnClose()
        {
            GetCamps.ForEach(x => x.OnClose());
            GetCamps.Clear();
        }
    }
}