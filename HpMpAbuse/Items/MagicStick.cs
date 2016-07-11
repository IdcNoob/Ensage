namespace HpMpAbuse.Items
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;

    internal class MagicStick : UsableItem
    {
        #region Constants

        private const string WandName = "item_magic_wand";

        #endregion

        #region Fields

        private readonly float restorePerCharge;

        #endregion

        #region Constructors and Destructors

        public MagicStick(string name)
            : base(name)
        {
            restorePerCharge =
                Ability.GetAbilityDataByName(Name).AbilitySpecialData.First(x => x.Name == "restore_per_charge").Value;
        }

        #endregion

        #region Properties

        protected override float HealthRestore => restorePerCharge * Item.CurrentCharges;

        protected override float ManaRestore => restorePerCharge * Item.CurrentCharges;

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted()
        {
            return base.CanBeCasted() && Item.CurrentCharges > 0
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

        #endregion
    }
}