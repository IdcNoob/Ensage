namespace InformationPinger.Modules
{
    using System.Linq;

    using Ensage;

    internal class CourierPinger
    {
        private readonly Team team;

        private float lastPing;

        public CourierPinger(Team heroTeam)
        {
            team = heroTeam;
            lastPing = float.MinValue;
        }

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
                if (ObjectManager.GetEntitiesParallel<Courier>().Any(x => x.IsFlying && x.Team == team))
                {
                    return false;
                }

                lastPing = time;
                Variables.Sleeper.Sleep(delay * 60 * 1000, this);
                return true;
            }

            return false;
        }
    }
}