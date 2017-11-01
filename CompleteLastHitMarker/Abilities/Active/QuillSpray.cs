namespace CompleteLastHitMarker.Abilities.Active
{
    using System;
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;
    using Ensage.Common.Extensions.Damage;

    [Ability(AbilityId.bristleback_quill_spray)]
    internal class QuillSpray : DefaultActive
    {
        private const string QuillSprayModifier = "modifier_bristleback_quill_spray";

        private readonly float[] stackDamage = new float[4];

        public QuillSpray(Ability ability)
            : base(ability)
        {
            for (var i = 0u; i < Damage.Length; i++)
            {
                Damage[i] = Ability.AbilitySpecialData.First(x => x.Name == "quill_base_damage").GetValue(i);
                stackDamage[i] = Ability.AbilitySpecialData.First(x => x.Name == "quill_stack_damage").GetValue(i);
            }
        }

        public override float CalculateDamage(Hero source, Unit target)
        {
            return (float)Math.Round(
                target.DamageTaken(
                    (target.Modifiers.FirstOrDefault(x => x.Name == QuillSprayModifier)?.StackCount * stackDamage[Level - 1] ?? 0)
                    + Damage[Level - 1],
                    DamageType,
                    source));
        }
    }
}