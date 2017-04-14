namespace ItemManager.EventArgs
{
    using System;

    using Ensage;

    internal class ItemEventArgs : EventArgs
    {
        public ItemEventArgs(Item item, bool isMine)
        {
            Item = item;
            IsMine = isMine;
        }

        public bool IsMine { get; }

        public Item Item { get; }
    }
}