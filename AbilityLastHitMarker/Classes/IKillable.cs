namespace AbilityLastHitMarker.Classes
{
    using System.Collections.Generic;

    using Ensage;

    internal interface IKillable
    {
        Dictionary<uint, float> GetSavedDamage { get; }

        uint Handle { get; }

        uint Health { get; }

        float HpBarSize { get; }

        bool IsValid { get; }

        Unit Unit { get; }

        float HeroDamage { get; set; }

        bool DamageCalculated { get; set; }

        float Distance(Hero hero);
    }
}