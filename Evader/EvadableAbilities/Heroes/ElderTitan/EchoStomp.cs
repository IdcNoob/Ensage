namespace Evader.EvadableAbilities.Heroes.ElderTitan
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    internal class EchoStomp : AOE, IModifier
    {
        #region Fields

        private readonly float channelTime;

        private readonly float[] modifierDuration = new float[4];

        private Modifier abilityModifier;

        #endregion

        #region Constructors and Destructors

        public EchoStomp(Ability ability)
            : base(ability)
        {
            //todo add astral spirit
            channelTime = ability.GetChannelTime(0);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Remove("abaddon_aphotic_shield");

            ModifierAllyCounter.AddRange(AllyShields);
            ModifierAllyCounter.AddRange(Invul);
            ModifierAllyCounter.AddRange(VsMagic);

            for (var i = 0u; i < 4; i++)
            {
                modifierDuration[i] = Ability.AbilitySpecialData.First(x => x.Name == "sleep_duration").GetValue(i);
            }
        }

        #endregion

        #region Public Properties

        public uint ModifierHandle { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void AddModifer(Modifier modifier, Hero hero)
        {
            if (hero.Team != HeroTeam)
            {
                return;
            }

            abilityModifier = modifier;
            ModifierHandle = modifier.Handle;
        }

        public bool CanBeCountered()
        {
            return abilityModifier != null && abilityModifier.IsValid;
        }

        public override void Check()
        {
            if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                StartPosition = AbilityOwner.NetworkPosition;
                EndCast = StartCast + CastPoint;
                Obstacle = Pathfinder.AddObstacle(StartPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast && !Ability.IsChanneling)
            {
                End();
            }
        }

        public float GetModiferRemainingTime()
        {
            return modifierDuration[Ability.Level - 1] - abilityModifier.ElapsedTime;
        }

        public Hero GetModifierHero(ParallelQuery<Hero> allies)
        {
            return
                allies.Where(x => x.HasModifier(abilityModifier.Name)).OrderByDescending(x => x.Health).FirstOrDefault();
        }

        public override float GetRemainingDisableTime()
        {
            return GetRemainingTime() - 0.05f;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return EndCast + channelTime - Game.RawGameTime;
        }

        public void RemoveModifier(Modifier modifier)
        {
            abilityModifier = null;
        }

        #endregion
    }
}