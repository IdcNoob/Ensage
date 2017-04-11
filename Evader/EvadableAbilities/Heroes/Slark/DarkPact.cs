namespace Evader.EvadableAbilities.Heroes.Slark
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;

    using static Data.AbilityNames;

    internal class DarkPact : AOE, IModifierObstacle
    {
        public DarkPact(Ability ability)
            : base(ability)
        {
            DisablePathfinder = true;

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);

            AdditionalDelay = Ability.AbilitySpecialData.First(x => x.Name == "delay").Value;
        }

        public void AddModifierObstacle(Modifier modifier, Unit unit)
        {
            if (!AbilityOwner.IsVisible)
            {
                return;
            }

            StartCast = Game.RawGameTime;
            EndCast = StartCast + AdditionalDelay;
            StartPosition = unit.Position;
            Obstacle = Pathfinder.AddObstacle(StartPosition, GetRadius(), Obstacle);
        }

        public override void Check()
        {
            if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
            else if (Obstacle != null && !IsInPhase)
            {
                Pathfinder.UpdateObstacle(Obstacle.Value, AbilityOwner.NetworkPosition, GetRadius());
            }
        }

        public override void Draw()
        {
            if (Obstacle == null)
            {
                return;
            }

            var position = AbilityOwner.NetworkPosition;

            AbilityDrawer.DrawTime(GetRemainingTime(), position);
            AbilityDrawer.DrawCircle(position, GetRadius());

            AbilityDrawer.UpdateCirclePosition(position);
        }
    }
}