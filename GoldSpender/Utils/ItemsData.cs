namespace GoldSpender.Utils
{
    using System.Collections.Generic;

    using Ensage;

    internal static class ItemsData
    {
        #region Static Fields

        private static readonly Dictionary<uint, ShopFlags> ItemShops = new Dictionary<uint, ShopFlags>
        {
            { 1, ShopFlags.Base | ShopFlags.Side }, //Blink Dagger
            { 2, ShopFlags.Base | ShopFlags.Side }, //Blades of Attack
            { 3, ShopFlags.Base | ShopFlags.Side }, //Broadsword
            { 4, ShopFlags.Base | ShopFlags.Side }, //Chainmail
            { 6, ShopFlags.Base | ShopFlags.Side }, //Helm of Iron Will
            { 9, ShopFlags.Secret }, //Platemail
            { 10, ShopFlags.Base | ShopFlags.Side }, //Quarterstaff
            { 11, ShopFlags.Base | ShopFlags.Side }, //Quelling Blade
            { 14, ShopFlags.Base | ShopFlags.Side }, //Slippers of Agility
            { 17, ShopFlags.Base | ShopFlags.Side }, //Belt of Strength
            { 18, ShopFlags.Base | ShopFlags.Side }, //Band of Elvenskin
            { 19, ShopFlags.Base | ShopFlags.Side }, //Robe of the Magi
            { 24, ShopFlags.Secret }, //Ultimate Orb
            { 25, ShopFlags.Base | ShopFlags.Side }, //Gloves of Haste
            { 26, ShopFlags.Base | ShopFlags.Side }, //Morbid Mask
            { 27, ShopFlags.Base | ShopFlags.Side }, //Ring of Regen
            { 28, ShopFlags.Base | ShopFlags.Side }, //Sage's Mask
            { 29, ShopFlags.Base | ShopFlags.Side }, //Boots of Speed
            { 31, ShopFlags.Base | ShopFlags.Side }, //Cloak
            { 32, ShopFlags.Secret }, //Talisman of Evasion
            { 34, ShopFlags.Base | ShopFlags.Side }, //Magic Stick
            { 41, ShopFlags.Base | ShopFlags.Side }, //Bottle 
            { 46, ShopFlags.Base | ShopFlags.Side }, //Town Portal Scroll
            { 51, ShopFlags.Secret }, //Demon Edge
            { 52, ShopFlags.Secret }, //Eaglesong
            { 53, ShopFlags.Secret }, //Reaver
            { 54, ShopFlags.Secret }, //Sacred Relic
            { 55, ShopFlags.Secret }, //Hyperstone
            { 56, ShopFlags.Base | ShopFlags.Side }, //Ring of Health
            { 57, ShopFlags.Base | ShopFlags.Side }, //Void Stone
            { 58, ShopFlags.Secret }, //Mystic Staff
            { 59, ShopFlags.Side | ShopFlags.Secret }, //Energy Booster
            { 60, ShopFlags.Secret }, //Point Booster
            { 61, ShopFlags.Side | ShopFlags.Secret }, //Vitality Booster
            { 181, ShopFlags.Base | ShopFlags.Side }, //Orb of Venom
            { 182, ShopFlags.Base | ShopFlags.Side }, //Stout Shield
            { 240, ShopFlags.Base | ShopFlags.Side }, //Blight Stone
        };

        #endregion

        #region Public Methods and Operators

        public static bool IsPurchasable(uint id, ShopType shop)
        {
            var shopFlags = ShopFlags.None;
            switch (shop)
            {
                case ShopType.Base:
                    shopFlags = ShopFlags.Base;
                    break;
                case ShopType.Side:
                    shopFlags = ShopFlags.Side;
                    break;
                case ShopType.Secret:
                    shopFlags = ShopFlags.Secret;
                    break;
            }

            ShopFlags itemFlags;
            if (!ItemShops.TryGetValue(id, out itemFlags))
            {
                itemFlags = ShopFlags.Base;
            }

            return itemFlags.HasFlag(shopFlags);
        }

        #endregion
    }
}