namespace ItemManager.Core.Modules.Interfaces
{
    using System.Collections.Generic;

    using Ensage;

    internal interface IAbilityBasedModule
    {
        List<AbilityId> AbilityIds { get; }

        void Dispose();

        void Refresh();
    }
}