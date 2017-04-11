namespace AbilityLastHitMarker.Classes
{
    using System.Collections.Generic;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    internal class Tower : IKillable
    {
        public Tower(Unit tower)
        {
            Unit = tower;
            Handle = tower.Handle;
            HpBarSize = HUDInfo.GetHPBarSizeX(tower);
        }

        public bool DamageCalculated { get; set; }

        public Dictionary<uint, float> GetSavedDamage { get; } = new Dictionary<uint, float>();

        public uint Handle { get; }

        public uint Health => Unit.Health;

        public float HeroDamage { get; set; }

        public float HpBarSize { get; }

        public bool IsValid => Unit != null && Unit.IsValid && Unit.IsAlive;

        public Unit Unit { get; }

        public float Distance(Hero hero)
        {
            return Unit.Distance2D(hero);
        }

        public void SaveDamage(uint handle, float damage)
        {
            GetSavedDamage[handle] = damage;
        }
    }
}