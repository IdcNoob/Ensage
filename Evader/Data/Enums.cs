namespace Evader.Data
{
    using System;

    [Flags]
    public enum AbilityCastTarget
    {
        Self,

        Ally,

        Enemy,
    }

    public enum AbilityType
    {
        Counter,

        Blink,

        Disable
    }

    public enum EvadePriority
    {
        Walk,

        Blink,

        Counter,

        Disable
    }
}