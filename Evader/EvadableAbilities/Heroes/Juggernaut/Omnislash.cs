namespace Evader.EvadableAbilities.Heroes
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using static Core.Abilities;

    internal class Omnislash : LinearTarget, IModifier
    {
        #region Fields

        private Modifier modifier;

        private Hero modifierHero;

        #endregion

        #region Constructors and Destructors

        public Omnislash(Ability ability)
            : base(ability)
        {
            ModifierName = "modifier_juggernaut_omnislash";

            ModifierAllyCounter.Add(PhaseShift);
            ModifierEnemyCounter.Add(PhaseShift);

            BlinkAbilities.AddRange(BlinkAbilityNames);
            DisableAbilities.AddRange(DisableAbilityNames);

            CounterAbilities.Add(Lotus);
            CounterAbilities.Add(Manta);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
        }

        #endregion

        #region Public Methods and Operators

        public void AddModifer(Modifier modifier, Hero hero)
        {
            this.modifier = modifier;
            modifierHero = hero;
        }

        public bool CanBeCountered()
        {
            return modifier != null && modifier.IsValid;
        }

        public float GetModiferRemainingTime()
        {
            return modifier.RemainingTime;
        }

        public Hero GetModifierHero()
        {
            return modifierHero;
        }

        public void RemoveModifier()
        {
            modifier = null;
            modifierHero = null;
        }

        #endregion
    }
}