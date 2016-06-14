namespace AbilityLastHitMarker.Classes
{
    using System.Collections.Generic;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    internal class Creep : IKillable
    {
        #region Constructors and Destructors

        public Creep(Unit unit)
        {
            Unit = unit;
            Handle = unit.Handle;
            HpBarSize = HUDInfo.GetHPBarSizeX(unit);
        }

        #endregion

        #region Public Properties

        public bool DamageCalculated { get; set; }

        public Dictionary<uint, float> GetSavedDamage { get; } = new Dictionary<uint, float>();

        public uint Handle { get; }

        public uint Health => Unit.Health;

        public float HeroDamage { get; set; }

        public float HpBarSize { get; }

        public bool IsValid => Unit != null && Unit.IsValid && Unit.IsSpawned && Unit.IsAlive;

        public Unit Unit { get; }

        #endregion

        #region Public Methods and Operators

        public float Distance(Hero hero)
        {
            return Unit.Distance2D(hero);
        }

        public void SaveDamage(uint handle, float damage)
        {
            GetSavedDamage[handle] = damage;
        }

        #endregion
    }
}