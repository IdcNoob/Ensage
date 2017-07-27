namespace Evader.EvadableAbilities.Heroes.QueenOfPain
{
    using Ensage;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class SonicWave : LinearProjectile
    {
        public SonicWave(Ability ability)
            : base(ability)
        {
            BlinkAbilities.AddRange(BlinkAbilityNames);
            DisableAbilities.AddRange(DisableAbilityNames);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(TricksOfTheTrade);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(Invul);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Add(NetherWard);
            CounterAbilities.AddRange(Invis);
        }

        protected override float GetCastRange()
        {
            return base.GetCastRange() + GetEndRadius();
        }
    }
}