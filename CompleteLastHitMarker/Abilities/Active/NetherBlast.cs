namespace CompleteLastHitMarker.Abilities.Active
{
    using System;
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;
    using Ensage.Common.Extensions.Damage;

    [Ability(AbilityId.pugna_nether_blast)]
    internal class NetherBlast : DefaultActive
    {
        private readonly Ability talent;

        private readonly float talentBonusDamage;

        private readonly float towerDamageReduction;

        public NetherBlast(Ability ability)
            : base(ability)
        {
            DealsDamageToTowers = true;

            for (var i = 0u; i < Damage.Length; i++)
            {
                Damage[i] = Ability.AbilitySpecialData.First(x => x.Name == "blast_damage").GetValue(i);
            }

            towerDamageReduction = Ability.AbilitySpecialData.First(x => x.Name == "structure_damage_mod").Value;
            talent = ((Unit)ability.Owner).Spellbook.Spells.FirstOrDefault(x => x.Id == AbilityId.special_bonus_unique_pugna_2);
            talentBonusDamage = talent?.AbilitySpecialData.First(x => x.Name == "value").Value ?? 0;
        }

        public override float CalculateDamage(Hero source, Unit target)
        {
            var damage = target.SpellDamageTaken(Damage[Level - 1], DamageType, source, Name);

            if (talent?.Level >= 1)
            {
                damage += talentBonusDamage;
            }

            if (target is Tower)
            {
                damage *= towerDamageReduction;
            }

            return (float)Math.Round(damage);
        }
    }
}