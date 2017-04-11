namespace Timbersaw.Abilities
{
    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    internal abstract class TimberAbility
    {
        protected Sleeper Sleeper;

        protected TimberAbility(Ability ability)
        {
            Ability = ability;
            CastPoint = (float)ability.FindCastPoint();
            Name = ability.Name;
            Sleeper = new Sleeper();
        }

        public Ability Ability { get; }

        public float CastPoint { get; }

        public float GetSleepTime => CastPoint * 1000;

        public bool IsSleeping => Sleeper.Sleeping;

        protected string Name { get; }

        public virtual bool CanBeCasted()
        {
            return !IsSleeping && Ability.CanBeCasted();
        }

        public float GetCastRange()
        {
            return Ability.GetCastRange() + 50;
        }
    }
}