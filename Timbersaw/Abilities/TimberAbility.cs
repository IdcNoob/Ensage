namespace Timbersaw.Abilities
{
    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    internal abstract class TimberAbility
    {
        #region Fields

        protected Sleeper Sleeper;

        #endregion

        #region Constructors and Destructors

        protected TimberAbility(Ability ability)
        {
            Ability = ability;
            CastPoint = (float)ability.FindCastPoint();
            Name = ability.Name;
            Sleeper = new Sleeper();
        }

        #endregion

        #region Public Properties

        public Ability Ability { get; }

        public float CastPoint { get; }

        public float GetSleepTime => CastPoint * 1000;

        public bool IsSleeping => Sleeper.Sleeping;

        #endregion

        #region Properties

        protected string Name { get; }

        #endregion

        #region Public Methods and Operators

        public virtual bool CanBeCasted()
        {
            return !IsSleeping && Ability.CanBeCasted();
        }

        public float GetCastRange()
        {
            return Ability.GetCastRange() + 50;
        }

        #endregion
    }
}