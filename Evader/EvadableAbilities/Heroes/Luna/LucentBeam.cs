namespace Evader.EvadableAbilities.Heroes.Luna
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class LucentBeam : LinearTarget
    {
        public LucentBeam(Ability ability)
            : base(ability)
        {
            IsDisable = false;

            BlinkAbilities.Clear();
            DisableAbilities.Clear();

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Add(Lotus);
        }
    }
}