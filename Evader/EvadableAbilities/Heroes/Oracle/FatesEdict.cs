﻿namespace Evader.EvadableAbilities.Heroes.Oracle
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class FatesEdict : EvadableAbility, IModifier
    {
        public FatesEdict(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.ModifierSource);

            Modifier.AllyCounterAbilities.Add(Lotus);
            Modifier.AllyCounterAbilities.Add(Manta);
            Modifier.AllyCounterAbilities.AddRange(AllyPurges);
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