namespace Evader.EvadableAbilities.Heroes.ShadowShaman
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class Shackles : LinearTarget

    {
        #region Constructors and Destructors

        public Shackles(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Lotus);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(NetherWard);
        }

        #endregion
    }
}