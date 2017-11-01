namespace CompleteLastHitMarker.Abilities.Active
{
    using System;
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;
    using Ensage.Common.Extensions.Damage;

    [Ability(AbilityId.tiny_avalanche)]
    internal class Avalanche : DefaultActive
    {
        private readonly Ability talent;

        private readonly float talentBonusDamage;

        public Avalanche(Ability ability)
            : base(ability)
        {
            for (var i = 0u; i < Damage.Length; i++)
            {
                Damage[i] = Ability.AbilitySpecialData.First(x => x.Name == "avalanche_damage").GetValue(i);
            }

            talent = ((Unit)ability.Owner).Spellbook.Spells.FirstOrDefault(x => x.Id == AbilityId.special_bonus_unique_tiny);
            talentBonusDamage = talent?.AbilitySpecialData.First(x => x.Name == "value").Value ?? 0;
        }

        public override float CalculateDamage(Hero source, Unit target)
        {
            var damage = target.SpellDamageTaken(Damage[Level - 1], DamageType, source, Name);

            if (talent?.Level >= 1)
            {
                damage += talentBonusDamage;
            }

            return (float)Math.Round(damage);
        }
    }
}