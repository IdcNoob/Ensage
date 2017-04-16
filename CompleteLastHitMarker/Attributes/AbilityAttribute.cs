namespace CompleteLastHitMarker.Attributes
{
    using System;

    using Ensage;

    using Attribute = System.Attribute;

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    internal class AbilityAttribute : Attribute
    {
        public AbilityAttribute(AbilityId abilityId)
        {
            AbilityId = abilityId;
        }

        public AbilityId AbilityId { get; }
    }
}