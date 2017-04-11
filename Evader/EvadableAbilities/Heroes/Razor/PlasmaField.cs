namespace Evader.EvadableAbilities.Heroes.Razor
{
    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    internal class PlasmaField : AOE, IUnit
    {
        private readonly float projectileSpeed;

        public PlasmaField(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);

            projectileSpeed = Ability.GetProjectileSpeed();
        }

        public void AddUnit(Unit unit)
        {
            if (!AbilityOwner.IsVisible || unit.Name != "npc_dota_plasma_field")
            {
                return;
            }

            StartCast = Game.RawGameTime;
            EndCast = StartCast + GetRadius() / projectileSpeed;
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

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            return StartCast + (hero.NetworkPosition.Distance2D(AbilityOwner.Position) - 150) / projectileSpeed
                   - Game.RawGameTime;
        }

        protected override float GetRadius()
        {
            return base.GetRadius() + 150;
        }
    }
}