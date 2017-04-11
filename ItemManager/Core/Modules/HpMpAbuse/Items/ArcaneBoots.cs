namespace ItemManager.Core.Modules.HpMpAbuse.Items
{
    using System.Linq;

    using Ensage;

    internal sealed class ArcaneBoots : UsableItem
    {
        public ArcaneBoots(string name)
            : base(name)
        {
            ManaRestore = Ability.GetAbilityDataByName(Name)
                .AbilitySpecialData.First(x => x.Name == "replenish_amount")
                .Value;
        }

        public override bool CanBeCasted()
        {
            return base.CanBeCasted() && Menu.RecoveryMenu.IsAbilityEnabled(Name) && Hero.Mana < Hero.MaximumMana;
        }

        public override Attribute GetPowerTreadsAttribute()
        {
            return Attribute.Strength;
        }
    }
}