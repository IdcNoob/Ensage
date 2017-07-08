namespace CompleteLastHitMarker.Menus.Abilities
{
    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    using SharpDX;

    using Utils;

    internal class AbilitiesColor
    {
        public AbilitiesColor(MenuFactory factory)
        {
            var subFactory = factory.Menu("Border colors");

            var canBeKilledText = new MenuItem("canBeKilledColor", "Can be killed");
            var cantBeKilledText = new MenuItem("cantBeKilledColor", "Can not be killed");

            subFactory.Target.AddItem(canBeKilledText);

            var canRed = subFactory.Item("Red", "canBeKilledRed", new Slider(139, 0, 255));
            canRed.Item.ValueChanged += (sender, args) =>
                {
                    CanBeKilledColor = CanBeKilledColor.SetRed(args.GetNewValue<Slider>().Value);
                    canBeKilledText.SetFontColor(CanBeKilledColor);
                };

            var canGreen = subFactory.Item("Green", "canBeKilledGreen", new Slider(244, 0, 255));
            canGreen.Item.ValueChanged += (sender, args) =>
                {
                    CanBeKilledColor = CanBeKilledColor.SetGreen(args.GetNewValue<Slider>().Value);
                    canBeKilledText.SetFontColor(CanBeKilledColor);
                };

            var canBlue = subFactory.Item("Blue", "canBeKilledBlue", new Slider(102, 0, 255));
            canBlue.Item.ValueChanged += (sender, args) =>
                {
                    CanBeKilledColor = CanBeKilledColor.SetBlue(args.GetNewValue<Slider>().Value);
                    canBeKilledText.SetFontColor(CanBeKilledColor);
                };

            subFactory.Target.AddItem(cantBeKilledText);

            var cantRed = subFactory.Item("Red", "cantBeKilledRed", new Slider(222, 0, 255));
            cantRed.Item.ValueChanged += (sender, args) =>
                {
                    CanNotBeKilledColor = CanNotBeKilledColor.SetRed(args.GetNewValue<Slider>().Value);
                    cantBeKilledText.SetFontColor(CanNotBeKilledColor);
                };

            var cantGreen = subFactory.Item("Green", "cantBeKilledGreen", new Slider(127, 0, 255));
            cantGreen.Item.ValueChanged += (sender, args) =>
                {
                    CanNotBeKilledColor = CanNotBeKilledColor.SetGreen(args.GetNewValue<Slider>().Value);
                    cantBeKilledText.SetFontColor(CanNotBeKilledColor);
                };

            var cantBlue = subFactory.Item("Blue", "cantBeKilledBlue", new Slider(97, 0, 255));
            cantBlue.Item.ValueChanged += (sender, args) =>
                {
                    CanNotBeKilledColor = CanNotBeKilledColor.SetGreen(args.GetNewValue<Slider>().Value);
                    cantBeKilledText.SetFontColor(CanNotBeKilledColor);
                };

            CanBeKilledColor = new Color(canRed, canGreen, canBlue);
            CanNotBeKilledColor = new Color(cantRed, cantGreen, cantBlue);

            canBeKilledText.SetFontColor(CanBeKilledColor);
            cantBeKilledText.SetFontColor(CanNotBeKilledColor);
        }

        public Color CanBeKilledColor { get; private set; }

        public Color CanNotBeKilledColor { get; private set; }
    }
}