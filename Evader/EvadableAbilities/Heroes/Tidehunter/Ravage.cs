namespace Evader.EvadableAbilities.Heroes.Tidehunter
{
    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using Modifiers;

    using static Data.AbilityNames;

    internal sealed class Ravage : AOE, IModifier
    {
        private readonly float projectileSpeed;

        private readonly float tavelTime;

        private readonly float width;

        public Ravage(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            projectileSpeed = ability.GetProjectileSpeed() + 100;
            tavelTime = GetRadius() / projectileSpeed;
            width = 350;

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(StrongDefUltimates);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(HurricanePike);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Remove("abaddon_aphotic_shield");

            Modifier.AllyCounterAbilities.Add(Enrage);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(Invul);
            Modifier.AllyCounterAbilities.AddRange(VsMagic);
        }

        public EvadableModifier Modifier { get; }

        public override void Check()
        {
            var time = Game.RawGameTime;

            if (IsInPhase && StartCast + CastPoint <= time)
            {
                StartCast = time;
                StartPosition = AbilityOwner.NetworkPosition;
                EndCast = StartCast + CastPoint + tavelTime;
                Obstacle = Pathfinder.AddObstacle(StartPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && time > EndCast)
            {
                End();
            }
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            if (IsInPhase && hero.NetworkPosition.Distance2D(StartPosition) < width)
            {
                return StartCast + CastPoint - Game.RawGameTime;
            }

            return StartCast + CastPoint + (hero.NetworkPosition.Distance2D(StartPosition) - width) / projectileSpeed
                   - Game.RawGameTime;
        }
    }
}