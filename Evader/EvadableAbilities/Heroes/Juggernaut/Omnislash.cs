namespace Evader.EvadableAbilities.Heroes.Juggernaut
{
    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using Modifiers;
    using Modifiers.Special;

    using static Data.AbilityNames;

    internal class Omnislash : LinearTarget, IModifier
    {
        #region Constructors and Destructors

        public Omnislash(Ability ability)
            : base(ability)
        {
            Modifier = new OmnislashModifier(
                EnemyTeam,
                EvadableModifier.GetHeroType.ClosestToSource,
                Ability,
                maxDistanceToSource: Ability.GetRadius());

            BlinkAbilities.AddRange(BlinkAbilityNames);
            DisableAbilities.AddRange(DisableAbilityNames);

            CounterAbilities.Add(Lotus);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Remove("item_blade_mail");

            Modifier.AllyCounterAbilities.Add(PhaseShift);
            Modifier.AllyCounterAbilities.Add(Eul);
            Modifier.AllyCounterAbilities.AddRange(Invul);
            Modifier.AllyCounterAbilities.Add(Manta);
            Modifier.AllyCounterAbilities.AddRange(VsDamage);
            Modifier.AllyCounterAbilities.AddRange(VsPhys);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);
        }

        #endregion

        #region Public Properties

        public EvadableModifier Modifier { get; }

        #endregion
    }
}