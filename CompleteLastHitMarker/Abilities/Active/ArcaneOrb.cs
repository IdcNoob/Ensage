namespace CompleteLastHitMarker.Abilities.Active
{
    using System;
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;
    using Ensage.Common.Extensions.Damage;

    [Ability(AbilityId.obsidian_destroyer_arcane_orb)]
    internal class ArcaneOrb : DefaultActive
    {
        public ArcaneOrb(Ability ability)
            : base(ability)
        {
            for (var i = 0u; i < Damage.Length; i++)
            {
                Damage[i] = Ability.AbilitySpecialData.First(x => x.Name == "mana_pool_damage_pct").GetValue(i) / 100;
            }
        }

        public override float CalculateDamage(Hero source, Unit target)
        {
            return (float)Math.Round(
                target.SpellDamageTaken(Damage[Level - 1] * Math.Max(source.Mana - ManaCost, 0), DamageType, source, Name));
        }
    }
}