namespace Evader.EvadableAbilities.Heroes.Pugna
{
    using System.Linq;

    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class NetherBlast : LinearAOE
    {
        #region Constructors and Destructors

        public NetherBlast(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);

            AdditionalDelay = Ability.AbilitySpecialData.First(x => x.Name == "delay").Value;
        }

        #endregion

        #region Methods

        protected override float GetCastRange()
        {
            return base.GetCastRange() + 350;
        }

        #endregion
    }
}