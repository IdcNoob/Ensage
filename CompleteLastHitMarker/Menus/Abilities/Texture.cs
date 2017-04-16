namespace CompleteLastHitMarker.Menus.Abilities
{
    using Ensage.Common.Menu;

    internal class Texture
    {
        public Texture(Menu rootMenu)
        {
            var menu = new Menu("Texture", "textures");

            var size = new MenuItem("textureSize", "Texture size").SetValue(new Slider(30, 20, 40));
            menu.AddItem(size);
            size.ValueChanged += (sender, args) => Size = args.GetNewValue<Slider>().Value;
            Size = size.GetValue<Slider>().Value;

            var x = new MenuItem("textureX", "X coordinate").SetValue(new Slider(0, -100, 100));
            menu.AddItem(x);
            x.ValueChanged += (sender, args) => X = args.GetNewValue<Slider>().Value;
            X = x.GetValue<Slider>().Value;

            var y = new MenuItem("textureY", "Y coordinate").SetValue(new Slider(0, -100, 100));
            menu.AddItem(y);
            y.ValueChanged += (sender, args) => Y = args.GetNewValue<Slider>().Value;
            Y = y.GetValue<Slider>().Value;

            rootMenu.AddSubMenu(menu);
        }

        public int Size { get; private set; }

        public int X { get; private set; }

        public int Y { get; private set; }
    }
}