namespace CompleteLastHitMarker.Abilities.Interfaces
{
    using Ensage;

    using Units.Base;

    internal interface IActiveAbility
    {
        AbilityId AbilityId { get; }

        DamageType DamageType { get; }

        bool DealsAutoAttackDamage { get; }

        string Name { get; }

        DotaTexture Texture { get; }

        float CalculateDamage(Hero source, Unit target);

        bool CanBeCasted();

        bool IsValid(Hero hero, KillableUnit unit);
    }
}