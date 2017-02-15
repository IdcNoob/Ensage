namespace HpMpAbuse.Items
{
    using System.Collections.Generic;

    using Ensage;
    using Ensage.Common.Enums;
    using Ensage.Common.Extensions;

    internal class TranquilBoots
    {
        #region Constructors and Destructors

        public TranquilBoots(string name)
        {
            Name = name;
        }

        #endregion

        #region Public Properties

        public Item Item { get; private set; }

        public List<ItemId> RequiredItems { get; } = new List<ItemId>
        {
            ItemId.item_ring_of_regen,
            ItemId.item_ring_of_protection,
            ItemId.item_boots
        };

        #endregion

        #region Properties

        protected static Hero Hero => Variables.Hero;

        private string Name { get; }

        #endregion

        #region Public Methods and Operators

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

        #endregion
    }
}