namespace Evader.EvadableAbilities.Heroes.Terrorblade
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class Metamorphosis : EvadableAbility, IModifier
    {
        public Metamorphosis(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(EnemyTeam, EvadableModifier.GetHeroType.ModifierSource);

            Modifier.EnemyCounterAbilities.Add(Decrepify);
            Modifier.EnemyCounterAbilities.Add(FatesEdict);
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