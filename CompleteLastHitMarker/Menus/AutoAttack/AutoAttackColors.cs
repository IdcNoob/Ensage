namespace CompleteLastHitMarker.Menus.AutoAttack
{
    using Ensage;
    using Ensage.Common.Menu;

    using SharpDX;

    internal class AutoAttackColors
    {
        public AutoAttackColors(Menu rootMenu)
        {
            // i fucking hope there is no mistakes

            var menu = new Menu("Colors", "autoAttackColors");

            var allyColorsMenu = new Menu("Ally", "allyColors");
            var enemyColorsMenu = new Menu("Enemy", "enemyColors");

            var canBeKilledAllyText = new MenuItem("canBeKilledAllyColor", "Can be killed");
            var canBeKilledEnemyText = new MenuItem("canBeKilledEnemyColor", "Can be killed");
            var cantBeKilledAllyText = new MenuItem("cantBeKilledAllyColor", "Can not be killed");
            var cantBeKilledEnemyText = new MenuItem("cantBeKilledEnemyColor", "Can not be killed");
            var defaultAllyHpText = new MenuItem("normalAllyHpColor", "Default health color");
            var defaultEnemyHpText = new MenuItem("normalEnemyHpColor", "Default health color");

            allyColorsMenu.AddItem(canBeKilledAllyText);

            var canAllyRed = new MenuItem("canBeKilledAllyRed", "Red").SetValue(new Slider(139, 0, 255));
            allyColorsMenu.AddItem(canAllyRed);
            canAllyRed.ValueChanged += (sender, args) =>
                {
                    CanBeKilledAllyColor = new Color(
                        args.GetNewValue<Slider>().Value,
                        CanBeKilledAllyColor.G,
                        CanBeKilledAllyColor.B);
                    canBeKilledAllyText.SetFontColor(CanBeKilledAllyColor);
                };

            var canAllyGreen = new MenuItem("canBeKilledAllyGreen", "Green").SetValue(new Slider(244, 0, 255));
            allyColorsMenu.AddItem(canAllyGreen);
            canAllyGreen.ValueChanged += (sender, args) =>
                {
                    CanBeKilledAllyColor = new Color(
                        CanBeKilledAllyColor.R,
                        args.GetNewValue<Slider>().Value,
                        CanBeKilledAllyColor.B);
                    canBeKilledAllyText.SetFontColor(CanBeKilledAllyColor);
                };

            var canAllyBlue = new MenuItem("canBeKilledAllyBlue", "Blue").SetValue(new Slider(102, 0, 255));
            allyColorsMenu.AddItem(canAllyBlue);
            canAllyBlue.ValueChanged += (sender, args) =>
                {
                    CanBeKilledAllyColor = new Color(
                        CanBeKilledAllyColor.R,
                        CanBeKilledAllyColor.G,
                        args.GetNewValue<Slider>().Value);
                    canBeKilledAllyText.SetFontColor(CanBeKilledAllyColor);
                };

            allyColorsMenu.AddItem(cantBeKilledAllyText);

            var cantAllyRed = new MenuItem("cantBeKilledAllyRed", "Red").SetValue(new Slider(19, 0, 255));
            allyColorsMenu.AddItem(cantAllyRed);
            cantAllyRed.ValueChanged += (sender, args) =>
                {
                    CanNotBeKilledAllyColor = new Color(
                        args.GetNewValue<Slider>().Value,
                        CanNotBeKilledAllyColor.G,
                        CanNotBeKilledAllyColor.B);
                    cantBeKilledAllyText.SetFontColor(CanNotBeKilledAllyColor);
                };

            var cantAllyGreen = new MenuItem("cantBeKilledAllyGreen", "Green").SetValue(new Slider(173, 0, 255));
            allyColorsMenu.AddItem(cantAllyGreen);
            cantAllyGreen.ValueChanged += (sender, args) =>
                {
                    CanNotBeKilledAllyColor = new Color(
                        CanNotBeKilledAllyColor.R,
                        args.GetNewValue<Slider>().Value,
                        CanNotBeKilledAllyColor.B);
                    cantBeKilledAllyText.SetFontColor(CanNotBeKilledAllyColor);
                };

            var cantAllyBlue = new MenuItem("cantBeKilledAllyBlue", "Blue").SetValue(new Slider(16, 0, 255));
            allyColorsMenu.AddItem(cantAllyBlue);
            cantAllyBlue.ValueChanged += (sender, args) =>
                {
                    CanNotBeKilledAllyColor = new Color(
                        CanNotBeKilledAllyColor.R,
                        CanNotBeKilledAllyColor.G,
                        args.GetNewValue<Slider>().Value);
                    cantBeKilledAllyText.SetFontColor(CanNotBeKilledAllyColor);
                };

            allyColorsMenu.AddItem(defaultAllyHpText);

            var defaultAllyRed = new MenuItem("defaultAllyRed", "Red").SetValue(new Slider(20, 0, 255));
            allyColorsMenu.AddItem(defaultAllyRed);
            defaultAllyRed.ValueChanged += (sender, args) =>
                {
                    DefaultAllyHealthColor = new Color(
                        args.GetNewValue<Slider>().Value,
                        DefaultAllyHealthColor.G,
                        DefaultAllyHealthColor.B);
                    defaultAllyHpText.SetFontColor(DefaultAllyHealthColor);
                };

            var defaultAllyGreen = new MenuItem("defaultAllyGreen", "Green").SetValue(new Slider(107, 0, 255));
            allyColorsMenu.AddItem(defaultAllyGreen);
            defaultAllyGreen.ValueChanged += (sender, args) =>
                {
                    DefaultAllyHealthColor = new Color(
                        DefaultAllyHealthColor.R,
                        args.GetNewValue<Slider>().Value,
                        DefaultAllyHealthColor.B);
                    defaultAllyHpText.SetFontColor(DefaultAllyHealthColor);
                };

            var defaultAllyBlue = new MenuItem("defaultAllyBlue", "Blue").SetValue(new Slider(41, 0, 255));
            allyColorsMenu.AddItem(defaultAllyBlue);
            defaultAllyBlue.ValueChanged += (sender, args) =>
                {
                    DefaultAllyHealthColor = new Color(
                        DefaultAllyHealthColor.R,
                        DefaultAllyHealthColor.G,
                        args.GetNewValue<Slider>().Value);
                    defaultAllyHpText.SetFontColor(DefaultAllyHealthColor);
                };

            enemyColorsMenu.AddItem(canBeKilledEnemyText);

            var canEnemyRed = new MenuItem("canBeKilledEnemyRed", "Red").SetValue(new Slider(222, 0, 255));
            enemyColorsMenu.AddItem(canEnemyRed);
            canEnemyRed.ValueChanged += (sender, args) =>
                {
                    CanBeKilledEnemyColor = new Color(
                        args.GetNewValue<Slider>().Value,
                        CanBeKilledEnemyColor.G,
                        CanBeKilledEnemyColor.B);
                    canBeKilledEnemyText.SetFontColor(CanBeKilledEnemyColor);
                };

            var canEnemyGreen = new MenuItem("canBeKilledEnemyGreen", "Green").SetValue(new Slider(127, 0, 255));
            enemyColorsMenu.AddItem(canEnemyGreen);
            canEnemyGreen.ValueChanged += (sender, args) =>
                {
                    CanBeKilledEnemyColor = new Color(
                        CanBeKilledEnemyColor.R,
                        args.GetNewValue<Slider>().Value,
                        CanBeKilledEnemyColor.B);
                    canBeKilledEnemyText.SetFontColor(CanBeKilledEnemyColor);
                };

            var canEnemyBlue = new MenuItem("canBeKilledEnemyBlue", "Blue").SetValue(new Slider(97, 0, 255));
            enemyColorsMenu.AddItem(canEnemyBlue);
            canEnemyBlue.ValueChanged += (sender, args) =>
                {
                    CanBeKilledEnemyColor = new Color(
                        CanBeKilledEnemyColor.R,
                        CanBeKilledEnemyColor.G,
                        args.GetNewValue<Slider>().Value);
                    canBeKilledEnemyText.SetFontColor(CanBeKilledEnemyColor);
                };

            enemyColorsMenu.AddItem(cantBeKilledEnemyText);

            var cantEnemyRed = new MenuItem("cantBeKilledEnemyRed", "Red").SetValue(new Slider(158, 0, 255));
            enemyColorsMenu.AddItem(cantEnemyRed);
            cantEnemyRed.ValueChanged += (sender, args) =>
                {
                    CanNotBeKilledEnemyColor = new Color(
                        args.GetNewValue<Slider>().Value,
                        CanNotBeKilledEnemyColor.G,
                        CanNotBeKilledEnemyColor.B);
                    cantBeKilledEnemyText.SetFontColor(CanNotBeKilledEnemyColor);
                };

            var cantEnemyGreen = new MenuItem("cantBeKilledEnemyGreen", "Green").SetValue(new Slider(77, 0, 255));
            enemyColorsMenu.AddItem(cantEnemyGreen);
            cantEnemyGreen.ValueChanged += (sender, args) =>
                {
                    CanNotBeKilledEnemyColor = new Color(
                        CanNotBeKilledEnemyColor.R,
                        args.GetNewValue<Slider>().Value,
                        CanNotBeKilledEnemyColor.B);
                    cantBeKilledEnemyText.SetFontColor(CanNotBeKilledEnemyColor);
                };

            var cantEnemyBlue = new MenuItem("cantBeKilledEnemyBlue", "Blue").SetValue(new Slider(39, 0, 255));
            enemyColorsMenu.AddItem(cantEnemyBlue);
            cantEnemyBlue.ValueChanged += (sender, args) =>
                {
                    CanNotBeKilledEnemyColor = new Color(
                        CanNotBeKilledEnemyColor.R,
                        CanNotBeKilledEnemyColor.G,
                        args.GetNewValue<Slider>().Value);
                    cantBeKilledEnemyText.SetFontColor(CanNotBeKilledEnemyColor);
                };

            enemyColorsMenu.AddItem(defaultEnemyHpText);

            var defaultEnemyRed = new MenuItem("defaultEnemyRed", "Red").SetValue(new Slider(109, 0, 255));
            enemyColorsMenu.AddItem(defaultEnemyRed);
            defaultEnemyRed.ValueChanged += (sender, args) =>
                {
                    DefaultEnemyHealthColor = new Color(
                        args.GetNewValue<Slider>().Value,
                        DefaultEnemyHealthColor.G,
                        DefaultEnemyHealthColor.B);
                    defaultEnemyHpText.SetFontColor(DefaultEnemyHealthColor);
                };

            var defaultEnemyGreen = new MenuItem("defaultEnemyGreen", "Green").SetValue(new Slider(41, 0, 255));
            enemyColorsMenu.AddItem(defaultEnemyGreen);
            defaultEnemyGreen.ValueChanged += (sender, args) =>
                {
                    DefaultEnemyHealthColor = new Color(
                        DefaultEnemyHealthColor.R,
                        args.GetNewValue<Slider>().Value,
                        DefaultEnemyHealthColor.B);
                    defaultEnemyHpText.SetFontColor(DefaultEnemyHealthColor);
                };

            var defaultEnemyBlue = new MenuItem("defaultEnemyBlue", "Blue").SetValue(new Slider(0, 0, 255));
            enemyColorsMenu.AddItem(defaultEnemyBlue);
            defaultEnemyBlue.ValueChanged += (sender, args) =>
                {
                    DefaultEnemyHealthColor = new Color(
                        DefaultEnemyHealthColor.R,
                        DefaultEnemyHealthColor.G,
                        args.GetNewValue<Slider>().Value);
                    defaultEnemyHpText.SetFontColor(DefaultEnemyHealthColor);
                };

            CanBeKilledAllyColor = new Color(
                canAllyRed.GetValue<Slider>().Value,
                canAllyGreen.GetValue<Slider>().Value,
                canAllyBlue.GetValue<Slider>().Value);

            CanNotBeKilledAllyColor = new Color(
                cantAllyRed.GetValue<Slider>().Value,
                cantAllyGreen.GetValue<Slider>().Value,
                cantAllyBlue.GetValue<Slider>().Value);

            DefaultAllyHealthColor = new Color(
                defaultAllyRed.GetValue<Slider>().Value,
                defaultAllyGreen.GetValue<Slider>().Value,
                defaultAllyBlue.GetValue<Slider>().Value);

            CanBeKilledEnemyColor = new Color(
                canEnemyRed.GetValue<Slider>().Value,
                canEnemyGreen.GetValue<Slider>().Value,
                canEnemyBlue.GetValue<Slider>().Value);

            CanNotBeKilledEnemyColor = new Color(
                cantEnemyRed.GetValue<Slider>().Value,
                cantEnemyGreen.GetValue<Slider>().Value,
                cantEnemyBlue.GetValue<Slider>().Value);

            DefaultEnemyHealthColor = new Color(
                defaultEnemyRed.GetValue<Slider>().Value,
                defaultEnemyGreen.GetValue<Slider>().Value,
                defaultEnemyBlue.GetValue<Slider>().Value);

            canBeKilledAllyText.SetFontColor(CanBeKilledAllyColor);
            cantBeKilledAllyText.SetFontColor(CanNotBeKilledAllyColor);
            defaultAllyHpText.SetFontColor(DefaultAllyHealthColor);
            canBeKilledEnemyText.SetFontColor(CanBeKilledEnemyColor);
            cantBeKilledEnemyText.SetFontColor(CanNotBeKilledEnemyColor);
            defaultEnemyHpText.SetFontColor(DefaultEnemyHealthColor);

            menu.AddSubMenu(allyColorsMenu);
            menu.AddSubMenu(enemyColorsMenu);

            rootMenu.AddSubMenu(menu);
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