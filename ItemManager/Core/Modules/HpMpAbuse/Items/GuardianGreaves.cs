namespace ItemManager.Core.Modules.HpMpAbuse.Items
{
    using System.Linq;

    using Ensage;

    internal sealed class GuardianGreaves : UsableItem
    {
        public GuardianGreaves(string name)
            : base(name)
        {
            ManaRestore = Ability.GetAbilityDataByName(Name)
                .AbilitySpecialData.First(x => x.Name == "replenish_mana")
                .Value;

            HealthRestore = Ability.GetAbilityDataByName(Name)
                .AbilitySpecialData.First(x => x.Name == "replenish_health")
                .Value;
        }

        public override bool CanBeCasted()
        {
            return base.CanBeCasted() && Menu.RecoveryMenu.IsAbilityEnabled(Name)
                   && (Hero.Health < Hero.MaximumHealth || Hero.Mana < Hero.MaximumMana);
        }

        public override Attribute GetPowerTreadsAttribute()
        {
            return Attribute.Agility;
        }
    }
}