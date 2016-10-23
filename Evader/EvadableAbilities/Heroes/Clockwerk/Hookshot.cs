namespace Evader.EvadableAbilities.Heroes.Clockwerk
{
    using Ensage;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class Hookshot : LinearProjectile
    {
        #region Constructors and Destructors

        public Hookshot(Ability ability)
            : base(ability)
        {
            //todo: add particle support

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.AddRange(Invis);
        }

        #endregion

        #region Methods

        protected override float GetRadius()
        {
            return base.GetRadius() + 60;
        }

        #endregion
    }
}