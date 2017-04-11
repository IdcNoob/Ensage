namespace Evader.EvadableAbilities.Heroes.Phoenix
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common;

    using Modifiers;

    using static Data.AbilityNames;

    internal class Supernova : AOE, IModifier, IParticle
    {
        private readonly float radius;

        public Supernova(Ability ability)
            : base(ability)
        {
            DisablePathfinder = true;

            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(Armlet);

            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(Invul);
            Modifier.AllyCounterAbilities.AddRange(VsMagic);

            AdditionalDelay = Ability.AbilitySpecialData.First(x => x.Name == "tooltip_duration").Value;
            radius = Ability.AbilitySpecialData.First(x => x.Name == "aura_radius").Value;
        }

        public EvadableModifier Modifier { get; }

        public void AddParticle(ParticleEffectAddedEventArgs particleArgs)
        {
            StartCast = Game.RawGameTime;

            DelayAction.Add(
                1,
                () =>
                    {
                        StartPosition = particleArgs.ParticleEffect.GetControlPoint(0);
                        EndCast = StartCast + AdditionalDelay + 0.05f;
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

        protected override float GetRadius()
        {
            return radius;
        }
    }
}