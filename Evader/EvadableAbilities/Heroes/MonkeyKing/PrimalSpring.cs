namespace Evader.EvadableAbilities.Heroes.MonkeyKing
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using SharpDX;

    using static Data.AbilityNames;

    internal class PrimalSpring : AOE, IParticle, IModifierObstacle
    {
        private readonly float channelTime;

        private readonly float speed;

        private Vector3 initialPosition;

        public PrimalSpring(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);

            speed = Ability.GetAbilityDataByName("monkey_king_tree_dance")
                .AbilitySpecialData.First(x => x.Name == "spring_leap_speed")
                .Value;
            channelTime = ability.GetChannelTime(0);
        }

        public void AddModifierObstacle(Modifier modifier, Unit unit)
        {
            DelayAction.Add(
                5,
                () =>
                    {
                        StartPosition = unit.Position;
                        EndCast = StartCast + (channelTime - modifier.ElapsedTime)
                                  + StartPosition.Distance2D(initialPosition) / speed;
                        Obstacle = Pathfinder.AddObstacle(StartPosition, GetRadius(), Obstacle);
                    });
        }

        public void AddParticle(ParticleEffectAddedEventArgs particleArgs)
        {
            if (particleArgs.Name.Contains("channel"))
            {
                StartCast = Game.RawGameTime;
                DelayAction.Add(1, () => initialPosition = particleArgs.ParticleEffect.GetControlPoint(0).SetZ(350));
            }
            else
            {
                EndCast = Game.RawGameTime + StartPosition.Distance2D(initialPosition) / speed;
            }
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

        public override float GetRemainingDisableTime()
        {
            return 0;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return EndCast - Game.RawGameTime;
        }
    }
}