namespace PredictedCreepsLocation
{
    using Ensage.Common.Menu;

    internal class MenuManager
    {
        #region Fields

        private readonly MenuItem showOnMap;

        private readonly MenuItem showOnMapSize;

        private readonly MenuItem showOnMinimap;

        private readonly MenuItem showOnMinimapSize;

        #endregion

        #region Constructors and Destructors

        public MenuManager()
        {
            var menu = new Menu("PCL", "predictedCreepsLocation", true);

            menu.AddItem(showOnMap = new MenuItem("showOnMap", "Show on map").SetValue(true));
            menu.AddItem(showOnMapSize = new MenuItem("mapSize", "Map size").SetValue(new Slider(50, 25, 75)));
            menu.AddItem(showOnMinimap = new MenuItem("showOnMinimap", "Show on minimap").SetValue(true));
            menu.AddItem(
                    showOnMinimapSize = new MenuItem("minimapSize", "Minimap size").SetValue(new Slider(25, 15, 30)))
                .SetTooltip("Requires assembly reload");

            menu.AddToMainMenu();
        }

        #endregion

        #region Public Properties

        public bool ShowOnMapEnabled => showOnMap.GetValue<bool>();

        public int ShowOnMapSize => showOnMapSize.GetValue<Slider>().Value;

        public bool ShowOnMinimapEnabled => showOnMinimap.GetValue<bool>();

        public int ShowOnMinimapSize => showOnMinimapSize.GetValue<Slider>().Value;

        #endregion
    }
}