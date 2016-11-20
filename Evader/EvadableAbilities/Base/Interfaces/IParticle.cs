namespace Evader.EvadableAbilities.Base.Interfaces
{
    using Ensage;

    internal interface IParticle
    {
        #region Public Methods and Operators

        void AddParticle(ParticleEffectAddedEventArgs particleArgs);

        #endregion
    }
}