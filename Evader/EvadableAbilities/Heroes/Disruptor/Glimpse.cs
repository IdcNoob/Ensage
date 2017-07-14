namespace Evader.EvadableAbilities.Heroes.Disruptor
{
    using System;
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    using SharpDX;

    using static Data.AbilityNames;

    internal class Glimpse : EvadableAbility, IParticle
    {
        private Vector3 endPosition;

        private Vector3 heroPosition;

        private Vector3 startPosition;

        public Glimpse(Ability ability)
            : base(ability)
        {
            DisablePathfinder = true;

            DisableAbilities.Clear();
            BlinkAbilities.Clear();
            BlinkAbilities.Add("sandking_burrowstrike");
            BlinkAbilities.Add("phantom_lancer_doppelwalk");
            BlinkAbilities.Add("ember_spirit_activate_fire_remnant");

            CounterAbilities.Add(Manta);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(TricksOfTheTrade);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invul);
            CounterAbilities.Add(PhaseShift);
        }

        public void AddParticle(ParticleEffectAddedEventArgs particleArgs)
        {
            StartCast = Game.RawGameTime;

            DelayAction.Add(
                1,
                () =>
                    {
                        startPosition = particleArgs.ParticleEffect.GetControlPoint(0);
                        endPosition = particleArgs.ParticleEffect.GetControlPoint(1);
                        heroPosition = startPosition;
                        Obstacle = Pathfinder.AddObstacle(startPosition, 200, Obstacle, 256);
                        EndCast = StartCast + GetRemainingTime();
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
            else if (Obstacle != null)
            {
                var hero = ObjectManager.GetEntities<Hero>()
                    .Where(
                        x => x.IsValid && x.Team == HeroTeam && x.IsAlive && !x.IsMagicImmune()
                             && x.Distance2D(heroPosition) < 75)
                    .OrderBy(x => x.Distance2D(startPosition))
                    .FirstOrDefault();

                if (hero != null)
                {
                    heroPosition = hero.NetworkPosition;
                    Pathfinder.UpdateObstacle(Obstacle.Value, heroPosition, 75, (int)hero.Position.Z);
                }
            }
        }

        public override void Draw()
        {
            if (Obstacle != null)
            {
                AbilityDrawer.DrawTime(GetRemainingTime(), heroPosition);
            }
        }

        public override float GetRemainingDisableTime()
        {
            return 0;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return Math.Min(1.8f, startPosition.Distance2D(endPosition) / 600) - (Game.RawGameTime - StartCast);
        }
    }
}