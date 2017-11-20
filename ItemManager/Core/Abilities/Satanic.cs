namespace ItemManager.Core.Abilities
{
    using Attributes;

    using Base;

    using Ensage;
    using Ensage.Common.Objects;
    using Ensage.SDK.Extensions;

    using Menus.Modules.OffensiveAbilities.AbilitySettings;

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

            var isHexed = target.IsHexed();

            if (!Menu.HexStack && isHexed)
            {
                return false;
            }

            if (!Menu.SilenceStack && target.IsSilenced() && !isHexed)
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

            if (!Menu.DisarmStack && target.IsDisarmed() && !isHexed)
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