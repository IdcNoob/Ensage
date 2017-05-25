namespace ItemManager.Core.Modules.Interfaces
{
    using Ensage;

    internal interface IAbilityBasedModule
    {
        AbilityId AbilityId { get; }

        void Dispose();

        void Refresh();
    }
}