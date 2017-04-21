namespace ItemManager.Core.Abilities
{
    using Attributes;

    using Base;

    using Ensage;

    [Ability(AbilityId.item_phase_boots)]
    internal class PhaseBoots : UsableAbility
    {
        public PhaseBoots(Ability ability, Manager manager)
            : base(ability, manager)
        {
        }
    }
}