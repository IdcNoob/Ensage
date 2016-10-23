namespace Evader.EvadableAbilities.Heroes.Puck
{
    using Ensage;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class IllusoryOrb : LinearProjectile
    {
        #region Constructors and Destructors

        public IllusoryOrb(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
        }

        #endregion
    }
}