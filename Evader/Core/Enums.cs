namespace Evader.Core
{
    using System;

    [Flags]
    public enum AbilityFlags
    {
        None = 0,

        TargetEnemy = 1,

        CanBeCastedOnAlly = 2,

        BasicDispel = 4,

        StrongDispel = 8 | BasicDispel
    }

    public enum AbilityType
    {
        Counter,

        Blink,

        Disable
    }

    public enum Priority
    {
        Walk,

        Blink,

        Counter,

        Disable
    }
}