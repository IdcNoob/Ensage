namespace CompleteLastHitMarker.Menus.AutoAttack
{
    using Ensage.Common.Menu;

    internal class AutoAttackHealthBar
    {
        public AutoAttackHealthBar(Menu rootMenu)
        {
            var menu = new Menu("Health bar", "autoAttackHpBar");

            var x = new MenuItem("hpBarX", "X coordinate").SetValue(new Slider(0, -40, 40));
            menu.AddItem(x);
            x.ValueChanged += (sender, args) => X = args.GetNewValue<Slider>().Value;
            X = x.GetValue<Slider>().Value;

            var y = new MenuItem("hpBarY", "Y coordinate").SetValue(new Slider(0, -40, 40));
            menu.AddItem(y);
            y.ValueChanged += (sender, args) => Y = args.GetNewValue<Slider>().Value;
            Y = y.GetValue<Slider>().Value;

            var sizeX = new MenuItem("hpBarSizeX", "X size").SetValue(new Slider(0, 0, 60));
            menu.AddItem(sizeX);
            sizeX.ValueChanged += (sender, args) => SizeX = args.GetNewValue<Slider>().Value;
            SizeX = sizeX.GetValue<Slider>().Value;

            var sizeY = new MenuItem("hpBarSizeY", "Y size").SetValue(new Slider(0, 0, 10));
            menu.AddItem(sizeY);
            sizeY.ValueChanged += (sender, args) => SizeY = args.GetNewValue<Slider>().Value;
            SizeY = sizeY.GetValue<Slider>().Value;

            rootMenu.AddSubMenu(menu);
        }

        public int SizeX { get; private set; }

        public int SizeY { get; private set; }

        public int X { get; private set; }

        public int Y { get; private set; }
    }
}