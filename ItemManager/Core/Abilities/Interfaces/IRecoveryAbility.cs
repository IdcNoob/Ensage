namespace ItemManager.Core.Abilities.Interfaces
{
    using Ensage;

    using Utils;

    internal interface IRecoveryAbility
    {
        uint Handle { get; }

        float HealthRestore { get; }

        AbilityId Id { get; }

        bool IsSleeping { get; }

        ItemUtils.Stats ItemRestoredStats { get; }

        float ManaRestore { get; }

        string Name { get; }

        Attribute PowerTreadsAttribute { get; }

        bool CanBeCasted();

        void Use(bool queue = false);
    }
}