namespace Evader.EvadableAbilities.Heroes.StormSpirit
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    internal class StaticRemnant : AOE, IModifierObstacle
    {
        public StaticRemnant(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);

            AdditionalDelay = Ability.AbilitySpecialData.First(x => x.Name == "static_remnant_delay").Value;
        }

        public void AddModifierObstacle(Modifier modifier, Unit unit)
        {
            StartPosition = unit.Position;

            if (!ObjectManager.GetEntities<Creep>()
                    .Any(
                        x => x.IsValid && x.Distance2D(StartPosition) < GetRadius() - 25 && x.Team == HeroTeam
                             && x.IsAlive && x.IsSpawned))
            {
                return;
            }

            StartCast = Game.RawGameTime;
            EndCast = StartCast + AdditionalDelay;
            Obstacle = Pathfinder.AddObstacle(StartPosition, GetRadius(), Obstacle);
        }

        public override bool CanBeStopped()
        {
            return false;
        }

        public override void Check()
        {
            if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
        }

        public override float GetRemainingDisableTime()
        {
            return 0;
        }
    }
}