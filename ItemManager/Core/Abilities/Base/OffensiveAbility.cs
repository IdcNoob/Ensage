namespace ItemManager.Core.Abilities.Base
{
    using Attributes;

    using Ensage;
    using Ensage.Common.Objects;
    using Ensage.SDK.Extensions;

    using Interfaces;

    using Menus.Modules.OffensiveAbilities.AbilitySettings;

    [Ability(AbilityId.item_orchid)]
    [Ability(AbilityId.item_sheepstick)]
    [Ability(AbilityId.item_bloodthorn)]
    [Ability(AbilityId.item_medallion_of_courage)]
    [Ability(AbilityId.item_solar_crest)]
    [Ability(AbilityId.item_cyclone)]
    [Ability(AbilityId.item_heavens_halberd)]
    [Ability(AbilityId.item_rod_of_atos)]
    [Ability(AbilityId.item_ethereal_blade)]
    [Ability(AbilityId.item_diffusal_blade)]
    [Ability(AbilityId.item_nullifier)]
    internal class OffensiveAbility : UsableAbility, IOffensiveAbility
    {
        public OffensiveAbility(Ability ability, Manager manager)
            : base(ability, manager)
        {
        }

        public OffensiveAbilitySettings Menu { get; set; }

        public virtual bool CanBeCasted(Unit target)
        {
            if (target == null || !target.IsValid || !Menu.IsEnabled(target.StoredName()))
            {
                return false;
            }

            if (!target.IsAlive || !target.IsVisible || target.IsMagicImmune()
                || target.Distance2D(Manager.MyHero.Position) > GetCastRange() || target.IsInvulnerable() || target.IsReflectingAbilities())
            {
                return false;
            }

            if (!Menu.BreakLinkens && target.IsBlockingAbilities())
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
            Ability.UseAbility(target, queue);
        }
    }
}