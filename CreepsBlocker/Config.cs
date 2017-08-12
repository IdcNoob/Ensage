namespace CreepsBlocker
{
    using System;

    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    internal class Config : IDisposable
    {
        private readonly MenuFactory factory;

        public Config()
        {
            factory = MenuFactory.Create("Creeps Blocker");
            Key = factory.Item("Hotkey", new KeyBind('M'));
            BlockRangedCreep = factory.Item("Block ranged creep", true);
            BlockSensitivity = factory.Item("Block sensitivity", new Slider(550, 500, 700));
            BlockSensitivity.Item.Tooltip = "Bigger value will result in smaller block, but with higher success rate";
            CenterCamera = factory.Item("Center camera", true);
        }

        public MenuItem<bool> BlockRangedCreep { get; }

        public MenuItem<Slider> BlockSensitivity { get; }

        public MenuItem<bool> CenterCamera { get; }

        public MenuItem<KeyBind> Key { get; }

        public void Dispose()
        {
            factory?.Dispose();
        }
    }
}