namespace Evader.EvadableAbilities.Heroes.Bloodseeker
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Common;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class BloodRite : LinearAOE, IModifierObstacle, IModifier
    {
        private bool fowCast;

        private bool modifierAdded;

        public BloodRite(Ability ability)
            : base(ability)
        {
            DisablePathfinder = true;

            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(TricksOfTheTrade);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Remove("abaddon_aphotic_shield");

            Modifier.AllyCounterAbilities.Add(Lotus);
            Modifier.AllyCounterAbilities.Add(Eul);
            Modifier.AllyCounterAbilities.Add(Manta);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);

            AdditionalDelay = Ability.AbilitySpecialData.First(x => x.Name == "delay").Value;
        }

        public EvadableModifier Modifier { get; }

        public void AddModifierObstacle(Modifier mod, Unit unit)
        {
            var position = unit.Position;
            modifierAdded = true;

            AbilityDrawer.Dispose(AbilityDrawer.Type.Rectangle);
            AbilityDrawer.DrawCircle(position, GetRadius());

            if (Obstacle == null)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + AdditionalDelay;
                fowCast = true;
            }

            Obstacle = Pathfinder.AddObstacle(position, GetRadius(), Obstacle);
        }

        public override bool CanBeStopped()
        {
            return !modifierAdded && base.CanBeStopped();
        }

        public override void Draw()
        {
            if (Obstacle == null)
            {
                return;
            }

            AbilityDrawer.DrawTime(GetRemainingTime(), StartPosition);

            if (!modifierAdded)
            {
                AbilityDrawer.DrawDoubleArcRectangle(StartPosition, EndPosition, GetRadius());
            }
        }

        public override void End()
        {
            if (Obstacle == null)
            {
                return;
            }

            base.End();
            modifierAdded = false;
            fowCast = false;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return StartCast + (fowCast ? 0 : CastPoint) + AdditionalDelay - Game.RawGameTime;
        }
    }
}