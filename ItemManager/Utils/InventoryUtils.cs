namespace ItemManager.Utils
{
    using System.Collections.Generic;

    using Ensage;

    internal static class InventoryUtils
    {
        public static List<ItemSlot> BackpackSlots { get; } = new List<ItemSlot>
        {
            ItemSlot.BackPack_1,
            ItemSlot.BackPack_2,
            ItemSlot.BackPack_3
        };

        public static List<ItemSlot> InventorySlots { get; } = new List<ItemSlot>
        {
            ItemSlot.InventorySlot_1,
            ItemSlot.InventorySlot_2,
            ItemSlot.InventorySlot_3,
            ItemSlot.InventorySlot_4,
            ItemSlot.InventorySlot_5,
            ItemSlot.InventorySlot_6
        };

        public static List<ItemSlot> StashSlots { get; } = new List<ItemSlot>
        {
            ItemSlot.StashSlot_1,
            ItemSlot.StashSlot_2,
            ItemSlot.StashSlot_3,
            ItemSlot.StashSlot_4,
            ItemSlot.StashSlot_5,
            ItemSlot.StashSlot_6
        };
    }
}