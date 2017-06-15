namespace Evader.EvadableAbilities.Heroes.Leshrac
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Common;

    using Ensage;
    using Ensage.Common.Extensions;

    using Modifiers;

    using static Data.AbilityNames;

    internal class SplitEarth : LinearAOE, IModifierObstacle, IModifier
    {
        private bool fowCast;

        private bool modifierAdded;

        public SplitEarth(Ability ability)
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
            CounterAbilities.Remove("abaddon_aphotic_shield");

            Modifier.AllyCounterAbilities.Add(Enrage);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(Invul);
            Modifier.AllyCounterAbilities.AddRange(VsMagic);

            AdditionalDelay = Ability.AbilitySpecialData.First(x => x.Name == "delay").Value + 0.05f;
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

        protected override float GetRadius()
        {
            return Ability.GetRadius() + 60;
        }
    }
}