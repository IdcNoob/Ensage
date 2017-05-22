namespace CreepsPosition
{
    using System;

    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    internal class Config : IDisposable
    {
        private readonly MenuFactory factory;

        public Config()
        {
            factory = MenuFactory.Create("Creeps Position");
            Key = factory.Item("Hotkey", new KeyBind(9));
            Key.Item.Tooltip = "Game will freeze for ~1 second!";
            TimeToShow = factory.Item("Time to show (ms)", new Slider(5000, 1000, 10000));
            ShowOnlyNeutrals = factory.Item("Show only neutrals", true);
        }

        public MenuItem<KeyBind> Key { get; }

        public MenuItem<bool> ShowOnlyNeutrals { get; }

        public MenuItem<Slider> TimeToShow { get; }

        public void Dispose()
        {
            factory?.Dispose();
        }
    }
}