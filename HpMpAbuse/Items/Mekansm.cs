namespace HpMpAbuse.Items
{
    using System.Linq;

    using Ensage;

    internal sealed class Mekansm : UsableItem
    {
        #region Constructors and Destructors

        public Mekansm(string name)
            : base(name)
        {
            HealthRestore =
                Ability.GetAbilityDataByName(Name).AbilitySpecialData.First(x => x.Name == "heal_amount").Value;
        }

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted()
        {
            return base.CanBeCasted() && Hero.Health < Hero.MaximumHealth;
        }

        public override Attribute GetPowerTreadsAttribute()
        {
            return Attribute.Intelligence;
        }

        #endregion
    }
}