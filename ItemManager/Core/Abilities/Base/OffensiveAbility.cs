namespace ItemManager.Core.Abilities.Base
{
    using Attributes;

    using Ensage;

    using Interfaces;

    [Ability(AbilityId.item_orchid)]
    [Ability(AbilityId.item_sheepstick)]
    [Ability(AbilityId.item_bloodthorn)]
    [Ability(AbilityId.item_medallion_of_courage)]
    [Ability(AbilityId.item_solar_crest)]
    [Ability(AbilityId.item_cyclone)]
    [Ability(AbilityId.item_heavens_halberd)]
    [Ability(AbilityId.item_rod_of_atos)]
    [Ability(AbilityId.item_ethereal_blade)]
    [Ability(AbilityId.item_abyssal_blade)]
    internal class OffensiveAbility : UsableAbility, IOffensiveAbility
    {
        public OffensiveAbility(Ability ability, Manager manager)
            : base(ability, manager)
        {
            IsOffensiveAbility = true;
        }

        public virtual bool CanBeCasted(Unit target)
        {
            return base.CanBeCasted();
        }

        public override void Use(Unit target = null, bool queue = false)
        {
            if (target == null)
            {
                return;
            }

            SetSleep(200);
            Ability.UseAbility(target, queue);
        }
    }
}