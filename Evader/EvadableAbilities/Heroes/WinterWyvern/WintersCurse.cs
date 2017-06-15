namespace Evader.EvadableAbilities.Heroes.WinterWyvern
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class WintersCurse : LinearAOE
    {
        public WintersCurse(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(StrongDefUltimates);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Add(Lotus);
            CounterAbilities.Add(HurricanePike);
        }
    }
}