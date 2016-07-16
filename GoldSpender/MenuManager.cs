namespace GoldSpender
{
    using System.Linq;

    using Ensage.Common.Menu;

    internal class MenuManager
    {
        #region Fields

        private readonly MenuItem autoPurchase;

        private readonly MenuItem autoPurchaseSaveBuyback;

        private readonly MenuItem nearDeath;

        private readonly MenuItem nearDeathEnemyDistance;

        private readonly MenuItem nearDeathHp;

        private readonly MenuItem nearDeathItems;

        private readonly MenuItem nearDeathSaveBuyback;

        #endregion

        #region Constructors and Destructors

        public MenuManager()
        {
            var menu = new Menu("Gold Spender", "goldSpender", true, "alchemist_goblins_greed", true);

            var nearDeathMenu = new Menu("Near death", "nearDeath");
            nearDeathMenu.AddItem(nearDeath = new MenuItem("nearDeathEnabled", "Enabled").SetValue(true));
            nearDeathMenu.AddItem(
                nearDeathHp = new MenuItem("nearDeathHp", "HP threshold").SetValue(new Slider(250, 1, 500)));
            nearDeathMenu.AddItem(
                nearDeathEnemyDistance =
                new MenuItem("nearDeathEnemyDistance", "Enemy distance").SetValue(new Slider(600, 0, 2000)));
            nearDeathMenu.AddItem(
                nearDeathItems =
                new MenuItem("priorityNearDeathItems", "Items to buy").SetValue(
                    new PriorityChanger(
                    Variables.ItemsToBuyNearDeath.Select(x => x.Key).ToList(),
                    "priorityChangerNearDeathItems",
                    true)));
            nearDeathMenu.AddItem(
                nearDeathSaveBuyback =
                new MenuItem("nearDeathSaveBuyback", "Save for buyback after (mins)").SetValue(new Slider(30, 0, 60)));

            var autoPurchaseMenu = new Menu("Auto purchase", "autoPurchase");
            autoPurchaseMenu.AddItem(autoPurchase = new MenuItem("autoPurchaseEnabled", "Enabled").SetValue(false))
                .SetTooltip("Auto purchase items from quick buy when you are near shop");
            autoPurchaseMenu.AddItem(
                autoPurchaseSaveBuyback =
                new MenuItem("autoPurchaseSaveBuyback", "Save for buyback after (mins)").SetValue(new Slider(30, 0, 60)));

            menu.AddSubMenu(nearDeathMenu);
            menu.AddSubMenu(autoPurchaseMenu);
            menu.AddToMainMenu();
        }

        #endregion

        #region Public Properties

        public bool AutoPurchaseEnabled => autoPurchase.IsActive();

        public int AutoPurchaseSaveBuybackTime => autoPurchaseSaveBuyback.GetValue<Slider>().Value;

        public bool NearDeathEnabled => nearDeath.IsActive();

        public int NearDeathEnemyDistance => nearDeathEnemyDistance.GetValue<Slider>().Value;

        public int NearDeathHpThreshold => nearDeathHp.GetValue<Slider>().Value;

        public int NearDeathSaveBuybackTime => nearDeathSaveBuyback.GetValue<Slider>().Value;

        #endregion

        #region Public Methods and Operators

        public uint GetNearDeathItemPriority(string itemName)
        {
            return nearDeathItems.GetValue<PriorityChanger>().GetPriority(itemName);
        }

        public bool IsNearDeathItemActive(string itemName)
        {
            return nearDeathItems.GetValue<PriorityChanger>().AbilityToggler.IsEnabled(itemName);
        }

        #endregion
    }
}