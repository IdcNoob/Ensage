namespace Evader.EvadableAbilities.Heroes.Ursa
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class Earthshock : AOE
    {
        #region Constructors and Destructors

        public Earthshock(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }

        #endregion
    }
}