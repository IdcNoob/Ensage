namespace InformationPinger.Modules
{
    using System;

    using Ensage;
    using Ensage.Common;

    internal class RoshanPinger
    {
        private readonly Random random;

        public RoshanPinger()
        {
            Game.OnFireEvent += Game_OnFireEvent;
            random = new Random();
        }

        public bool RoshanKilled { get; set; }

        private void Game_OnFireEvent(FireEventEventArgs args)
        {
            if (args.GameEvent.Name == "dota_roshan_kill")
            {
                DelayAction.Add(random.Next(500, 2500), () => RoshanKilled = true);
            }
        }
    }
}