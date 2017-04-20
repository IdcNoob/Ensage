namespace CompleteLastHitMarker.Abilities.Active
{
    using System;
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;
    using Ensage.Common.AbilityInfo;

    [Ability(AbilityId.techies_land_mines)]
    internal class ProximityMines : DefaultActive
    {
        private readonly float towerDamageReduction;

        public ProximityMines(Ability ability)
            : base(ability)
        {
            DealsDamageToTowers = true;
            towerDamageReduction = Ability.AbilitySpecialData.First(x => x.Name == "building_damage_pct").Value / 100;
        }

        public override float CalculateDamage(Hero source, Unit target)
        {
            var damage = AbilityDamage.CalculateDamage(Ability, source, target);

            if (target is Tower)
            {
                damage *= towerDamageReduction;
            }

            return (float)Math.Round(damage);
        }
    }
}