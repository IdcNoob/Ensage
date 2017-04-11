namespace Evader.EvadableAbilities.Heroes.Leshrac
{
    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using Modifiers;

    using static Data.AbilityNames;

    internal class PulseNova : EvadableAbility, IModifier
    {
        public PulseNova(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(
                EnemyTeam,
                EvadableModifier.GetHeroType.ClosestToSource,
                maxDistanceToSource: Ability.GetRadius() + 50,
                ignoreRemainingTime: true);

            Modifier.AllyCounterAbilities.AddRange(VsDamage);
            Modifier.AllyCounterAbilities.AddRange(VsMagic);

            Modifier.AllyCounterAbilities.Remove("phantom_lancer_doppelwalk");
            Modifier.AllyCounterAbilities.Remove("life_stealer_rage");
            Modifier.AllyCounterAbilities.Remove("juggernaut_blade_fury");
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