namespace Evader.EvadableAbilities.Heroes.NyxAssassin
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class Vendetta : EvadableAbility, IModifier
    {
        public Vendetta(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(EnemyTeam, EvadableModifier.GetHeroType.ModifierSource);

            Modifier.EnemyCounterAbilities.AddRange(DisableAbilityNames);
            Modifier.EnemyCounterAbilities.AddRange(VsPhys);
        }

        public EvadableModifier Modifier { get; }

        public override void Check()
        {
        }

        public override void Draw()
        {
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return 0;
        }
    }
}