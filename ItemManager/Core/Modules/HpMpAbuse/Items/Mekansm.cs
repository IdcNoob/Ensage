namespace ItemManager.Core.Modules.HpMpAbuse.Items
{
    using System.Linq;

    using Ensage;

    internal sealed class Mekansm : UsableItem
    {
        public Mekansm(string name)
            : base(name)
        {
            HealthRestore = Ability.GetAbilityDataByName(Name)
                .AbilitySpecialData.First(x => x.Name == "heal_amount")
                .Value;
        }

        public override bool CanBeCasted()
        {
            return base.CanBeCasted() && Menu.RecoveryMenu.IsAbilityEnabled(Name) && Hero.Health < Hero.MaximumHealth;
        }

        public override Attribute GetPowerTreadsAttribute()
        {
            return Attribute.Intelligence;
        }
    }
}