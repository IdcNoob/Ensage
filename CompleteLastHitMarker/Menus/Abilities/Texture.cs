namespace CompleteLastHitMarker.Menus.Abilities
{
    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    internal class Texture
    {
        public Texture(MenuFactory factory)
        {
            var subFactory = factory.Menu("Texture");

            Size = subFactory.Item("Texture size", new Slider(30, 20, 40));
            X = subFactory.Item("X coordinate", new Slider(0, -100, 100));
            Y = subFactory.Item("Y coordinate", new Slider(0, -100, 100));
        }

        public MenuItem<Slider> Size { get; }

        public MenuItem<Slider> X { get; }

        public MenuItem<Slider> Y { get; }
    }
}