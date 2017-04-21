namespace ItemManager.Core.Abilities
{
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;

    [Ability(AbilityId.item_iron_talon)]
    internal class IronTalon : UsableAbility
    {
        public IronTalon(Ability ability, Manager manager)
            : base(ability, manager)
        {
            Damage = ability.AbilitySpecialData.First(x => x.Name == "creep_damage_pct").Value / 100;
        }

        public float Damage { get; }
    }
}