namespace ItemManager.Core.Modules.HpMpAbuse.Items
{
    using System.Collections.Generic;

    using Ensage;
    using Ensage.Common.Extensions;

    internal class TranquilBoots
    {
        public TranquilBoots(string name)
        {
            Name = name;
        }

        public Item Item { get; private set; }

        public List<AbilityId> RequiredItems { get; } = new List<AbilityId>
        {
            AbilityId.item_ring_of_regen,
            AbilityId.item_ring_of_protection,
            AbilityId.item_boots
        };

        protected static Hero Hero => ObjectManager.LocalHero;

        private string Name { get; }

        public bool AssembleTime(int time)
        {
            return Item.AssembledTime + time < Game.RawGameTime;
        }

        public void Disassemble()
        {
            Item.DisassembleItem();
        }

        public void FindItem()
        {
            Item = Hero.FindItem(Name);
        }

        public bool IsValid()
        {
            return Item != null && Item.IsValid;
        }
    }
}