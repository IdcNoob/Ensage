namespace ItemManager.Core.Abilities
{
    using Attributes;

    using Base;

    using Ensage;

    [Ability(AbilityId.treant_living_armor)]
    internal class LivingArmor : UsableAbility
    {
        public LivingArmor(Ability ability, Manager manager)
            : base(ability, manager)
        {
        }
    }
}