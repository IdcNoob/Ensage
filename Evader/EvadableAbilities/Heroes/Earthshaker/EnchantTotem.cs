namespace Evader.EvadableAbilities.Heroes.Earthshaker
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class EnchantTotem : AOE
    {
        public EnchantTotem(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsLowDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.Add(HurricanePike);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Remove("abaddon_aphotic_shield");
        }
    }
}