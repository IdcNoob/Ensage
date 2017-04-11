namespace Evader.EvadableAbilities.Heroes.Juggernaut
{
    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using Modifiers;

    using static Data.AbilityNames;

    internal class BladeFury : EvadableAbility, IModifier
    {
        public BladeFury(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(
                EnemyTeam,
                EvadableModifier.GetHeroType.ClosestToSource,
                maxDistanceToSource: Ability.GetRadius());

            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(VsMagic);
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