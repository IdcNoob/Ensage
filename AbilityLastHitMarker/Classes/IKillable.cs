namespace AbilityLastHitMarker.Classes
{
    using System.Collections.Generic;

    using Ensage;

    internal interface IKillable
    {
        #region Public Properties

        Dictionary<uint, float> GetSavedDamage { get; }

        uint Handle { get; }

        uint Health { get; }

        float HpBarSize { get; }

        bool IsValid { get; }

        Unit Unit { get; }

        #endregion

        #region Public Methods and Operators

        float Distance(Hero hero);

        float HeroDamage { get; set; }

        bool DamageCalculated { get; set; }

        #endregion
    }
}