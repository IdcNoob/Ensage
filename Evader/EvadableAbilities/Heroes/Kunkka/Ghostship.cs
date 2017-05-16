namespace Evader.EvadableAbilities.Heroes.Kunkka
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using Modifiers;

    using static Data.AbilityNames;

    internal class Ghostship : AOE, IModifier, IUnit
    {
        private readonly float shipRange;

        private Unit abilityUnit;

        public Ghostship(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(TricksOfTheTrade);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invul);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Remove("abaddon_aphotic_shield");

            Modifier.AllyCounterAbilities.Add(Enrage);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(Invul);
            Modifier.AllyCounterAbilities.AddRange(VsMagic);

            shipRange = ability.AbilitySpecialData.First(x => x.Name == "ghostship_distance").Value;
            AdditionalDelay = shipRange / Ability.GetProjectileSpeed();
        }

        public EvadableModifier Modifier { get; }

        public void AddUnit(Unit unit)
        {
            abilityUnit = unit;
            StartCast = Game.RawGameTime;
            EndCast = StartCast + AdditionalDelay;
            StartPosition = unit.Position.SetZ(350);
        }

        public override bool CanBeStopped()
        {
            return false;
        }

        public override void Check()
        {
            if (StartCast > 0 && Obstacle == null)
            {
                if (!IsAbilityUnitValid() || !abilityUnit.IsVisible)
                {
                    return;
                }

                var position = StartPosition.Extend(abilityUnit.Position.SetZ(350), shipRange);

                if (position.Distance2D(StartPosition) < 50)
                {
                    return;
                }

                StartPosition = position.SetZ(350);
                Obstacle = Pathfinder.AddObstacle(StartPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return StartCast + AdditionalDelay - Game.RawGameTime;
        }

        private bool IsAbilityUnitValid()
        {
            return abilityUnit != null && abilityUnit.IsValid;
        }
    }
}