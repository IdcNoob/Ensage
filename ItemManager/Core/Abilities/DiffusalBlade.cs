namespace ItemManager.Core.Abilities
{
    using Attributes;

    using Base;

    using Ensage;
    using Ensage.SDK.Extensions;

    using Utils;

    [Ability(AbilityId.item_diffusal_blade)]
    [Ability(AbilityId.item_diffusal_blade_2)]
    internal class DiffusalBlade : OffensiveAbility
    {
        private readonly Item diffusalBlade;

        public DiffusalBlade(Ability ability, Manager manager)
            : base(ability, manager)
        {
            diffusalBlade = ability as Item;
        }

        public override bool CanBeCasted(Unit target)
        {
            return base.CanBeCasted(target) && diffusalBlade.CurrentCharges > 0
                   && !target.HasModifier(ModifierUtils.DiffusalDebuff);
        }
    }
}