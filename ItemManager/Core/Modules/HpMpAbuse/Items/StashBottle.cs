namespace ItemManager.Core.Modules.HpMpAbuse.Items
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;

    using Utils;

    internal class StashBottle : UsableItem
    {
        public StashBottle(string name)
            : base(name)
        {
        }

        public override bool CanBeCasted()
        {
            return !Sleeper.Sleeping(Name) && Item != null && Hero.HasModifier(ModifierUtils.FountainRegeneration)
                   && (Hero.Health < Hero.MaximumHealth || Hero.Mana < Hero.MaximumMana);
        }

        public override void FindItem()
        {
            Item = Hero.Inventory.Stash.FirstOrDefault(x => x.StoredName() == Name);
        }

        public override Attribute GetPowerTreadsAttribute()
        {
            return Attribute.Agility;
        }

        public override void Use(bool queue = true)
        {
            Sleeper.Sleep(100 + Game.Ping, Name);
        }
    }
}