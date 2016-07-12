namespace HpMpAbuse.Items
{
    using System.Linq;

    using Ensage;

    internal sealed class GuardianGreaves : UsableItem
    {
        #region Constructors and Destructors

        public GuardianGreaves(string name)
            : base(name)
        {
            ManaRestore =
                Ability.GetAbilityDataByName(Name).AbilitySpecialData.First(x => x.Name == "replenish_mana").Value;

            HealthRestore =
                Ability.GetAbilityDataByName(Name).AbilitySpecialData.First(x => x.Name == "replenish_health").Value;
        }

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted()
        {
            return base.CanBeCasted() && (Hero.Health < Hero.MaximumHealth || Hero.Mana < Hero.MaximumMana);
        }

        public override Attribute GetPowerTreadsAttribute()
        {
            return Attribute.Agility;
        }

        #endregion
    }
}