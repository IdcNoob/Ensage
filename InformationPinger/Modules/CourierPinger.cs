namespace InformationPinger.Modules
{
    using System.Linq;

    using Ensage;

    internal class CourierPinger
    {
        #region Fields

        private readonly Team team;

        private float lastPing;

        #endregion

        #region Constructors and Destructors

        public CourierPinger(Team heroTeam)
        {
            team = heroTeam;
            lastPing = float.MinValue;
        }

        #endregion

        #region Public Methods and Operators

        public bool ShouldRemind(int delay)
        {
            if (Variables.Sleeper.Sleeping(this))
            {
                return false;
            }

            Variables.Sleeper.Sleep(1000, this);

            var time = Game.GameTime;

            if (time < 180)
            {
                return false;
            }

            if (time > lastPing + delay * 60)
            {
                if (ObjectManager.GetEntities<Courier>().Any(x => x.IsFlying && x.Team == team))
                {
                    return false;
                }

                lastPing = time;
                Variables.Sleeper.Sleep(delay * 60 * 1000, this);
                return true;
            }

            return false;
        }

        #endregion
    }
}