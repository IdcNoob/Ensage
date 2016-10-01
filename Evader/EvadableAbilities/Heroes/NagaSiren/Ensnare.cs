namespace Evader.EvadableAbilities.Heroes
{
    using Ensage;

    using static Core.Abilities;

    using Projectile = Base.Projectile;

    internal class Ensnare : Projectile
    {
        #region Constructors and Destructors

        public Ensnare(Ability ability)
            : base(ability)
        {
            ModifierName = "modifier_naga_siren_ensnare";

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(BallLightning);
        }

        #endregion
    }
}