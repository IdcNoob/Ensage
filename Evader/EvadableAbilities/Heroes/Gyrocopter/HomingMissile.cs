namespace Evader.EvadableAbilities.Heroes.Gyrocopter
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class HomingMissile : EvadableAbility, IModifier
    {
        public HomingMissile(Ability ability)
            : base(ability)
        {
            //todo improve ?

            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.ModifierSource);
            DisableTimeSinceCastCheck = true;

            Modifier.AllyCounterAbilities.Add(Enrage);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(Invul);
            Modifier.AllyCounterAbilities.AddRange(VsPhys);
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