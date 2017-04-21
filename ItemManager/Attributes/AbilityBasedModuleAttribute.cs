namespace ItemManager.Attributes
{
    using System;

    using Ensage;

    using Attribute = System.Attribute;

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    internal class AbilityBasedModuleAttribute : Attribute
    {
        public AbilityBasedModuleAttribute(AbilityId abilityId)
        {
            AbilityId = abilityId;
        }

        public AbilityId AbilityId { get; }
    }
}