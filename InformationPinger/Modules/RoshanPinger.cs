namespace InformationPinger.Modules
{
    using Ensage;

    internal class RoshanPinger
    {
        #region Constructors and Destructors

        public RoshanPinger()
        {
            Game.OnFireEvent += Game_OnFireEvent;
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
                RoshanKilled = true;
            }
        }

        #endregion
    }
}