namespace Evader.EvadableAbilities.Heroes.Timbersaw
{
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class Chakram : LinearProjectile, IModifierObstacle, IModifier
    {
        public Chakram(Ability ability)
            : base(ability)
        {
            Name = "shredder_chakram"; // fix for aghanim chakram

            Modifier = new EvadableModifier(
                HeroTeam,
                EvadableModifier.GetHeroType.LowestHealth,
                ignoreRemainingTime: true);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsLowPureMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);

            Modifier.AllyCounterAbilities.AddRange(AllyShields);
        }

        public EvadableModifier Modifier { get; }

        public void AddModifierObstacle(Modifier modifier, Unit unit)
        {
            if (Obstacle != null)
            {
                End();
            }
        }
    }
}