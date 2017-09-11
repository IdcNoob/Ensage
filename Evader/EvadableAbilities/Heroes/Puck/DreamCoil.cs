namespace Evader.EvadableAbilities.Heroes.Puck
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    using Modifiers;

    using static Data.AbilityNames;

    internal class DreamCoil : AOE, IModifier, IParticle
    {
        private readonly float coilBreakRadius;

        public DreamCoil(Ability ability)
            : base(ability)
        {
            DisablePathfinder = true;

            CounterAbilities.Add(Eul);

            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            Modifier.AllyCounterAbilities.Add(Enrage);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(Invul);

            AdditionalDelay = Ability.AbilitySpecialData.First(x => x.Name == "coil_duration_scepter").Value;
            coilBreakRadius = Ability.AbilitySpecialData.First(x => x.Name == "coil_break_radius").Value;
        }

        public EvadableModifier Modifier { get; }

        public void AddParticle(ParticleEffectAddedEventArgs particleArgs)
        {
            DelayAction.Add(
                1,
                () =>
                    {
                        if (Hero.Modifiers.All(x => x.Name != "modifier_puck_coiled"))
                        {
                            return;
                        }

                        StartCast = Game.RawGameTime;
                        EndCast = StartCast + AdditionalDelay;
                        StartPosition = particleArgs.ParticleEffect.GetControlPoint(0);
                        Obstacle = Pathfinder.AddObstacle(StartPosition, GetRadius(), Obstacle);
                    });
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
            var pos = Hero.IsMoving ? Hero.InFront(60) : Hero.Position;

            if (pos.Distance2D(StartPosition) < 580)
            {
                return 0;
            }

            return 0.2f;
        }

        protected override float GetRadius()
        {
            return coilBreakRadius;
        }
    }
}