namespace CompleteLastHitMarker.Menus.AutoAttack
{
    using Ensage;
    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    using SharpDX;

    using Utils;

    internal class AutoAttackColors
    {
        public AutoAttackColors(MenuFactory factory)
        {
            var subFactory = factory.Menu("Colors");

            var allyColorsFactory = subFactory.Menu("Ally");
            var enemyColorsFactory = subFactory.Menu("Enemy");

            var canBeKilledAllyText = new MenuItem("canBeKilledAllyColor", "Can be killed");
            var canBeKilledEnemyText = new MenuItem("canBeKilledEnemyColor", "Can be killed");
            var cantBeKilledAllyText = new MenuItem("cantBeKilledAllyColor", "Can not be killed");
            var cantBeKilledEnemyText = new MenuItem("cantBeKilledEnemyColor", "Can not be killed");
            var defaultAllyHpText = new MenuItem("normalAllyHpColor", "Default health color");
            var defaultEnemyHpText = new MenuItem("normalEnemyHpColor", "Default health color");

            allyColorsFactory.Target.AddItem(canBeKilledAllyText);

            var canAllyRed = allyColorsFactory.Item("Red", "canBeKilledAllyRed", new Slider(139, 0, 255));
            canAllyRed.Item.ValueChanged += (sender, args) =>
                {
                    CanBeKilledAllyColor = CanBeKilledAllyColor.SetRed(args.GetNewValue<Slider>().Value);
                    canBeKilledAllyText.SetFontColor(CanBeKilledAllyColor);
                };

            var canAllyGreen = allyColorsFactory.Item("Green", "canBeKilledAllyGreen", new Slider(244, 0, 255));
            canAllyGreen.Item.ValueChanged += (sender, args) =>
                {
                    CanBeKilledAllyColor = CanBeKilledAllyColor.SetGreen(args.GetNewValue<Slider>().Value);
                    canBeKilledAllyText.SetFontColor(CanBeKilledAllyColor);
                };

            var canAllyBlue = allyColorsFactory.Item("Blue", "canBeKilledAllyBlue", new Slider(102, 0, 255));
            canAllyBlue.Item.ValueChanged += (sender, args) =>
                {
                    CanBeKilledAllyColor = CanBeKilledAllyColor.SetBlue(args.GetNewValue<Slider>().Value);
                    canBeKilledAllyText.SetFontColor(CanBeKilledAllyColor);
                };

            allyColorsFactory.Target.AddItem(cantBeKilledAllyText);

            var cantAllyRed = allyColorsFactory.Item("Red", "cantBeKilledAllyRed", new Slider(19, 0, 255));
            cantAllyRed.Item.ValueChanged += (sender, args) =>
                {
                    CanNotBeKilledAllyColor = CanNotBeKilledAllyColor.SetRed(args.GetNewValue<Slider>().Value);
                    cantBeKilledAllyText.SetFontColor(CanNotBeKilledAllyColor);
                };

            var cantAllyGreen = allyColorsFactory.Item("Green", "cantBeKilledAllyGreen", new Slider(173, 0, 255));
            cantAllyGreen.Item.ValueChanged += (sender, args) =>
                {
                    CanNotBeKilledAllyColor = CanNotBeKilledAllyColor.SetGreen(args.GetNewValue<Slider>().Value);
                    cantBeKilledAllyText.SetFontColor(CanNotBeKilledAllyColor);
                };

            var cantAllyBlue = allyColorsFactory.Item("Blue", "cantBeKilledAllyBlue", new Slider(16, 0, 255));
            cantAllyBlue.Item.ValueChanged += (sender, args) =>
                {
                    CanNotBeKilledAllyColor = CanNotBeKilledAllyColor.SetBlue(args.GetNewValue<Slider>().Value);
                    cantBeKilledAllyText.SetFontColor(CanNotBeKilledAllyColor);
                };

            allyColorsFactory.Target.AddItem(defaultAllyHpText);

            var defaultAllyRed = allyColorsFactory.Item("Red", "defaultAllyRed", new Slider(20, 0, 255));
            defaultAllyRed.Item.ValueChanged += (sender, args) =>
                {
                    DefaultAllyHealthColor = DefaultAllyHealthColor.SetRed(args.GetNewValue<Slider>().Value);
                    defaultAllyHpText.SetFontColor(DefaultAllyHealthColor);
                };

            var defaultAllyGreen = allyColorsFactory.Item("Green", "defaultAllyGreen", new Slider(107, 0, 255));
            defaultAllyGreen.Item.ValueChanged += (sender, args) =>
                {
                    DefaultAllyHealthColor = DefaultAllyHealthColor.SetGreen(args.GetNewValue<Slider>().Value);
                    defaultAllyHpText.SetFontColor(DefaultAllyHealthColor);
                };

            var defaultAllyBlue = allyColorsFactory.Item("Blue", "defaultAllyBlue", new Slider(41, 0, 255));
            defaultAllyBlue.Item.ValueChanged += (sender, args) =>
                {
                    DefaultAllyHealthColor = DefaultAllyHealthColor.SetBlue(args.GetNewValue<Slider>().Value);
                    defaultAllyHpText.SetFontColor(DefaultAllyHealthColor);
                };

            enemyColorsFactory.Target.AddItem(canBeKilledEnemyText);

            var canEnemyRed = enemyColorsFactory.Item("Red", "canBeKilledEnemyRed", new Slider(222, 0, 255));
            canEnemyRed.Item.ValueChanged += (sender, args) =>
                {
                    CanBeKilledEnemyColor = CanBeKilledEnemyColor.SetRed(args.GetNewValue<Slider>().Value);
                    canBeKilledEnemyText.SetFontColor(CanBeKilledEnemyColor);
                };

            var canEnemyGreen = enemyColorsFactory.Item("Green", "canBeKilledEnemyGreen", new Slider(127, 0, 255));
            canEnemyGreen.Item.ValueChanged += (sender, args) =>
                {
                    CanBeKilledEnemyColor = CanBeKilledEnemyColor.SetGreen(args.GetNewValue<Slider>().Value);
                    canBeKilledEnemyText.SetFontColor(CanBeKilledEnemyColor);
                };

            var canEnemyBlue = enemyColorsFactory.Item("Blue", "canBeKilledEnemyBlue", new Slider(97, 0, 255));
            canEnemyBlue.Item.ValueChanged += (sender, args) =>
                {
                    CanBeKilledEnemyColor = CanBeKilledEnemyColor.SetBlue(args.GetNewValue<Slider>().Value);
                    canBeKilledEnemyText.SetFontColor(CanBeKilledEnemyColor);
                };

            enemyColorsFactory.Target.AddItem(cantBeKilledEnemyText);

            var cantEnemyRed = enemyColorsFactory.Item("Red", "cantBeKilledEnemyRed", new Slider(158, 0, 255));
            cantEnemyRed.Item.ValueChanged += (sender, args) =>
                {
                    CanNotBeKilledEnemyColor = CanNotBeKilledEnemyColor.SetRed(args.GetNewValue<Slider>().Value);
                    cantBeKilledEnemyText.SetFontColor(CanNotBeKilledEnemyColor);
                };

            var cantEnemyGreen = enemyColorsFactory.Item("Green", "cantBeKilledEnemyGreen", new Slider(77, 0, 255));
            cantEnemyGreen.Item.ValueChanged += (sender, args) =>
                {
                    CanNotBeKilledEnemyColor = CanNotBeKilledEnemyColor.SetGreen(args.GetNewValue<Slider>().Value);
                    cantBeKilledEnemyText.SetFontColor(CanNotBeKilledEnemyColor);
                };

            var cantEnemyBlue = enemyColorsFactory.Item("Blue", "cantBeKilledEnemyBlue", new Slider(39, 0, 255));
            cantEnemyBlue.Item.ValueChanged += (sender, args) =>
                {
                    CanNotBeKilledEnemyColor = CanNotBeKilledEnemyColor.SetBlue(args.GetNewValue<Slider>().Value);
                    cantBeKilledEnemyText.SetFontColor(CanNotBeKilledEnemyColor);
                };

            enemyColorsFactory.Target.AddItem(defaultEnemyHpText);

            var defaultEnemyRed = enemyColorsFactory.Item("Red", "defaultEnemyRed", new Slider(109, 0, 255));
            defaultEnemyRed.Item.ValueChanged += (sender, args) =>
                {
                    DefaultEnemyHealthColor = DefaultEnemyHealthColor.SetRed(args.GetNewValue<Slider>().Value);
                    defaultEnemyHpText.SetFontColor(DefaultEnemyHealthColor);
                };

            var defaultEnemyGreen = enemyColorsFactory.Item("Green", "defaultEnemyGreen", new Slider(41, 0, 255));
            defaultEnemyGreen.Item.ValueChanged += (sender, args) =>
                {
                    DefaultEnemyHealthColor = DefaultEnemyHealthColor.SetGreen(args.GetNewValue<Slider>().Value);
                    defaultEnemyHpText.SetFontColor(DefaultEnemyHealthColor);
                };

            var defaultEnemyBlue = enemyColorsFactory.Item("Blue", "defaultEnemyBlue", new Slider(0, 0, 255));
            defaultEnemyBlue.Item.ValueChanged += (sender, args) =>
                {
                    DefaultEnemyHealthColor = DefaultEnemyHealthColor.SetBlue(args.GetNewValue<Slider>().Value);
                    defaultEnemyHpText.SetFontColor(DefaultEnemyHealthColor);
                };

            CanBeKilledAllyColor = new Color(canAllyRed, canAllyGreen, canAllyBlue);
            CanNotBeKilledAllyColor = new Color(cantAllyRed, cantAllyGreen, cantAllyBlue);
            DefaultAllyHealthColor = new Color(defaultAllyRed, defaultAllyGreen, defaultAllyBlue);
            CanBeKilledEnemyColor = new Color(canEnemyRed, canEnemyGreen, canEnemyBlue);
            CanNotBeKilledEnemyColor = new Color(cantEnemyRed, cantEnemyGreen, cantEnemyBlue);
            DefaultEnemyHealthColor = new Color(defaultEnemyRed, defaultEnemyGreen, defaultEnemyBlue);

            canBeKilledAllyText.SetFontColor(CanBeKilledAllyColor);
            cantBeKilledAllyText.SetFontColor(CanNotBeKilledAllyColor);
            defaultAllyHpText.SetFontColor(DefaultAllyHealthColor);
            canBeKilledEnemyText.SetFontColor(CanBeKilledEnemyColor);
            cantBeKilledEnemyText.SetFontColor(CanNotBeKilledEnemyColor);
            defaultEnemyHpText.SetFontColor(DefaultEnemyHealthColor);
        }

        public Color CanBeKilledAllyColor { get; private set; }

        public Color CanBeKilledEnemyColor { get; private set; }

        public Color CanNotBeKilledAllyColor { get; private set; }

        public Color CanNotBeKilledEnemyColor { get; private set; }

        public Color DefaultAllyHealthColor { get; private set; }

        public Color DefaultEnemyHealthColor { get; private set; }

        public Color CanBeKilledColor(Team source, Team target)
        {
            return source == target ? CanBeKilledAllyColor : CanBeKilledEnemyColor;
        }

        public Color CanNotBeKilledColor(Team source, Team target)
        {
            return source == target ? CanNotBeKilledAllyColor : CanNotBeKilledEnemyColor;
        }

        public Color DefaultHealthColor(Team source, Team target)
        {
            return source == target ? DefaultAllyHealthColor : DefaultEnemyHealthColor;
        }
    }
}