namespace Evader.EvadableAbilities.Heroes
{
    using Base;

    using Ensage;

    using static Core.Abilities;

    internal class Omnislash : LinearTarget
    {
        #region Constructors and Destructors

        public Omnislash(Ability ability)
            : base(ability)
        {
            ModifierName = "modifier_juggernaut_omnislash";

            BlinkAbilities.AddRange(BlinkAbilityNames);
            DisableAbilities.AddRange(DisableAbilityNames);

            CounterAbilities.Add(Lotus);
            CounterAbilities.Add(Manta);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
        }

        #endregion
    }
}