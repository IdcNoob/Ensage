namespace Evader.EvadableAbilities.Heroes.TrollWarlord
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using UsableAbilities.Base;

    using static Data.AbilityNames;

    internal class WhirlingAxesMelee : AOE, IParticle, IModifier
    {
        private readonly float duration;

        private readonly float radius;

        public WhirlingAxesMelee(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            radius = Ability.AbilitySpecialData.First(x => x.Name == "max_range").Value - 75;
            duration = Ability.AbilitySpecialData.First(x => x.Name == "whirl_duration").Value;

            DisablePathfinder = true;

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.Add(HurricanePike);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);

            Modifier.AllyCounterAbilities.Add(Lotus);
            Modifier.AllyCounterAbilities.Add(Manta);
            Modifier.AllyCounterAbilities.AddRange(AllyPurges);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);
        }

        public EvadableModifier Modifier { get; }

        public void AddParticle(ParticleEffectAddedEventArgs particleArgs)
        {
            if (Obstacle != null || !AbilityOwner.IsVisible)
            {
                return;
            }

            StartCast = Game.RawGameTime;
            StartPosition = AbilityOwner.NetworkPosition;
            EndCast = StartCast + duration;
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
            else if (Obstacle != null)
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

            AbilityDrawer.DrawCircle(StartPosition, GetRadius());
            AbilityDrawer.UpdateCirclePosition(AbilityOwner.NetworkPosition);
        }

        public override bool IgnoreRemainingTime(UsableAbility ability, float remainingTime = 0)
        {
            return Obstacle != null;
        }

        protected override float GetRadius()
        {
            return radius;
        }
    }
}