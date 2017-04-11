namespace InformationPinger.Modules
{
    using Ensage;

    internal class RunePinger
    {
        public bool TimeToSpawn(int time)
        {
            if (Variables.Sleeper.Sleeping(this))
            {
                return false;
            }

            Variables.Sleeper.Sleep(1000, this);

            if (Game.GameTime % 120 > 120 - time)
            {
                Variables.Sleeper.Sleep((120 - time) * 1000, this);
                return true;
            }

            return false;
        }
    }
}