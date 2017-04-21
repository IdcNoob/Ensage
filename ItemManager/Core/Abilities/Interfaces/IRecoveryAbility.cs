namespace ItemManager.Core.Abilities.Interfaces
{
    using Ensage;

    using Menus.Modules.Recovery;

    using Utils;

    internal interface IRecoveryAbility
    {
        uint Handle { get; }

        float HealthRestore { get; }

        AbilityId Id { get; }

        bool IsSleeping { get; }

        float ManaRestore { get; }

        string Name { get; }

        Attribute PowerTreadsAttribute { get; }

        RestoredStats RestoredStats { get; }

        bool CanBeCasted();

        bool ShouldBeUsed(MyHero hero, RecoveryMenu menu, float missingHealth, float missingMana);

        void Use(Unit target = null, bool queue = false);
    }
}