namespace CompleteLastHitMarker.Abilities.Interfaces
{
    using System.Collections.Generic;

    using Ensage;

    using Units.Base;

    internal interface IPassiveAbility
    {
        AbilityId AbilityId { get; }

        DamageType DamageType { get; }

        float GetBonusDamage(Hero hero, KillableUnit unit, IEnumerable<IPassiveAbility> abilities);

        bool IsValid(Hero hero, KillableUnit unit);
    }
}