namespace Evader.EvadableAbilities.Heroes.Invoker
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common;

    using static Data.AbilityNames;

    internal class SunStrike : AOE, IModifierObstacle
    {
        public SunStrike(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(HurricanePike);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);

            AdditionalDelay = Ability.AbilitySpecialData.First(x => x.Name == "delay").Value;
        }

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
    }
}