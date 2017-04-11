namespace Evader.EvadableAbilities.Heroes.Lina
{
    using System.Linq;

    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class LagunaBlade : LinearTarget
    {
        public LagunaBlade(Ability ability)
            : base(ability)
        {
            BlinkAbilities.AddRange(BlinkAbilityNames);
            DisableAbilities.AddRange(DisableAbilityNames);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(TricksOfTheTrade);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invul);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Lotus);
            CounterAbilities.Add(NetherWard);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);

            AdditionalDelay = Ability.AbilitySpecialData.First(x => x.Name == "damage_delay").Value + 0.05f;
        }

        protected override float GetCastRange()
        {
            return base.GetCastRange() + 150;
        }
    }
}