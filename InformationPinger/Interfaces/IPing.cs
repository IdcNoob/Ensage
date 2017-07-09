namespace InformationPinger.Interfaces
{
    internal interface IPing
    {
        float Cooldown { get; }

        void Ping();
    }
}