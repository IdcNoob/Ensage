namespace CompleteLastHitMarker.Menus.Abilities
{
    using Ensage.Common.Menu;

    using SharpDX;

    internal class AbilitiesColor
    {
        public AbilitiesColor(Menu rootMenu)
        {
            var menu = new Menu("Border colors", "borderColors");

            var canBeKilledText = new MenuItem("canBeKilledColor", "Can be killed");
            var cantBeKilledText = new MenuItem("cantBeKilledColor", "Can not be killed");

            menu.AddItem(canBeKilledText);

            var canRed = new MenuItem("canBeKilledRed", "Red").SetValue(new Slider(139, 0, 255));
            menu.AddItem(canRed);
            canRed.ValueChanged += (sender, args) =>
                {
                    CanBeKilledColor = new Color(
                        args.GetNewValue<Slider>().Value,
                        CanBeKilledColor.G,
                        CanBeKilledColor.B);
                    canBeKilledText.SetFontColor(CanBeKilledColor);
                };

            var canGreen = new MenuItem("canBeKilledGreen", "Green").SetValue(new Slider(244, 0, 255));
            menu.AddItem(canGreen);
            canGreen.ValueChanged += (sender, args) =>
                {
                    CanBeKilledColor = new Color(
                        CanBeKilledColor.R,
                        args.GetNewValue<Slider>().Value,
                        CanBeKilledColor.B);
                    canBeKilledText.SetFontColor(CanBeKilledColor);
                };

            var canBlue = new MenuItem("canBeKilledBlue", "Blue").SetValue(new Slider(102, 0, 255));
            menu.AddItem(canBlue);
            canBlue.ValueChanged += (sender, args) =>
                {
                    CanBeKilledColor = new Color(
                        CanBeKilledColor.R,
                        CanBeKilledColor.G,
                        args.GetNewValue<Slider>().Value);
                    canBeKilledText.SetFontColor(CanBeKilledColor);
                };

            menu.AddItem(cantBeKilledText);

            var cantRed = new MenuItem("cantBeKilledRed", "Red").SetValue(new Slider(222, 0, 255));
            menu.AddItem(cantRed);
            cantRed.ValueChanged += (sender, args) =>
                {
                    CanNotBeKilledColor = new Color(
                        args.GetNewValue<Slider>().Value,
                        CanNotBeKilledColor.G,
                        CanNotBeKilledColor.B);
                    cantBeKilledText.SetFontColor(CanNotBeKilledColor);
                };

            var cantGreen = new MenuItem("cantBeKilledGreen", "Green").SetValue(new Slider(127, 0, 255));
            menu.AddItem(cantGreen);
            cantGreen.ValueChanged += (sender, args) =>
                {
                    CanNotBeKilledColor = new Color(
                        CanNotBeKilledColor.R,
                        args.GetNewValue<Slider>().Value,
                        CanNotBeKilledColor.B);
                    cantBeKilledText.SetFontColor(CanNotBeKilledColor);
                };

            var cantBlue = new MenuItem("cantBeKilledBlue", "Blue").SetValue(new Slider(97, 0, 255));
            menu.AddItem(cantBlue);
            cantBlue.ValueChanged += (sender, args) =>
                {
                    CanNotBeKilledColor = new Color(
                        CanNotBeKilledColor.R,
                        CanNotBeKilledColor.G,
                        args.GetNewValue<Slider>().Value);
                    cantBeKilledText.SetFontColor(CanNotBeKilledColor);
                };

            CanBeKilledColor = new Color(
                canRed.GetValue<Slider>().Value,
                canGreen.GetValue<Slider>().Value,
                canBlue.GetValue<Slider>().Value);

            CanNotBeKilledColor = new Color(
                cantRed.GetValue<Slider>().Value,
                cantGreen.GetValue<Slider>().Value,
                cantBlue.GetValue<Slider>().Value);

            canBeKilledText.SetFontColor(CanBeKilledColor);
            cantBeKilledText.SetFontColor(CanNotBeKilledColor);

            rootMenu.AddSubMenu(menu);
        }

        public Color CanBeKilledColor { get; private set; }

        public Color CanNotBeKilledColor { get; private set; }
    }
}