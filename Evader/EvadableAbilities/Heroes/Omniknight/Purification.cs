namespace Evader.EvadableAbilities.Heroes.Omniknight
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class Purification : LinearAOE
    {
        public Purification(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsLowPureMagic);
            CounterAbilities.AddRange(Invul);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }

        protected override float GetCastRange()
        {
            return base.GetCastRange() + 200;
        }
    }
}