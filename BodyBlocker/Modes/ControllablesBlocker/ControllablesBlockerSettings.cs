namespace BodyBlocker.Modes.ControllablesBlocker
{
    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    internal class ControllablesBlockerSettings
    {
        public ControllablesBlockerSettings()
        {
            var subFactory = MenuFactory.Attach("bodyBlocker").Menu("Controllables blocker");
            Key = subFactory.Item("Hotkey", new KeyBind('-'));
            Key.Item.Tooltip = "Block enemy hero with controllable units (Press)";
            BlockSensitivity = subFactory.Item("Block sensitivity", new Slider(150, 100, 200));
            BlockSensitivity.Item.Tooltip = "Bigger value will result in smaller block, but with higher success rate";
            ControllablesCount = subFactory.Item("Units", new Slider(1, 1, 5));
            ControllablesCount.Item.Tooltip = "Number of units to use";
            SpreadUnits = subFactory.Item("Spread units", true);
            SpreadUnits.Item.Tooltip =
                "If enabled units will try to form an arc, otherwise they all will run in front of the hero";
        }

        public MenuItem<Slider> BlockSensitivity { get; }

        public MenuItem<Slider> ControllablesCount { get; }

        public MenuItem<KeyBind> Key { get; }

        public MenuItem<bool> SpreadUnits { get; }
    }
}