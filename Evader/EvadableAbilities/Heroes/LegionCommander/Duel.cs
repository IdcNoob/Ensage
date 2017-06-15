namespace Evader.EvadableAbilities.Heroes.LegionCommander
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Common;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class Duel : LinearTarget, IModifier
    {
        public Duel(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(
                EnemyTeam,
                EvadableModifier.GetHeroType.ClosestToSource,
                maxDistanceToSource: Ability.GetRealCastRange() + 100);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(HurricanePike);
            CounterAbilities.AddRange(StrongDefUltimates);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Remove("item_glimmer_cape");

            Modifier.AllyCounterAbilities.Add(Enrage);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.Add(FalsePromise);
            Modifier.AllyCounterAbilities.AddRange(Invul);
            Modifier.AllyCounterAbilities.AddRange(VsPhys);
            Modifier.AllyCounterAbilities.Remove("item_glimmer_cape");

            Modifier.EnemyCounterAbilities.AddRange(VsPhys);
            Modifier.EnemyCounterAbilities.Add(FatesEdict);
            Modifier.EnemyCounterAbilities.AddRange(DisableAbilities);

            //leave invuls to use on allies
            Modifier.EnemyCounterAbilities.RemoveAll(x => Invul.Contains(x));
        }

        public EvadableModifier Modifier { get; }
    }
}