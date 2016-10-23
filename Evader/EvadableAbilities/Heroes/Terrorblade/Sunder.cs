namespace Evader.EvadableAbilities.Heroes.Terrorblade
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class Sunder : LinearTarget
    {
        #region Constructors and Destructors

        public Sunder(Ability ability)
            : base(ability)
        {
            BlinkAbilities.AddRange(BlinkAbilityNames);
            DisableAbilities.AddRange(DisableAbilityNames);

            CounterAbilities.Add(Lotus);
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(TricksOfTheTrade);
            CounterAbilities.Add("phantom_lancer_doppelwalk");
            CounterAbilities.Add("shadow_demon_disruption");
            CounterAbilities.Add("obsidian_destroyer_astral_imprisonment");
        }

        #endregion
    }
}