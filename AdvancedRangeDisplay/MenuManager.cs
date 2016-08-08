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

        private readonly List<string> radiusOnlyAbilities = new List<string>
            {
                "nevermore_shadowraze1",
                "nevermore_shadowraze2",
                "nevermore_shadowraze3"
            };

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

            if (string.IsNullOrEmpty(abilityName))
            {
                return;
            }

            var key = hero.StoredName() + abilityName;
            var abilityMenu = new Menu("  ", key, false, abilityName, true);

            var enable = new MenuItem(key + "enabled", "Enabled").SetValue(false);

            if (ability.ClassID == ClassID.CDOTA_Ability_AttributeBonus)
            {
                enable.SetTooltip("Experience range");
            }

            var radiusOnly = new MenuItem(key + "radius", "Damage radius only").SetValue(false);
            var red = new MenuItem(key + "red", "Red").SetValue(new Slider(255, 0, 255));
            var green = new MenuItem(key + "green", "Green").SetValue(new Slider(0, 0, 255));
            var blue = new MenuItem(key + "blue", "Blue").SetValue(new Slider(0, 0, 255));

            enable.ValueChanged += (sender, arg) =>
                {
                    var enabled = arg.GetNewValue<bool>();
                    abilityMenu.DisplayName = enabled ? " *" : "  ";
                    OnChange?.Invoke(
                        this,
                        new AbilityArgs
                            {
                                Hero = hero,
                                Name = abilityName,
                                Enabled = enabled,
                                Redraw = true
                            });
                };

            radiusOnly.ValueChanged += (sender, args) =>
                {
                    OnChange?.Invoke(
                        this,
                        new AbilityArgs
                            {
                                Hero = hero,
                                Name = abilityName,
                                Enabled = enable.IsActive(),
                                RadiusOnly = args.GetNewValue<bool>(),
                                Redraw = true
                            });
                };

            red.ValueChanged += (sender, arg) =>
                {
                    OnChange?.Invoke(
                        this,
                        new AbilityArgs
                            {
                                Hero = hero,
                                Name = abilityName,
                                Red = arg.GetNewValue<Slider>().Value
                            });
                };

            green.ValueChanged += (sender, arg) =>
                {
                    OnChange?.Invoke(
                        this,
                        new AbilityArgs
                            {
                                Hero = hero,
                                Name = abilityName,
                                Green = arg.GetNewValue<Slider>().Value
                            });
                };

            blue.ValueChanged += (sender, arg) =>
                {
                    OnChange?.Invoke(
                        this,
                        new AbilityArgs
                            {
                                Hero = hero,
                                Name = abilityName,
                                Blue = arg.GetNewValue<Slider>().Value
                            });
                };

            abilityMenu.AddItem(enable);
            await Task.Delay(222);
            if (radiusOnlyAbilities.Contains(abilityName))
            {
                abilityMenu.AddItem(radiusOnly);
                await Task.Delay(222);
            }
            abilityMenu.AddItem(red.SetFontColor(Color.IndianRed));
            await Task.Delay(222);
            abilityMenu.AddItem(green.SetFontColor(Color.LightGreen));
            await Task.Delay(222);
            abilityMenu.AddItem(blue.SetFontColor(Color.LightBlue));
            await Task.Delay(222);
            heroMenus.First(x => x.Key.Equals(hero)).Value.AddSubMenu(abilityMenu);
            await Task.Delay(222);

            OnChange?.Invoke(
                this,
                new AbilityArgs
                    {
                        Hero = hero,
                        Name = abilityName,
                        Enabled = enable.IsActive(),
                        RadiusOnly = radiusOnly.IsActive(),
                        Red = red.GetValue<Slider>().Value,
                        Green = green.GetValue<Slider>().Value,
                        Blue = blue.GetValue<Slider>().Value,
                        Redraw = true
                    });

            abilityMenu.DisplayName = enable.IsActive() ? " *" : "  ";
        }

        public void OnClose()
        {
            menu.RemoveFromMainMenu();
            heroMenus.Clear();
        }

        #endregion
    }
}