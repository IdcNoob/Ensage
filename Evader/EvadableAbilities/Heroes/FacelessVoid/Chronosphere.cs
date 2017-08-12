namespace Evader.EvadableAbilities.Heroes.FacelessVoid
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Common;

    using Core;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    internal class Chronosphere : LinearAOE, IModifierObstacle
    {
        private readonly float[] duration = new float[3];

        private bool fowCast;

        private bool modifierAdded;

        public Chronosphere(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(StrongDefUltimates);
            CounterAbilities.Add(HurricanePike);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Bloodstone);

            CounterAbilities.Remove("slark_dark_pact");
            BlinkAbilities.Remove("slark_pounce");

            if (AbilityOwner.Team == Variables.HeroTeam)
            {
                // leave only blink abilities
                // if void is ally
                CounterAbilities.Clear();
                DisableAbilities.Clear();
            }

            for (var i = 0u; i < 3; i++)
            {
                duration[i] = ability.AbilitySpecialData.First(x => x.Name == "duration").GetValue(i);
            }

            ObstacleStays = true;
        }

        public void AddModifierObstacle(Modifier mod, Unit unit)
        {
            var position = unit.Position;
            modifierAdded = true;

            AbilityDrawer.Dispose(AbilityDrawer.Type.Rectangle);
            AbilityDrawer.DrawCircle(position, GetRadius());

            if (Obstacle == null)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + GetDuration();
                fowCast = true;
            }

            Obstacle = Pathfinder.AddObstacle(position, GetRadius(), Obstacle);
        }

        public override bool CanBeStopped()
        {
            return !modifierAdded && base.CanBeStopped();
        }

        public override void Check()
        {
            if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + CastPoint + GetDuration();
            }
            else if (StartCast > 0 && Obstacle == null && CanBeStopped() && !AbilityOwner.IsTurning())
            {
                StartPosition = AbilityOwner.InFront(-GetRadius() * 0.9f);
                EndPosition = AbilityOwner.InFront(GetCastRange() + GetRadius() * 0.8f);
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
        }

        public override void Draw()
        {
            if (Obstacle == null)
            {
                return;
            }

            if (modifierAdded)
            {
                AbilityDrawer.DrawTime(GetRemainingTime(), AbilityOwner.Position);
            }
            else
            {
                AbilityDrawer.DrawDoubleArcRectangle(StartPosition, EndPosition, GetRadius());
                AbilityDrawer.DrawTime(GetRemainingTime(), AbilityOwner.Position);
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
            return StartCast + (fowCast ? 0 : CastPoint) - Game.RawGameTime;
        }

        protected override float GetRadius()
        {
            return base.GetRadius() + 50;
        }

        private float GetDuration()
        {
            return duration[Ability.Level - 1];
        }
    }
}