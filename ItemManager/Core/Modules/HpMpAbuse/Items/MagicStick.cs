namespace ItemManager.Core.Modules.HpMpAbuse.Items
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;

    internal class MagicStick : UsableItem
    {
        private const string WandName = "item_magic_wand";

        private readonly float restorePerCharge;

        public MagicStick(string name)
            : base(name)
        {
            restorePerCharge = Ability.GetAbilityDataByName(Name)
                .AbilitySpecialData.First(x => x.Name == "restore_per_charge")
                .Value;
        }

        protected override float HealthRestore => restorePerCharge * Item.CurrentCharges;

        protected override float ManaRestore => restorePerCharge * Item.CurrentCharges;

        public override bool CanBeCasted()
        {
            return base.CanBeCasted() && Menu.RecoveryMenu.IsAbilityEnabled(Name) && Item.CurrentCharges > 0
                   && (Hero.Health < Hero.MaximumHealth || Hero.Mana < Hero.MaximumMana);
        }

        public override void FindItem()
        {
            Item = Hero.FindItem(Name) ?? Hero.FindItem(WandName);
        }

        public override Attribute GetPowerTreadsAttribute()
        {
            return Attribute.Agility;
        }
    }
}