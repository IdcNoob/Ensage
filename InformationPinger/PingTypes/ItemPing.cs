namespace InformationPinger.PingTypes
{
    using Ensage;
    using Ensage.SDK.Helpers;

    using Interfaces;

    internal class ItemPing : IPing
    {
        private readonly bool doublePing;

        private readonly Item item;

        public ItemPing(Item item, bool doublePing)
        {
            this.item = item;
            this.doublePing = doublePing;
            Cooldown = doublePing ? 3.22f : 1.22f;
        }

        public float Cooldown { get; }

        public void Ping()
        {
            if (!item.IsValid || !item.Owner.IsValid)
            {
                return;
            }

            Network.EnemyItemAlert(item);

            if (doublePing)
            {
                UpdateManager.BeginInvoke(() => Network.EnemyItemAlert(item), 250);
            }
        }
    }
}