namespace Evader.EvadableAbilities.Heroes
{
    using Base;

    using Ensage;

    using static Core.Abilities;

    internal class Laser : LinearTarget
    {
        #region Constructors and Destructors

        public Laser(Ability ability)
            : base(ability)
        {
            ModifierName = "modifier_tinker_laser_blind";
            //todo: tinker laser aghanim fix

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsLowPureMagic);
            CounterAbilities.Add(Lotus);
        }

        #endregion
    }
}