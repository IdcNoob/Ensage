namespace ItemManager.Core.Abilities
{
    using Attributes;

    using Base;

    using Ensage;
    using Ensage.Common.Objects;
    using Ensage.SDK.Extensions;

    using Menus.Modules.OffensiveAbilities.AbilitySettings;

    using Utils;

    [Ability(AbilityId.item_satanic)]
    internal class Satanic : OffensiveAbility
    {
        public Satanic(Ability ability, Manager manager)
            : base(ability, manager)
        {
        }

        private SatanicSettings SatanicMenu => (SatanicSettings)Menu;

        public override bool CanBeCasted(Unit target)
        {
            if (target == null || !target.IsValid || !Menu.IsEnabled(target.StoredName()))
            {
                return false;
            }

            if (!Manager.MyHero.CanAttack() || Manager.MyHero.HealthPercentage > SatanicMenu.HealthThreshold)
            {
                return false;
            }

            if (!target.IsAlive || !target.IsVisible || target.Distance2D(Manager.MyHero.Position) > GetCastRange()
                || target.IsInvulnerable())
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

        public override void Use(Unit target = null, bool queue = false)
        {
            if (target == null)
            {
                return;
            }

            SetSleep(CastPoint + 200);
            Ability.UseAbility(queue);
        }
    }
}