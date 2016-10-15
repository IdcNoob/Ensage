namespace InformationPinger.Modules
{
    using System;

    using Ensage;
    using Ensage.Common;

    using SharpDX;

    internal class RoshanPinger
    {
        #region Fields

        private readonly Random random;

        #endregion

        #region Constructors and Destructors

        public RoshanPinger()
        {
            Game.OnFireEvent += Game_OnFireEvent;
            random = new Random();
        }

        #endregion

        #region Public Properties

        public bool RoshanKilled { get; set; }

        #endregion

        #region Methods

        private void Game_OnFireEvent(FireEventEventArgs args)
        {
            if (args.GameEvent.Name == "dota_roshan_kill")
            {
                DelayAction.Add(random.NextFloat(500, 2500), () => RoshanKilled = true);
            }
        }

        #endregion
    }
}