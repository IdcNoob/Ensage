namespace Evader.EvadableAbilities.Heroes.Invoker
{
    using System.Linq;

    using Base.Interfaces;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class ChaosMeteor : LinearProjectile, IParticle
    {
        private readonly Ability wex;

        public ChaosMeteor(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);

            AdditionalDelay = Ability.AbilitySpecialData.First(x => x.Name == "land_time").Value - 0.5f;
            wex = AbilityOwner.FindSpell("invoker_wex");
        }

        public void AddParticle(ParticleEffectAddedEventArgs particleArgs)
        {
            StartCast = Game.RawGameTime + AdditionalDelay;

            DelayAction.Add(
                1,
                () =>
                    {
                        var cp0 = particleArgs.ParticleEffect.GetControlPoint(0).SetZ(350);
                        var cp1 = particleArgs.ParticleEffect.GetControlPoint(1).SetZ(350);
                        StartPosition = cp1.Extend(cp0, GetRadius() / 2);
                        EndPosition = cp0.Extend(StartPosition, GetCastRange() + cp0.Distance2D(cp1));
                        EndCast = StartCast + AdditionalDelay + GetCastRange() / GetProjectileSpeed() - 0.5f;
                        Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
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

        public override void Draw()
        {
            if (Obstacle == null)
            {
                return;
            }

            AbilityDrawer.DrawTime(GetRemainingTime(), StartPosition);
            AbilityDrawer.DrawDoubleArcRectangle(StartPosition, EndPosition, GetRadius(), GetEndRadius());
            AbilityDrawer.DrawCircle(StartPosition, (GetRadius() + GetEndRadius()) / 2);

            AbilityDrawer.UpdateCirclePosition(GetProjectilePosition());
        }

        protected override float GetCastRange()
        {
            return (350 + (wex?.Level ?? 8) * 155) * 1.2f;
        }
    }
}