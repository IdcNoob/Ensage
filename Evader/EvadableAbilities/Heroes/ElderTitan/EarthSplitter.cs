namespace Evader.EvadableAbilities.Heroes
{
    using System.Linq;

    using Base;

    using Ensage;

    using static Core.Abilities;

    internal class EarthSplitter : LinearAOE
    {
        #region Constructors and Destructors

        public EarthSplitter(Ability ability)
            : base(ability)
        {
            BlinkAbilities.AddRange(BlinkAbilityNames);
            DisableAbilities.AddRange(DisableAbilityNames);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(TricksOfTheTrade);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(Invul);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.AddRange(Invis);

            AdditionalDelay = Ability.AbilitySpecialData.First(x => x.Name == "crack_time").Value;
        }

        #endregion
    }
}