namespace InformationPinger.Modules
{
    using System;

    using Ensage;

    internal class ChatWheel
    {
        #region Fields

        private readonly Random random = new Random();

        #endregion

        #region Enums

        public enum Phrase
        {
            NeedWards = 3,

            UpgradeCourier = 40,

            Roshan = 53,

            CurrentTime = 57,

            CheckRunes = 58,
        }

        #endregion

        #region Public Methods and Operators

        public void Say(Phrase say, bool sayTime = false)
        {
            Game.ExecuteCommand("chatwheel_say " + (int)say);

            if (sayTime)
            {
                Game.ExecuteCommand("chatwheel_say " + (int)Phrase.CurrentTime);
                Variables.Sleeper.Sleep(random.Next(3111, 3333), "CanPing");
            }
            else
            {
                Variables.Sleeper.Sleep(random.Next(1111, 1333), "CanPing");
            }
        }

        #endregion
    }
}