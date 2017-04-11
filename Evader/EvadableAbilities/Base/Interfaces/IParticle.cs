namespace Evader.EvadableAbilities.Base.Interfaces
{
    using Ensage;

    internal interface IParticle
    {
        void AddParticle(ParticleEffectAddedEventArgs particleArgs);
    }
}