namespace ItemManager.Core.Modules.HpMpAbuse.Items
{
    using Ensage;
    using Ensage.Common.Extensions;

    using Utils;

    internal class SoulRing : UsableItem
    {
        public SoulRing(string name)
            : base(name)
        {
        }

        public override bool CanBeCasted()
        {
            return base.CanBeCasted() && (float)Hero.Health / Hero.MaximumHealth * 100 >= Menu.SoulRingMenu.HpThreshold
                   && Hero.Mana / Hero.MaximumMana * 100 <= Menu.SoulRingMenu.MpThreshold
                   && (Menu.RecoveryMenu.IsActive && Menu.RecoveryMenu.IsAbilityEnabled(Name)
                       && !Hero.HasModifier(ModifierUtils.FountainRegeneration)
                       || !Menu.RecoveryMenu.IsActive && Menu.SoulRingMenu.IsActive);
        }

        public override ItemUtils.Stats GetDropItemStats()
        {
            return Hero.Mana < Hero.MaximumMana ? ItemUtils.Stats.Mana : ItemUtils.Stats.None;
        }

        public override Attribute GetPowerTreadsAttribute()
        {
            return Attribute.Strength;
        }
    }
}