namespace Evader.EvadableAbilities.Heroes
{
    using Ensage;

    using static Core.Abilities;

    using Projectile = Base.Projectile;

    internal class LifeBreak : Projectile
    {
        #region Constructors and Destructors

        public LifeBreak(Ability ability)
            : base(ability)
        {
            IsDisjointable = false;

            BlinkAbilities.AddRange(BlinkAbilityNames);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invis);
        }

        #endregion
    }
}