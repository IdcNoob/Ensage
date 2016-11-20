namespace Evader.EvadableAbilities.Heroes.Timbersaw
{
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class Chakram : LinearProjectile, IModifierObstacle, IModifier
    {
        #region Constructors and Destructors

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

        #endregion

        #region Public Properties

        public EvadableModifier Modifier { get; }

        #endregion

        #region Public Methods and Operators

        public void AddModifierObstacle(Modifier modifier, Unit unit)
        {
            if (Obstacle != null)
            {
                End();
            }
        }

        #endregion
    }
}