namespace ItemManager.Core.Abilities.Base
{
    using Attributes;

    using Ensage;
    using Ensage.SDK.Extensions;

    using Interfaces;

    using Menus.Modules.DefensiveAbilities.AbilitySettings;

    using Utils;

    [Ability(AbilityId.item_blade_mail)]
    [Ability(AbilityId.item_lotus_orb)]
    [Ability(AbilityId.item_black_king_bar)]
    [Ability(AbilityId.item_shivas_guard)]
    [Ability(AbilityId.item_mjollnir)]
    internal class DefensiveAbility : UsableAbility, IDefensiveAbility
    {
        public DefensiveAbility(Ability ability, Manager manager)
            : base(ability, manager)
        {
        }

        public DefensiveAbilitySettings Menu { get; set; }

        public override bool CanBeCasted()
        {
            if (!base.CanBeCasted())
            {
                return false;
            }

            if (!Menu.MagicImmunityStack && Manager.MyHero.Hero.IsMagicImmune())
            {
                return false;
            }

            if (!Menu.BladeMailStack && Manager.MyHero.HasModifier(ModifierUtils.BladeMailReflect))
            {
                return false;
            }

            if (!Menu.LotusOrbStack && Manager.MyHero.Hero.IsReflectingAbilities())
            {
                return false;
            }

            return true;
        }
    }
}