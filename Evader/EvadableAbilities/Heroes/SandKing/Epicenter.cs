namespace Evader.EvadableAbilities.Heroes.SandKing
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class Epicenter : NoObstacleAbility, IModifier
    {
        public Epicenter(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            DisableAbilities.AddRange(DisableAbilityNames);

            Modifier.AllyCounterAbilities.Add(PhaseShift);
            Modifier.AllyCounterAbilities.Add(Eul);
            Modifier.AllyCounterAbilities.Add(TricksOfTheTrade);
            Modifier.AllyCounterAbilities.AddRange(VsDamage);
            Modifier.AllyCounterAbilities.AddRange(VsMagic);
            Modifier.AllyCounterAbilities.AddRange(Invul);
            Modifier.AllyCounterAbilities.Add(SnowBall);
            Modifier.AllyCounterAbilities.Add(Armlet);
            Modifier.AllyCounterAbilities.AddRange(Invis);

            AdditionalDelay = Ability.GetChannelTime(0);
        }

        public EvadableModifier Modifier { get; }

        public override void Check()
        {
            if (StartCast <= 0 && Ability.IsChanneling && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + AdditionalDelay;
                Obstacle = Handle;
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
        }

        public override float GetRemainingDisableTime()
        {
            return GetRemainingTime() - 0.1f;
        }
    }
}