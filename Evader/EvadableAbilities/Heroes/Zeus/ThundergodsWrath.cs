namespace Evader.EvadableAbilities.Heroes.Zeus
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class ThundergodsWrath : NoObstacleAbility
    {
        public ThundergodsWrath(Ability ability)
            : base(ability)
        {
            DisableAbilities.AddRange(DisableAbilityNames);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.AddRange(Invis);
        }
    }
}