namespace InformationPinger.Modules
{
    using System;

    using Ensage;
    using Ensage.Common;

    internal class ChatWheel
    {
        private readonly Random random = new Random();

        public void Say(ChatWheelMessage message, bool sayTime = false)
        {
            Network.ChatWheel(message);

            if (sayTime)
            {
                DelayAction.Add(250, () => Network.ChatWheel(ChatWheelMessage.Current_Time));
                Variables.Sleeper.Sleep(random.Next(3111, 3333), "CanPing");
            }
            else
            {
                Variables.Sleeper.Sleep(random.Next(1111, 1333), "CanPing");
            }
        }
    }
}