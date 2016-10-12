namespace Evader.EvadableAbilities.Heroes
{
    using Ensage;

    using static Core.Abilities;

    using LinearProjectile = Base.LinearProjectile;

    internal class SonicWave : LinearProjectile
    {
        #region Constructors and Destructors

        public SonicWave(Ability ability)
            : base(ability)
        {
            BlinkAbilities.AddRange(BlinkAbilityNames);
            DisableAbilities.AddRange(DisableAbilityNames);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(TricksOfTheTrade);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(Invul);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(NetherWard);
            CounterAbilities.AddRange(Invis);
        }

        #endregion

        #region Methods

        protected override float GetCastRange()
        {
            return base.GetCastRange() + GetEndRadius();
        }

        #endregion
    }
}