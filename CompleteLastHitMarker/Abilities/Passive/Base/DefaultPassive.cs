namespace CompleteLastHitMarker.Abilities.Passive.Base
{
    using System.Collections.Generic;

    using Ensage;

    using Interfaces;

    using Units.Base;

    using Utils;

    internal abstract class DefaultPassive : DefaultAbility, IPassiveAbility
    {
        protected DefaultPassive(Ability ability)
            : base(ability)
        {
        }

        protected bool DealsDamageToAllies { get; set; }

        public abstract float GetBonusDamage(Hero hero, KillableUnit unit, IEnumerable<IPassiveAbility> abilities);

        protected virtual bool CanDoDamage(Hero hero, KillableUnit unit, IEnumerable<IPassiveAbility> abilities)
        {
            if (!DealsDamageToAllies && unit.Team == hero.Team)
            {
                return false;
            }

            if (!DealsDamageToTowers && unit.UnitType == UnitType.Tower)
            {
                return false;
            }

            return true;
        }
    }
}