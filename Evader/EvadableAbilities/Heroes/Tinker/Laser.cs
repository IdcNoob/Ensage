namespace Evader.EvadableAbilities.Heroes.Tinker
{
    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using Modifiers;

    using static Data.AbilityNames;

    internal class Laser : LinearTarget, IModifier
    {
        #region Constructors and Destructors

        public Laser(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsLowPureMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Add(Lotus);

            Modifier.AllyCounterAbilities.Add(Lotus);
            Modifier.AllyCounterAbilities.Add(Manta);
            Modifier.AllyCounterAbilities.AddRange(AllyPurges);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);
        }

        #endregion

        #region Public Properties

        public EvadableModifier Modifier { get; }

        #endregion

        #region Methods

        protected override float GetCastRange()
        {
            return base.GetCastRange() + (AbilityOwner.AghanimState() ? GetRadius() : 0);
        }

        protected override float GetRadius()
        {
            return AbilityOwner.AghanimState() ? 400 : base.GetRadius();
        }

        #endregion
    }
}