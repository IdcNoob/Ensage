namespace Evader.EvadableAbilities.Heroes.Zeus
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions.SharpDX;

    using UsableAbilities.Base;

    using static Data.AbilityNames;

    internal class Nimbus : AOE, IParticle
    {
        private readonly float duration;

        private Unit cloud;

        public Nimbus(Ability ability)
            : base(ability)
        {
            DisablePathfinder = true;

            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);

            duration = Ability.AbilitySpecialData.First(x => x.Name == "cloud_duration").Value;
        }

        public void AddParticle(ParticleEffectAddedEventArgs particleArgs)
        {
            StartCast = Game.RawGameTime;

            DelayAction.Add(
                1,
                () =>
                    {
                        StartPosition = particleArgs.ParticleEffect.GetControlPoint(0).SetZ(AbilityOwner.Position.Z);
                        EndCast = StartCast + duration;
                        Obstacle = Pathfinder.AddObstacle(StartPosition, GetRadius(), Obstacle);
                    });
        }

        public override bool CanBeStopped()
        {
            return false;
        }

        public override void Check()
        {
            if (StartCast <= 0)
            {
                return;
            }

            if (cloud == null)
            {
                cloud = ObjectManager.GetEntitiesFast<Unit>()
                    .FirstOrDefault(
                        x => x.IsValid && x.NetworkName == "CDOTA_Unit_ZeusCloud" && x.Team == AbilityOwner.Team);
            }

            if (Obstacle != null && (Game.RawGameTime > EndCast || cloud != null && cloud.IsValid && !cloud.IsAlive))
            {
                End();
            }
        }

        public override void End()
        {
            base.End();
            cloud = null;
        }

        public override float GetRemainingDisableTime()
        {
            return 0;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return EndCast - Game.RawGameTime;
        }

        public override bool IgnoreRemainingTime(UsableAbility ability, float remainingTime = 0)
        {
            return true;
        }
    }
}