namespace CreepsAggro
{
    using System;

    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    internal class Config : IDisposable
    {
        private readonly MenuFactory factory;

        public Config()
        {
            factory = MenuFactory.Create("Creeps Aggro");
            Aggro = factory.Item("Aggro", new KeyBind(187));
            UnAggro = factory.Item("Drop aggro", new KeyBind(189));
            MoveToMousePosition = factory.Item("Move to mouse position", true);
        }

        public MenuItem<KeyBind> Aggro { get; }

        public MenuItem<bool> MoveToMousePosition { get; }

        public MenuItem<KeyBind> UnAggro { get; }

        public void Dispose()
        {
            factory?.Dispose();
        }
    }
}