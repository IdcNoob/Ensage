namespace ItemManager.Core.Abilities
{
    using Attributes;

    using Base;

    using Ensage;
    using Ensage.Common.Objects;
    using Ensage.SDK.Extensions;

    using Utils;

    [Ability(AbilityId.item_armlet)]
    internal class Armlet : OffensiveAbility
    {
        public Armlet(Ability ability, Manager manager)
            : base(ability, manager)
        {
        }

        public override bool CanBeCasted(Unit target)
        {
            if (Manager.MyHero.HasModifier(ModifierUtils.ArmletStrength) || target == null || !target.IsValid
                || !Menu.IsEnabled(target.StoredName()))
            {
                return false;
            }

            if (!target.IsAlive || !target.IsVisible || target.Distance2D(Manager.MyHero.Position) > GetCastRange()
                || target.IsInvulnerable())
            {
                return false;
            }

            return true;
        }

        public override void Use(Unit target = null, bool queue = false)
        {
            SetSleep(800);
            Ability.ToggleAbility();
        }
    }
}