namespace InformationPinger.PingTypes
{
    using Ensage;
    using Ensage.SDK.Helpers;

    using Interfaces;

    internal class HeroChatWheelPing : IPing
    {
        private readonly bool doublePing;

        private readonly HeroId heroId;

        private readonly ChatWheelMessage message;

        public HeroChatWheelPing(ChatWheelMessage message, HeroId heroId, bool doublePing)
        {
            this.message = message;
            this.heroId = heroId;
            this.doublePing = doublePing;
            Cooldown = doublePing ? 3.22f : 1.2f;
        }

        public float Cooldown { get; }

        public void Ping()
        {
            Network.ChatWheel(message, heroId);

            if (doublePing)
            {
                UpdateManager.BeginInvoke(() => Network.ChatWheel(message, heroId), 250);
            }
        }
    }
}