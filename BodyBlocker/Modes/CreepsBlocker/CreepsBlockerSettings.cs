namespace BodyBlocker.Modes.CreepsBlocker
{
    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    internal class CreepsBlockerSettings
    {
        public CreepsBlockerSettings()
        {
            var subFactory = MenuFactory.Attach("bodyBlocker").Menu("Creeps blocker");
            Key = subFactory.Item("Hotkey", new KeyBind('-'));
            Key.Item.Tooltip = "Block lane creeps with your hero (Hold)";
            BlockRangedCreep = subFactory.Item("Block ranged creep", true);
            BlockSensitivity = subFactory.Item("Block sensitivity", new Slider(550, 500, 700));
            BlockSensitivity.Item.Tooltip = "Bigger value will result in smaller block, but with higher success rate";
            CenterCamera = subFactory.Item("Center camera", true);
        }

        public MenuItem<bool> BlockRangedCreep { get; }

        public MenuItem<Slider> BlockSensitivity { get; }

        public MenuItem<bool> CenterCamera { get; }

        public MenuItem<KeyBind> Key { get; }
    }
}