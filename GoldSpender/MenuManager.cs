namespace GoldSpender
{
    using System.Linq;

    using Ensage.Common.Menu;

    internal class MenuManager
    {
        #region Fields

        private readonly MenuItem nearDeath;

        private readonly MenuItem nearDeathAutoDisable;

        private readonly MenuItem nearDeathEnemyDistance;

        private readonly MenuItem nearDeathHp;

        private readonly MenuItem nearDeathItems;

        private readonly MenuItem testItem;

        #endregion

        #region Constructors and Destructors

        public MenuManager()
        {
            var menu = new Menu("Gold Spender", "goldSpender", true, "alchemist_goblins_greed", true);

            var nearDeathMenu = new Menu("Near death", "nearDeath");

            nearDeathMenu.AddItem(nearDeath = new MenuItem("nearDeathEnabled", "Enabled").SetValue(true));
            nearDeathMenu.AddItem(
                nearDeathHp = new MenuItem("nearDeathHp", "HP threshold %").SetValue(new Slider(15, 1, 99)));
            nearDeathMenu.AddItem(
                nearDeathEnemyDistance =
                new MenuItem("nearDeathEnemyDistance", "Enemy distance").SetValue(new Slider(600, 0, 2000)));
            nearDeathMenu.AddItem(
                nearDeathItems =
                new MenuItem("priorityNearDeathItems", "Items to buy").SetValue(
                    new PriorityChanger(
                    Variables.ItemsToBuy.Select(x => x.Key).ToList(),
                    "priorityChangerNearDeathItems",
                    true)));
            nearDeathMenu.AddItem(
                nearDeathAutoDisable =
                new MenuItem("nearDeathAutoDisable", "Auto disable after (mins)").SetValue(new Slider(20, 0, 60)));

            menu.AddItem(testItem = new MenuItem("test", "Test")).SetValue(false);

            menu.AddSubMenu(nearDeathMenu);
            menu.AddToMainMenu();
        }

        #endregion

        #region Public Properties

        public int NearDeathAutoDisableTime => nearDeathAutoDisable.GetValue<Slider>().Value;

        public bool NearDeathEnabled => nearDeath.IsActive();

        public bool TestEnabled => testItem.IsActive();

        public int NearDeathEnemyDistance => nearDeathEnemyDistance.GetValue<Slider>().Value;

        public int NearDeathHpThreshold => nearDeathHp.GetValue<Slider>().Value;

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