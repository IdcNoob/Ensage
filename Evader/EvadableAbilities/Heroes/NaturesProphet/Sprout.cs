namespace Evader.EvadableAbilities.Heroes.NaturesProphet
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common;

    using UsableAbilities.Base;

    internal class Sprout : AOE, IParticle
    {
        private readonly float[] duration = new float[4];

        public Sprout(Ability ability)
            : base(ability)
        {
            DisablePathfinder = true;

            BlinkAbilities.Clear();

            CounterAbilities.Add("item_quelling_blade");
            CounterAbilities.Add("item_iron_talon");
            CounterAbilities.Add("item_bfury");
            CounterAbilities.Add("item_tango_single");
            CounterAbilities.Add("item_tango");

            for (var i = 0u; i < duration.Length; i++)
            {
                duration[i] = ability.AbilitySpecialData.First(x => x.Name == "duration").GetValue(i);
            }
        }

        public void AddParticle(ParticleEffectAddedEventArgs particleArgs)
        {
            StartCast = Game.RawGameTime;

            DelayAction.Add(
                1,
                () =>
                    {
                        StartPosition = particleArgs.ParticleEffect.GetControlPoint(0);
                        EndCast = StartCast + GetDuration();
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

        protected override float GetRadius()
        {
            return 250;
        }

        private float GetDuration()
        {
            return duration[Ability.Level - 1];
        }
    }
}