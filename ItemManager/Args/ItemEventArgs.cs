namespace ItemManager.Args
{
    using System;

    using Ensage.Common.Enums;

    internal class ItemEventArgs : EventArgs
    {
        #region Constructors and Destructors

        public ItemEventArgs(ItemId itemId, bool bought)
        {
            ItemId = itemId;
            Bought = bought;
        }

        #endregion

        #region Public Properties

        public bool Bought { get; }

        public ItemId ItemId { get; }

        #endregion
    }
}