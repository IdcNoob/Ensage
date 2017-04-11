namespace InformationPinger.Modules
{
    using System;

    using Ensage;
    using Ensage.Common;

    internal class ChatWheel
    {
        public enum Phrase
        {
            NeedWards = 3,

            UpgradeCourier = 40,

            Roshan = 53,

            CurrentTime = 57,

            CheckRunes = 58,
        }

        private readonly Random random = new Random();

        public void Say(Phrase say, bool sayTime = false)
        {
            Game.ExecuteCommand("chatwheel_say " + (int)say);

            if (sayTime)
            {
                DelayAction.Add(250, () => Game.ExecuteCommand("chatwheel_say " + (int)Phrase.CurrentTime));
                Variables.Sleeper.Sleep(random.Next(3111, 3333), "CanPing");
            }
            else
            {
                Variables.Sleeper.Sleep(random.Next(1111, 1333), "CanPing");
            }
        }
    }
}