namespace Evader.EvadableAbilities.Heroes.Kunkka
{
    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using Modifiers;

    using static Data.AbilityNames;

    internal class Ghostship : AOE, IModifier, IUnit
    {
        private readonly float additionalAghanimDelay;

        private readonly float castRange;

        private Unit abilityUnit;

        private bool aghanimState;

        private bool fowCast;

        public Ghostship(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(HurricanePike);
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

            castRange = Ability.GetCastRange();

            AdditionalDelay = 3.1f;
            additionalAghanimDelay = 1.6f;
        }

        public EvadableModifier Modifier { get; }

        private bool AghanimState
        {
            get
            {
                //cache aghanim for fow casts
                if (AbilityOwner.IsVisible)
                {
                    aghanimState = AbilityOwner.AghanimState();
                }
                return aghanimState;
            }
        }

        public void AddUnit(Unit unit)
        {
            abilityUnit = unit;
            StartCast = Game.RawGameTime;
            EndCast = StartCast + GetDelay();
            StartPosition = unit.Position.SetZ(350);
            fowCast = true;
        }

        public override bool CanBeStopped()
        {
            return !fowCast && base.CanBeStopped();
        }

        public override void Check()
        {
            if (fowCast && Obstacle == null)
            {
                if (!IsAbilityUnitValid() || !abilityUnit.IsVisible)
                {
                    return;
                }

                var position = StartPosition.Extend(abilityUnit.Position.SetZ(350), GetShipRange());

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

        public override void End()
        {
            if (Obstacle == null)
            {
                return;
            }

            base.End();
            fowCast = false;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return StartCast + (fowCast ? 0 : CastPoint) + GetDelay() - Game.RawGameTime;
        }

        private float GetDelay()
        {
            return AghanimState ? additionalAghanimDelay : AdditionalDelay;
        }

        private float GetShipRange()
        {
            return AghanimState ? castRange : castRange * 2;
        }

        private bool IsAbilityUnitValid()
        {
            return abilityUnit != null && abilityUnit.IsValid;
        }
    }
}