namespace Timbersaw.Abilities
{
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    internal abstract class TimberAbility
    {
        #region Constructors and Destructors

        protected TimberAbility(Ability ability)
        {
            Ability = ability;
            CastPoint = (float)ability.FindCastPoint();
            Name = ability.Name;
        }

        #endregion

        #region Public Properties

        public Ability Ability { get; }

        public float CastPoint { get; }

        public float GetSleepTime => CastPoint * 1000;

        #endregion

        #region Properties

        protected string Name { get; }

        #endregion

        #region Public Methods and Operators

        public virtual bool CanBeCasted()
        {
            return Utils.SleepCheck("Timber." + Name) && Ability.CanBeCasted();
        }

        public float GetCastRange()
        {
            return Ability.GetCastRange() + 50;
        }

        #endregion
    }
}