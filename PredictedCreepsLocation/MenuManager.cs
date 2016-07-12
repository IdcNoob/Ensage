namespace PredictedCreepsLocation
{
    using Ensage.Common.Menu;

    internal class MenuManager
    {
        #region Fields

        private readonly MenuItem showOnMap;

        private readonly MenuItem showOnMapIcon;

        private readonly MenuItem showOnMapSize;

        private readonly MenuItem showOnMinimap;

        private readonly MenuItem showOnMinimapIcon;

        private readonly MenuItem showOnMinimapSize;

        #endregion

        #region Constructors and Destructors

        public MenuManager()
        {
            var menu = new Menu("PCL", "predictedCreepsLocation", true);

            menu.AddItem(showOnMap = new MenuItem("showOnMap", "Show on map").SetValue(true));
            menu.AddItem(showOnMapIcon = new MenuItem("mapIcon", "Map icon").SetValue(false))
                .SetTooltip("If enabled will shows creep icon, otherwise will show \"C\"");
            menu.AddItem(showOnMapSize = new MenuItem("mapSize", "Map size").SetValue(new Slider(50, 25, 75)));
            menu.AddItem(showOnMinimap = new MenuItem("showOnMinimap", "Show on minimap").SetValue(true));
            menu.AddItem(showOnMinimapIcon = new MenuItem("minimapIcon", "Minimap icon").SetValue(false))
                .SetTooltip("If enabled will shows creep icon, otherwise will show \"C\"");
            menu.AddItem(
                showOnMinimapSize = new MenuItem("minimapSize", "Minimap size").SetValue(new Slider(25, 15, 40)));

            menu.AddToMainMenu();
        }

        #endregion

        #region Public Properties

        public bool ShowOnMapEnabled => showOnMap.GetValue<bool>();

        public bool ShowOnMapIcon => showOnMapIcon.GetValue<bool>();

        public float ShowOnMapSize => showOnMapSize.GetValue<Slider>().Value;

        public bool ShowOnMinimapEnabled => showOnMinimap.GetValue<bool>();

        public bool ShowOnMinimapIcon => showOnMinimapIcon.GetValue<bool>();

        public float ShowOnMinimapSize => showOnMinimapSize.GetValue<Slider>().Value;

        #endregion
    }
}