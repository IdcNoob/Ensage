namespace Evader.EvadableAbilities.Heroes.DeathProphet
{
    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using Modifiers;

    using static Data.AbilityNames;

    internal class Exorcism : NoObstacleAbility, IModifier
    {
        #region Constructors and Destructors

        public Exorcism(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(
                EnemyTeam,
                EvadableModifier.GetHeroType.ClosestToSource,
                maxDistanceToSource: Ability.GetRadius());

            DisableAbilities.AddRange(DisableAbilityNames);

            Modifier.EnemyCounterAbilities.AddRange(DisableAbilityNames);
            Modifier.EnemyCounterAbilities.Remove(Eul);

            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(VsPhys);
        }

        #endregion

        #region Public Properties

        public EvadableModifier Modifier { get; }

        #endregion
    }
}