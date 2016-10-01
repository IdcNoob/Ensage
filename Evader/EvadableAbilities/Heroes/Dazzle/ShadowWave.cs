namespace Evader.EvadableAbilities.Heroes
{
    using Base;

    using Ensage;

    using static Core.Abilities;

    internal class ShadowWave : LinearAOE
    {
        #region Constructors and Destructors

        public ShadowWave(Ability ability)
            : base(ability)
        {
            //todo check radius

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            //todo check fist vs wave
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.Add("item_ghost");
            CounterAbilities.Add("item_buckler");
        }

        #endregion
    }
}