namespace BodyBlocker.Modes.HeroBlocker
{
    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    internal class HeroBlockerSettings
    {
        public HeroBlockerSettings()
        {
            var subFactory = MenuFactory.Attach("bodyBlocker").Menu("Hero blocker");
            Key = subFactory.Item("Hotkey", new KeyBind('-'));
            Key.Item.Tooltip = "Block enemy hero with your hero  (Hold)";
            BlockSensitivity = subFactory.Item("Block sensitivity", new Slider(150, 100, 200));
            BlockSensitivity.Item.Tooltip = "Bigger value will result in smaller block, but with higher success rate";
            CenterCamera = subFactory.Item("Center camera", true);
        }

        public MenuItem<Slider> BlockSensitivity { get; }

        public MenuItem<bool> CenterCamera { get; }

        public MenuItem<KeyBind> Key { get; }
    }
}