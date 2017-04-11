namespace Evader.Core.Menus
{
    using Ensage.Common.Menu;

    internal class RandomiserMenu
    {
        public RandomiserMenu(Menu rootMenu)
        {
            var menu = new Menu("Randomiser", "randomiser");

            var enabled = new MenuItem("enabled", "Enabled").SetValue(false)
                .SetTooltip("Will intentionally fail to counter abilities");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) => Enabled = args.GetNewValue<bool>();
            Enabled = enabled.IsActive();

            var nukesOnly = new MenuItem("nukesOnly", "Nukes only").SetValue(true)
                .SetTooltip("If enabled, only nukes will be randomised and disable abilities will always be countered");
            menu.AddItem(nukesOnly);
            nukesOnly.ValueChanged += (sender, args) => NukesOnly = args.GetNewValue<bool>();
            NukesOnly = nukesOnly.IsActive();

            var failChance = new MenuItem("failChance", "Fail chance").SetValue(new Slider(20, 5, 50));
            menu.AddItem(failChance);
            failChance.ValueChanged += (sender, args) => FailChance = args.GetNewValue<Slider>().Value;
            FailChance = failChance.GetValue<Slider>().Value;

            rootMenu.AddSubMenu(menu);
        }

        public bool Enabled { get; private set; }

        public int FailChance { get; private set; }

        public bool NukesOnly { get; private set; }
    }
}