namespace Evader.EvadableAbilities.Heroes.Kunkka
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    using Modifiers;

    using static Data.AbilityNames;

    internal class Torrent : AOE, IModifierObstacle, IModifier
    {
        private readonly float bonusRadius;

        private readonly Ability talent;

        public Torrent(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(HurricanePike);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);

            Modifier.AllyCounterAbilities.Add(Enrage);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(Invul);
            Modifier.AllyCounterAbilities.AddRange(VsMagic);

            AdditionalDelay = Ability.AbilitySpecialData.First(x => x.Name == "delay").Value;

            talent = AbilityOwner.FindSpell("special_bonus_unique_kunkka");

            if (talent != null)
            {
                bonusRadius = talent.AbilitySpecialData.First(x => x.Name == "value").Value;
            }
        }

        public EvadableModifier Modifier { get; }

        public void AddModifierObstacle(Modifier mod, Unit unit)
        {
            StartCast = Game.RawGameTime;

            DelayAction.Add(
                1,
                () =>
                    {
                        StartPosition = unit.Position;
                        EndCast = StartCast + AdditionalDelay;
                        Obstacle = Pathfinder.AddObstacle(StartPosition, GetRadius(), Obstacle);
                    });
        }

        public override bool CanBeStopped()
        {
            return false;
        }

        public override void Check()
        {
            if (StartCast > 0 && Obstacle != null && Game.RawGameTime > EndCast)
            {
                End();
            }
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return EndCast - Game.RawGameTime;
        }

        protected override float GetRadius()
        {
            return base.GetRadius() + (talent?.Level > 0 ? bonusRadius : 0);
        }
    }
}