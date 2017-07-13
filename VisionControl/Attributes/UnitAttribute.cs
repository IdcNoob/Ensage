namespace VisionControl.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    internal class UnitAttribute : Attribute
    {
        public UnitAttribute(string abilityName, params string[] unitNames)
        {
            AbilityName = abilityName;
            UnitNames = unitNames;
        }

        public string AbilityName { get; }

        public string[] UnitNames { get; }
    }
}