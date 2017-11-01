namespace CompleteLastHitMarker.Abilities.Active
{
    using System;
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;
    using Ensage.Common.AbilityInfo;
    using Ensage.Common.Extensions;

    [Ability(AbilityId.tiny_toss)]
    internal class Toss : DefaultActive
    {
        private readonly Ability grow;

        private readonly Unit owner;

        private readonly float[] tossBonusDamage = new float[3];

        private readonly float[] tossBonusDamageScepter = new float[3];

        private readonly float towerDamageReduction;

        public Toss(Ability ability)
            : base(ability)
        {
            DealsDamageToTowers = true;
            towerDamageReduction = 0.33f;
            owner = (Unit)ability.Owner;
            grow = owner.Spellbook.Spells.FirstOrDefault(x => x.Id == AbilityId.tiny_grow);

            if (grow == null)
            {
                return;
            }

            for (var i = 0u; i < tossBonusDamage.Length; i++)
            {
                tossBonusDamage[i] = (grow.AbilitySpecialData.First(x => x.Name == "grow_bonus_damage_pct").GetValue(i) / 100) + 1;
                tossBonusDamageScepter[i] =
                    (grow.AbilitySpecialData.First(x => x.Name == "grow_bonus_damage_pct_scepter").GetValue(i) / 100) + 1;
            }
        }

        public override float CalculateDamage(Hero source, Unit target)
        {
            var damage = AbilityDamage.CalculateDamage(Ability, source, target);

            if (target is Tower)
            {
                damage *= towerDamageReduction;
            }
            else if (grow?.Level >= 1)
            {
                damage *= owner.AghanimState() ? tossBonusDamageScepter[grow.Level - 1] : tossBonusDamage[grow.Level - 1];
            }

            return (float)Math.Round(damage);
        }
    }
}