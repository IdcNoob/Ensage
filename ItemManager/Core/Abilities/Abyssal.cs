namespace ItemManager.Core.Abilities
{
    using Attributes;

    using Base;

    using Ensage;

    [Ability(AbilityId.item_abyssal_blade)]
    internal class Abyssal : OffensiveAbility
    {
        public Abyssal(Ability ability, Manager manager)
            : base(ability, manager)
        {
        }

        public override float GetCastRange()
        {
            return base.GetCastRange() + 100;
        }
    }
}