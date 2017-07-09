namespace InformationPinger.PingTypes
{
    using Ensage;
    using Ensage.SDK.Helpers;

    using Interfaces;

    internal class ChatWheelPing : IPing
    {
        private readonly ChatWheelMessage message;

        private readonly bool sayTime;

        public ChatWheelPing(ChatWheelMessage message, bool sayTime = false)
        {
            this.message = message;
            this.sayTime = sayTime;
            Cooldown = sayTime ? 3.3f : 1.3f;
        }

        public float Cooldown { get; }

        public void Ping()
        {
            Network.ChatWheel(message);

            if (sayTime)
            {
                UpdateManager.BeginInvoke(() => Network.ChatWheel(ChatWheelMessage.Current_Time), 250);
            }
        }
    }
}