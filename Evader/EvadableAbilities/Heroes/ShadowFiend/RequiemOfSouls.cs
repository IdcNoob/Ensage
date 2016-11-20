namespace Evader.EvadableAbilities.Heroes.ShadowFiend
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using Modifiers;

    using static Data.AbilityNames;

    internal class RequiemOfSouls : AOE, IModifier
    {
        #region Fields

        private readonly float projectileSpeed;

        private readonly float width;

        #endregion

        #region Constructors and Destructors

        public RequiemOfSouls(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            DisablePathfinder = true;

            DisableAbilities.AddRange(DisableAbilityNames);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(Armlet);

            Modifier.AllyCounterAbilities.Add(Lotus);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(VsDamage);

            projectileSpeed = ability.AbilitySpecialData.First(x => x.Name == "requiem_line_speed").Value;
            width = ability.AbilitySpecialData.First(x => x.Name == "requiem_line_width_end").Value;
        }

        #endregion

        #region Public Properties

        public EvadableModifier Modifier { get; }

        #endregion

        #region Public Methods and Operators

        public override void Check()
        {
            var time = Game.RawGameTime;

            if (IsInPhase && StartCast + CastPoint <= time)
            {
                StartCast = time;
                StartPosition = AbilityOwner.NetworkPosition;
                EndCast = StartCast + CastPoint + GetRadius() / projectileSpeed;
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

        #endregion

        #region Methods

        protected override float GetRadius()
        {
            return base.GetRadius() + width;
        }

        #endregion
    }
}