namespace InformationPinger.PingTypes
{
    using Ensage;
    using Ensage.SDK.Helpers;

    using Interfaces;

    internal class AbilityPing : IPing
    {
        private readonly Ability ability;

        private readonly bool doublePing;

        public AbilityPing(Ability ability, bool doublePing)
        {
            this.ability = ability;
            this.doublePing = doublePing;
            Cooldown = doublePing ? 3.22f : 1.22f;
        }

        public float Cooldown { get; }

        public void Ping()
        {
            if (!ability.IsValid || !ability.Owner.IsValid)
            {
                return;
            }

            ability.Announce();

            if (doublePing)
            {
                UpdateManager.BeginInvoke(() => ability.Announce(), 250);
            }
        }
    }
}