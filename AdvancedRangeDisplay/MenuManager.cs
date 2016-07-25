namespace AdvancedRangeDisplay
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using Ensage.Common.Objects;

    using SharpDX;

    internal class MenuManager
    {
        #region Fields

        private readonly Dictionary<Hero, Menu> heroMenus = new Dictionary<Hero, Menu>();

        private readonly Menu menu;

        #endregion

        #region Constructors and Destructors

        public MenuManager()
        {
            menu = new Menu("Advanced Ranges", "advancedRanges", true);
            menu.AddToMainMenu();
        }

        #endregion

        #region Public Events

        public event EventHandler<AbilityArgs> OnChange;

        #endregion

        #region Public Methods and Operators

        public async void AddHeroMenu(Hero hero)
        {
            var heroName = hero.StoredName();
            var heroMenu = new Menu(hero.GetRealName(), heroName, false, heroName, true);
            heroMenus.Add(hero, heroMenu);

            foreach (var ability in hero.Spellbook.Spells.Where(x => !x.IsHidden))
            {
                AddMenuItem(hero, ability);
                await Task.Delay(222);
            }

            menu.AddSubMenu(heroMenu);
        }

        public async void AddMenuItem(Hero hero, Ability ability)
        {
            var abilityName = ability is Item ? ability.GetDefaultName() : ability.StoredName();
            var key = hero.StoredName() + abilityName;

            var spellMenu = new Menu("  ", key, false, abilityName, true);

            var enable = new MenuItem(key + "enabled", "Enabled").SetValue(false);

            if (ability.ClassID == ClassID.CDOTA_Ability_AttributeBonus)
            {
                enable.SetTooltip("Experience range");
            }

            var red = new MenuItem(key + "red", "Red").SetValue(new Slider(255, 0, 255));
            var green = new MenuItem(key + "green", "Green").SetValue(new Slider(0, 0, 255));
            var blue = new MenuItem(key + "blue", "Blue").SetValue(new Slider(0, 0, 255));

            enable.ValueChanged += (sender, arg) =>
                {
                    var enabled = arg.GetNewValue<bool>();
                    spellMenu.DisplayName = enabled ? " *" : "  ";
                    OnChange?.Invoke(
                        this,
                        new AbilityArgs { Hero = hero, Name = abilityName, Enabled = enabled, Redraw = true });
                };

            red.ValueChanged +=
                (sender, arg) =>
                    {
                        OnChange?.Invoke(
                            this,
                            new AbilityArgs { Hero = hero, Name = abilityName, Red = arg.GetNewValue<Slider>().Value });
                    };

            green.ValueChanged +=
                (sender, arg) =>
                    {
                        OnChange?.Invoke(
                            this,
                            new AbilityArgs { Hero = hero, Name = abilityName, Green = arg.GetNewValue<Slider>().Value });
                    };

            blue.ValueChanged +=
                (sender, arg) =>
                    {
                        OnChange?.Invoke(
                            this,
                            new AbilityArgs { Hero = hero, Name = abilityName, Blue = arg.GetNewValue<Slider>().Value });
                    };

            spellMenu.AddItem(enable);
            await Task.Delay(222);
            spellMenu.AddItem(red.SetFontColor(Color.IndianRed));
            await Task.Delay(222);
            spellMenu.AddItem(green.SetFontColor(Color.LightGreen));
            await Task.Delay(222);
            spellMenu.AddItem(blue.SetFontColor(Color.LightBlue));
            await Task.Delay(222);
            heroMenus.First(x => x.Key.Equals(hero)).Value.AddSubMenu(spellMenu);
            await Task.Delay(222);

            OnChange?.Invoke(
                this,
                new AbilityArgs
                    {
                        Hero = hero, Name = abilityName, Enabled = enable.IsActive(), Red = red.GetValue<Slider>().Value,
                        Green = green.GetValue<Slider>().Value, Blue = blue.GetValue<Slider>().Value, Redraw = true
                    });

            spellMenu.DisplayName = enable.IsActive() ? " *" : "  ";
        }

        public void OnClose()
        {
            menu.RemoveFromMainMenu();
            heroMenus.Clear();
        }

        #endregion
    }
}