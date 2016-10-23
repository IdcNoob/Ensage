namespace Evader.EvadableAbilities.Heroes.Tidehunter
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class AnchorSmash : AOE
    {
        #region Constructors and Destructors

        public AnchorSmash(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.Add("item_ghost");
            CounterAbilities.Add("item_buckler");
        }

        #endregion
    }
}