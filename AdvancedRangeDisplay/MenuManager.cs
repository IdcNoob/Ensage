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

        private readonly Hero myHero;

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
            myHero = ObjectManager.LocalHero;

            menu = new Menu("Advanced Ranges", "advancedRanges", true);
            menu.AddToMainMenu();
        }

        #endregion

        #region Public Events

        public event EventHandler<AbilityEventArgs> OnChange;

        #endregion

        #region Public Methods and Operators

        public async Task AddHeroMenu(Hero hero)
        {
            var heroName = hero.StoredName();
            var heroMenu = new Menu(hero.GetRealName(), heroName, false, heroName, true);
            menu.AddSubMenu(heroMenu);
            heroMenus.Add(hero, heroMenu);

            await AddMenuItem(hero, null, Ranges.CustomRange.Attack);
            await AddMenuItem(hero, null, Ranges.CustomRange.Expiriece);

            foreach (var ability in
                hero.Spellbook.Spells.Where(
                    x => !x.IsHidden && x.ClassID != ClassID.CDOTA_Ability_AttributeBonus && !x.Name.Contains("empty")))
            {
                await AddMenuItem(hero, ability);
            }
        }

        public async Task AddMenuItem(
            Hero hero,
            Ability ability,
            Ranges.CustomRange customRange = Ranges.CustomRange.None)
        {
            var abilityName = ability is Item ? ability.GetDefaultName() : ability?.StoredName();
            var texture = abilityName;
            var menuName = string.Empty;

            switch (customRange)
            {
                case Ranges.CustomRange.Attack:
                    abilityName = "attackRange";
                    menuName = "Attack range";
                    texture = null;
                    break;
                case Ranges.CustomRange.Expiriece:
                    abilityName = "expRange";
                    menuName = "Exp range";
                    texture = null;
                    break;
            }

            if (string.IsNullOrEmpty(abilityName))
            {
                return;
            }

            var key = hero.StoredName() + abilityName;

            if (hero.Equals(myHero))
            {
                key += "myHero";
            }

            var abilityMenu = new Menu(texture == null ? menuName : " ", key, false, texture, true);

            var enable = new MenuItem(key + "enabled", "Enabled").SetValue(false);
            await Task.Delay(100);

            var radiusOnly = new MenuItem(key + "radius", "Damage radius only").SetValue(false);
            await Task.Delay(100);

            var red = new MenuItem(key + "red", "Red").SetValue(new Slider(255, 0, 255));
            await Task.Delay(100);

            var green = new MenuItem(key + "green", "Green").SetValue(new Slider(0, 0, 255));
            await Task.Delay(100);

            var blue = new MenuItem(key + "blue", "Blue").SetValue(new Slider(0, 0, 255));
            await Task.Delay(100);

            enable.ValueChanged += (sender, arg) =>
                {
                    var enabled = arg.GetNewValue<bool>();
                    abilityMenu.DisplayName = enabled
                                                  ? abilityMenu.DisplayName + "*"
                                                  : abilityMenu.DisplayName.Replace("*", "");
                    OnChange?.Invoke(
                        this,
                        new AbilityEventArgs
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
                        new AbilityEventArgs
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
                        new AbilityEventArgs
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
                        new AbilityEventArgs
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
                        new AbilityEventArgs
                            {
                                Hero = hero,
                                Name = abilityName,
                                Blue = arg.GetNewValue<Slider>().Value
                            });
                };

            abilityMenu.AddItem(enable);
            if (radiusOnlyAbilities.Contains(abilityName))
            {
                abilityMenu.AddItem(radiusOnly);
                await Task.Delay(50);
            }
            abilityMenu.AddItem(red.SetFontColor(Color.IndianRed));
            await Task.Delay(50);
            abilityMenu.AddItem(green.SetFontColor(Color.LightGreen));
            await Task.Delay(50);
            abilityMenu.AddItem(blue.SetFontColor(Color.LightBlue));
            await Task.Delay(50);
            heroMenus.First(x => x.Key.Equals(hero)).Value.AddSubMenu(abilityMenu);
            await Task.Delay(50);

            OnChange?.Invoke(
                this,
                new AbilityEventArgs
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

            abilityMenu.DisplayName = enable.IsActive()
                                          ? abilityMenu.DisplayName + "  *"
                                          : abilityMenu.DisplayName + "  ";
        }

        public void OnClose()
        {
            menu.RemoveFromMainMenu();
        }

        #endregion
    }
}