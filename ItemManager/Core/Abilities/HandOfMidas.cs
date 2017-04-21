namespace ItemManager.Core.Abilities
{
    using Attributes;

    using Base;

    using Ensage;

    [Ability(AbilityId.item_hand_of_midas)]
    internal class HandOfMidas : UsableAbility
    {
        public HandOfMidas(Ability ability, Manager manager)
            : base(ability, manager)
        {
        }
    }
}