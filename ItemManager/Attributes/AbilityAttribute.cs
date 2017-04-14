namespace ItemManager.Attributes
{
    using System;

    using Ensage;

    using Attribute = System.Attribute;

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    internal class AbilityAttribute : Attribute
    {
        public AbilityAttribute(AbilityId abilityId)
        {
            AbilityId = abilityId;
        }

        public AbilityId AbilityId { get; }
    }
}