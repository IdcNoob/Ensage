namespace Evader.EvadableAbilities.Heroes.AncientApparition
{
    using System;
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using Modifiers;

    using SharpDX;

    using static Data.AbilityNames;

    internal class IceBlast : AOE, IModifier, IUnit, IParticle

    {
        private readonly float growRadius;

        private readonly float maxRadius;

        private readonly float minRadius;

        private Unit abilityUnit;

        private bool flies;

        private Vector3 initialPosition;

        public IceBlast(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealthPct);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(TricksOfTheTrade);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(Invul);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Bloodstone);

            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(VsMagic);
            Modifier.AllyCounterAbilities.Remove("legion_commander_press_the_attack");
            Modifier.AllyCounterAbilities.Remove("treant_living_armor");

            minRadius = Ability.AbilitySpecialData.First(x => x.Name == "radius_min").Value;
            maxRadius = Ability.AbilitySpecialData.First(x => x.Name == "radius_max").Value;
            growRadius = Ability.AbilitySpecialData.First(x => x.Name == "radius_grow").Value;
        }

        public EvadableModifier Modifier { get; }

        public void AddParticle(ParticleEffectAddedEventArgs particleArgs)
        {
            if (particleArgs.Name.Contains("final"))
            {
                flies = true;
            }
            else
            {
                flies = false;
                End();
            }
        }

        public void AddUnit(Unit unit)
        {
            abilityUnit = unit;
            StartCast = Game.RawGameTime;
            initialPosition = unit.Position;
        }

        public override bool CanBeStopped()
        {
            return false;
        }

        public override void Check()
        {
            if (IsAbilityUnitValid())
            {
                StartPosition = abilityUnit.Position;
            }
            else if (flies && Obstacle == null)
            {
                StartPosition = StartPosition.Extend(initialPosition, -115);
                Obstacle = Pathfinder.AddObstacle(StartPosition, GetRadius(), Obstacle);
                EndCast = Game.RawGameTime;
            }
        }

        public override float GetRemainingDisableTime()
        {
            return 0;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return Math.Min(2, StartPosition.Distance2D(initialPosition) / 750) * 0.7f - (Game.RawGameTime - EndCast);
        }

        protected override float GetRadius()
        {
            return Math.Min(maxRadius, Math.Max((EndCast - StartCast) * growRadius + minRadius, minRadius)) * 1.5f;
        }

        private bool IsAbilityUnitValid()
        {
            return abilityUnit != null && abilityUnit.IsValid;
        }
    }
}