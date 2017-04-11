namespace Evader.EvadableAbilities.Heroes.ChaosKnight
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class RealityRift : LinearTarget
    {
        public RealityRift(Ability ability)
            : base(ability)
        {
            DisableAbilities.AddRange(DisableAbilityNames);
            BlinkAbilities.AddRange(BlinkAbilityNames);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Add(Lotus);
        }
    }
}