namespace ItemManager.Core.Modules.HpMpAbuse.Items
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;

    using Utils;

    internal sealed class UrnOfShadows : UsableItem
    {
        public UrnOfShadows(string name)
            : base(name)
        {
            HealthRestore = Ability.GetAbilityDataByName(Name)
                .AbilitySpecialData.First(x => x.Name == "soul_heal_amount")
                .Value;
        }

        public override bool CanBeCasted()
        {
            return base.CanBeCasted() && Menu.RecoveryMenu.IsAbilityEnabled(Name) && Item.CurrentCharges > 0
                   && !Hero.HasModifier(ModifierUtils.UrnRegeneration)
                   && Hero.MaximumHealth - Hero.Health >= HealthRestore;
        }

        public override ItemUtils.Stats GetDropItemStats()
        {
            return ItemUtils.Stats.Health;
        }

        public override Attribute GetPowerTreadsAttribute()
        {
            return Attribute.Agility;
        }

        public override void Use(bool queue = true)
        {
            Item.UseAbility(Hero, queue);
            Sleeper.Sleep(1000, Name);
            Sleeper.Sleep(300, "Used");
        }
    }
}