namespace HpMpAbuse.Items
{
    using System.Linq;

    using Ensage;

    internal sealed class ArcaneBoots : UsableItem
    {
        #region Constructors and Destructors

        public ArcaneBoots(string name)
            : base(name)
        {
            ManaRestore =
                Ability.GetAbilityDataByName(Name).AbilitySpecialData.First(x => x.Name == "replenish_amount").Value;
        }

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted()
        {
            return base.CanBeCasted() && Menu.Recovery.IsEnabled(Name) && Hero.Mana < Hero.MaximumMana;
        }

        public override Attribute GetPowerTreadsAttribute()
        {
            return Attribute.Strength;
        }

        #endregion
    }
}