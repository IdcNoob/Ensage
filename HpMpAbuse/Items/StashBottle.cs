namespace HpMpAbuse.Items
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;

    using Helpers;

    internal class StashBottle : UsableItem
    {
        #region Constructors and Destructors

        public StashBottle(string name)
            : base(name)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted()
        {
            return !Sleeper.Sleeping(Name) && Item != null && Hero.HasModifier(Modifiers.FountainRegeneration)
                   && (Hero.Health < Hero.MaximumHealth || Hero.Mana < Hero.MaximumMana);
        }

        public override void FindItem()
        {
            Item = Hero.Inventory.StashItems.FirstOrDefault(x => x.StoredName() == Name);
        }

        public override Attribute GetPowerTreadsAttribute()
        {
            return Attribute.Agility;
        }

        public override void Use(bool queue = true)
        {
            Sleeper.Sleep(100 + Game.Ping, Name);
        }

        #endregion
    }
}