namespace ItemManager.Core.Abilities
{
    using Attributes;

    using Base;

    using Ensage;
    using Ensage.Common.Objects;
    using Ensage.SDK.Extensions;

    using Menus.Modules.OffensiveAbilities.AbilitySettings;

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

        private DiffusalBladeSettings diffusalMenu => (DiffusalBladeSettings)Menu;

        public override bool CanBeCasted(Unit target)
        {
            if (diffusalBlade.CurrentCharges <= 0)
            {
                return false;
            }

            if (target == null || !target.IsValid || target.HasModifier(ModifierUtils.DiffusalDebuff)
                || !Menu.IsEnabled(target.StoredName()))
            {
                return false;
            }

            if (!target.IsAlive || !target.IsVisible || target.Distance2D(Manager.MyHero.Position) > GetCastRange()
                || target.IsReflectingAbilities())
            {
                return false;
            }

            if (diffusalMenu.ImmunityOnly)
            {
                return target.HasAnyModifiers(ModifierUtils.DiffusalRemovableModifiers.ToArray());
            }

            if (target.IsMagicImmune() || target.IsInvulnerable())
            {
                return false;
            }

            if (!Menu.BreakLinkens && target.IsLinkensProtected())
            {
                return false;
            }

            if (!Menu.HexStack && target.IsReallyHexed())
            {
                return false;
            }

            if (!Menu.SilenceStack && target.IsSilenced() && !target.IsReallyHexed())
            {
                return false;
            }

            if (!Menu.RootStack && target.IsRooted())
            {
                return false;
            }

            if (!Menu.StunStack && target.IsStunned())
            {
                return false;
            }

            if (!Menu.DisarmStack && target.IsDisarmed() && !target.IsReallyHexed())
            {
                return false;
            }

            return true;
        }
    }
}