namespace JungleStacker
{
    using System.Collections.Generic;

    using Ensage;

    using global::JungleStacker.Classes;

    using SharpDX;

    internal class JungleCamps
    {
        #region Fields

        private readonly List<Camp> camps = new List<Camp>();

        #endregion

        #region Constructors and Destructors

        public JungleCamps(Team heroTeam)
        {
            if (heroTeam == Team.Radiant)
            {
                this.camps.Add(
                    new Camp
                        {
                            OverlayPosition = new Vector3(-1500, -4100, 256),
                            CampPosition = new Vector3(-1704, -4205, 256), StackPosition = new Vector3(-1833, -3062, 256),
                            WaitPosition = new Vector3(-1925, -4062, 256), Id = 1, StackTime = 55.5, Team = Team.Radiant,
                            Ancients = false, Name = "Hard Camp"
                        });

                this.camps.Add(
                    new Camp
                        {
                            OverlayPosition = new Vector3(100, -3424, 384), CampPosition = new Vector3(-276, -3099, 256),
                            StackPosition = new Vector3(-554, -1925, 256), WaitPosition = new Vector3(-238, -2901, 256),
                            Id = 2, StackTime = 55, Team = Team.Radiant, Ancients = false, Name = "Medium Camp"
                        });

                this.camps.Add(
                    new Camp
                        {
                            OverlayPosition = new Vector3(1600, -3200, 384), CampPosition = new Vector3(1591, -3758, 256),
                            StackPosition = new Vector3(-127, -4558, 256), WaitPosition = new Vector3(1642, -3995, 256),
                            Id = 3, StackTime = 53, Team = Team.Radiant, Ancients = false, Name = "Medium Camp"
                        });
                this.camps.Add(
                    new Camp
                        {
                            OverlayPosition = new Vector3(4150, -3400, 384), CampPosition = new Vector3(4404, -3672, 384),
                            StackPosition = new Vector3(3002, -3936, 384), WaitPosition = new Vector3(4347, -3852, 384),
                            Id = 4, StackTime = 53, Team = Team.Radiant, Ancients = false, Name = "Hard Camp"
                        });
                this.camps.Add(
                    new Camp
                        {
                            OverlayPosition = new Vector3(3400, -4700, 384), CampPosition = new Vector3(2958, -4796, 384),
                            StackPosition = new Vector3(1555, -5337, 384), WaitPosition = new Vector3(2993, -5048, 384),
                            Id = 5, StackTime = 53, Team = Team.Radiant, Ancients = false, Name = "Easy Camp"
                        });
                this.camps.Add(
                    new Camp
                        {
                            OverlayPosition = new Vector3(-3000, 300, 384), CampPosition = new Vector3(-2965, -64, 384),
                            StackPosition = new Vector3(-3472, -1566, 384), WaitPosition = new Vector3(-2649, -140, 384),
                            Id = 6, StackTime = 53, Team = Team.Radiant, Ancients = true, Name = "Ancients Camp"
                        });
                this.camps.Add(
                    new Camp
                        {
                            OverlayPosition = new Vector3(-3300, 825, 384), CampPosition = new Vector3(-3829, 650, 384),
                            StackPosition = new Vector3(-3893, -737, 384), WaitPosition = new Vector3(-4016, 589, 384),
                            Id = 7, StackTime = 53, Team = Team.Radiant, Ancients = true, Name = "Secret Camp"
                        });
            }
            else if (heroTeam == Team.Dire)
            {
                this.camps.Add(
                    new Camp
                        {
                            OverlayPosition = new Vector3(1700, 3400, 384), CampPosition = new Vector3(1118, 3409, 384),
                            StackPosition = new Vector3(449, 4752, 384), WaitPosition = new Vector3(960, 3688, 384),
                            Id = 8, StackTime = 54, Team = Team.Dire, Ancients = false, Name = "Hard Camp"
                        });
                this.camps.Add(
                    new Camp
                        {
                            OverlayPosition = new Vector3(-291, 3200, 384), CampPosition = new Vector3(-337, 3741, 384),
                            StackPosition = new Vector3(-611, 5520, 256), WaitPosition = new Vector3(-499, 4027, 384),
                            Id = 9, StackTime = 55.5, Team = Team.Dire, Ancients = false, Name = "Medium Camp"
                        });
                this.camps.Add(
                    new Camp
                        {
                            OverlayPosition = new Vector3(-1184, 2208, 384), CampPosition = new Vector3(-1532, 2733, 256),
                            StackPosition = new Vector3(-1180, 4090, 384), WaitPosition = new Vector3(-1441, 2900, 256),
                            Id = 10, StackTime = 55, Team = Team.Dire, Ancients = false, Name = "Medium Camp"
                        });
                this.camps.Add(
                    new Camp
                        {
                            OverlayPosition = new Vector3(-3077, 4100, 384), CampPosition = new Vector3(-3077, 4698, 384),
                            StackPosition = new Vector3(-3533, 6295, 384), WaitPosition = new Vector3(-3074, 4851, 384),
                            Id = 11, StackTime = 53, Team = Team.Dire, Ancients = false, Name = "Easy Camp"
                        });
                this.camps.Add(
                    new Camp
                        {
                            OverlayPosition = new Vector3(-4200, 3300, 384), CampPosition = new Vector3(-4403, 3607, 384),
                            StackPosition = new Vector3(-2801, 3684, 245), WaitPosition = new Vector3(-4185, 3739, 256),
                            Id = 12, StackTime = 54, Team = Team.Dire, Ancients = false, Name = "Hard Camp"
                        });
                this.camps.Add(
                    new Camp
                        {
                            OverlayPosition = new Vector3(4200, -400, 256), CampPosition = new Vector3(3902, -711, 256),
                            StackPosition = new Vector3(2493, -1059, 256), WaitPosition = new Vector3(3694, -852, 127),
                            Id = 13, StackTime = 53, Team = Team.Dire, Ancients = true, Name = "Ancients Camp"
                        });
                this.camps.Add(
                    new Camp
                        {
                            OverlayPosition = new Vector3(4200, 850, 384), CampPosition = new Vector3(4085, 542, 384),
                            StackPosition = new Vector3(3537, 1713, 256), WaitPosition = new Vector3(3822, 531, 384),
                            Id = 14, StackTime = 55, Team = Team.Dire, Ancients = false, Name = "Secret Camp"
                        });
            }
        }

        #endregion

        #region Public Properties

        public List<Camp> GetCamps
        {
            get
            {
                return this.camps;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            this.camps.ForEach(x => x.OnClose());
            this.camps.Clear();
        }

        #endregion
    }
}