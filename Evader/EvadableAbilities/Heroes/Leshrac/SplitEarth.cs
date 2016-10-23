namespace Evader.EvadableAbilities.Heroes.Leshrac
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Common;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    internal class SplitEarth : LinearAOE, IModifierObstacle, IModifier
    {
        #region Fields

        private readonly float modifierDuration;

        private Modifier abilityModifier;

        private bool fowCast;

        private bool modifierAdded;

        #endregion

        #region Constructors and Destructors

        public SplitEarth(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Remove("abaddon_aphotic_shield");

            ModifierAllyCounter.AddRange(AllyShields);
            ModifierAllyCounter.AddRange(Invul);
            ModifierAllyCounter.AddRange(VsMagic);

            modifierDuration = Ability.AbilitySpecialData.First(x => x.Name == "duration").Value;
            AdditionalDelay = Ability.AbilitySpecialData.First(x => x.Name == "delay").Value;
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

        public void AddModifierObstacle(Modifier mod, Unit unit)
        {
            var position = unit.Position;
            modifierAdded = true;

            AbilityDrawer.Dispose(AbilityDrawer.Type.Rectangle);
            AbilityDrawer.DrawCircle(position, GetRadius());

            if (Obstacle == null)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + AdditionalDelay;
                fowCast = true;
            }

            Obstacle = Pathfinder.AddObstacle(position, GetRadius(), Obstacle);
        }

        public bool CanBeCountered()
        {
            return abilityModifier != null && abilityModifier.IsValid;
        }

        public override bool CanBeStopped()
        {
            return !modifierAdded && base.CanBeStopped();
        }

        public override void Draw()
        {
            if (Obstacle == null)
            {
                return;
            }

            AbilityDrawer.DrawTime(GetRemainingTime(), StartPosition);

            if (!modifierAdded)
            {
                AbilityDrawer.DrawDoubleArcRectangle(StartPosition, EndPosition, GetRadius());
            }
        }

        public override void End()
        {
            if (Obstacle == null)
            {
                return;
            }

            base.End();
            modifierAdded = false;
            fowCast = false;
        }

        public float GetModiferRemainingTime()
        {
            return modifierDuration - abilityModifier.ElapsedTime;
        }

        public Hero GetModifierHero(ParallelQuery<Hero> allies)
        {
            return
                allies.Where(x => x.HasModifier(abilityModifier.Name)).OrderByDescending(x => x.Health).FirstOrDefault();
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return StartCast + (fowCast ? 0 : CastPoint) + AdditionalDelay - Game.RawGameTime;
        }

        public void RemoveModifier(Modifier modifier)
        {
            abilityModifier = null;
        }

        #endregion

        #region Methods

        protected override float GetRadius()
        {
            return Ability.GetRadius() + 60;
        }

        #endregion
    }
}