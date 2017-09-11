namespace AdvancedRangeDisplay
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using Ensage.Common.Objects;

    using SharpDX;

    internal class MenuManager
    {
        private readonly Dictionary<Hero, Menu> abilityMenus = new Dictionary<Hero, Menu>();

        private readonly Dictionary<string, Item> addedItems;

        private readonly Dictionary<Hero, MenuItem> enabledItemsMenu = new Dictionary<Hero, MenuItem>();

        private readonly Dictionary<Hero, Menu> heroMenus = new Dictionary<Hero, Menu>();

        private readonly Dictionary<Hero, Menu> itemMenus = new Dictionary<Hero, Menu>();

        private readonly Menu menu;

        private readonly Hero myHero;

        private readonly List<string> radiusOnlyAbilities = new List<string>
        {
            "nevermore_shadowraze1",
            "nevermore_shadowraze2",
            "nevermore_shadowraze3",
            "kunkka_tidebringer",
        };

        public MenuManager(Dictionary<string, Item> addedItems)
        {
            myHero = ObjectManager.LocalHero;
            this.addedItems = addedItems;

            menu = new Menu("Advanced Ranges", "advancedRanges", true);

            AddCreepRangesMenu();
            menu.AddToMainMenu();
        }

        public event EventHandler<AbilityEventArgs> OnChange;

        public event EventHandler<BoolEventArgs> OnCreepChange;

        public event EventHandler<EventArgs> OnCreepColorChange;

        public int CreepBlueColor { get; private set; }

        public int CreepGreenColor { get; private set; }

        public int CreepRedColor { get; private set; }

        public bool ShowCreepAggroRange { get; private set; }

        public void AddHeroMenu(Hero hero)
        {
            var heroName = hero.StoredName();
            var heroMenu = new Menu(hero.GetRealName(), heroName, false, heroName, true);

            menu.AddSubMenu(heroMenu);
            heroMenus.Add(hero, heroMenu);

            var key = hero.StoredName();
            if (hero.Equals(myHero))
            {
                key += "myHero";
            }

            var settings = new Menu("Settings", key + "settings");
            heroMenus.First(x => x.Key.Equals(hero)).Value.AddSubMenu(settings);

            var abilities = new MenuItem(key + "generateAbilities", "Generate ability ranges").SetValue(false);
            settings.AddItem(abilities);

            abilities.ValueChanged += (sender, args) =>
                {
                    if (!args.GetNewValue<bool>())
                    {
                        args.Process = false;
                        return;
                    }
                    AddAbilities(hero);
                };

            if (abilities.IsActive())
            {
                AddAbilities(hero);
            }

            var items = new MenuItem(key + "generateItems", "Generate item ranges").SetValue(false);
            enabledItemsMenu.Add(hero, items);
            settings.AddItem(items);

            items.ValueChanged += (sender, args) =>
                {
                    if (!args.GetNewValue<bool>())
                    {
                        args.Process = false;
                        return;
                    }
                    AddAItems(hero);
                };

            if (items.IsActive())
            {
                AddAItems(hero);
            }

            var attack = new MenuItem(key + "generateAttack", "Generate attack range").SetValue(false);
            settings.AddItem(attack);

            attack.ValueChanged += (sender, args) =>
                {
                    if (!args.GetNewValue<bool>())
                    {
                        args.Process = false;
                        return;
                    }
                    AddMenuItem(hero, null, Ranges.CustomRange.Attack);
                };

            if (attack.IsActive())
            {
                AddMenuItem(hero, null, Ranges.CustomRange.Attack);
            }

            var experience = new MenuItem(key + "generateExpirience", "Generate experience range").SetValue(false);
            settings.AddItem(experience);

            experience.ValueChanged += (sender, args) =>
                {
                    if (!args.GetNewValue<bool>())
                    {
                        args.Process = false;
                        return;
                    }
                    AddMenuItem(hero, null, Ranges.CustomRange.Experience);
                };

            if (experience.IsActive())
            {
                AddMenuItem(hero, null, Ranges.CustomRange.Experience);
            }

            var aggro = new MenuItem(key + "generateAggro", "Generate agrro range").SetValue(false);
            settings.AddItem(aggro);

            aggro.ValueChanged += (sender, args) =>
                {
                    if (!args.GetNewValue<bool>())
                    {
                        args.Process = false;
                        return;
                    }
                    AddMenuItem(hero, null, Ranges.CustomRange.Aggro);
                };

            if (aggro.IsActive())
            {
                AddMenuItem(hero, null, Ranges.CustomRange.Aggro);
            }
        }

        public void AddMenuItem(Hero hero, Ability ability, Ranges.CustomRange customRange = Ranges.CustomRange.None)
        {
            var isItem = ability is Item;
            var abilityName = isItem ? ability.GetDefaultName() : ability?.StoredName();
            var texture = abilityName;
            var menuName = string.Empty;

            switch (customRange)
            {
                case Ranges.CustomRange.Attack:
                    abilityName = "attackRange";
                    menuName = "Attack range";
                    texture = null;
                    break;
                case Ranges.CustomRange.Experience:
                    abilityName = "expRange";
                    menuName = "Exp range";
                    texture = null;
                    break;
                case Ranges.CustomRange.Aggro:
                    abilityName = "aggroRange";
                    menuName = "Aggro range";
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
            var radiusOnly = new MenuItem(key + "radius", "Damage radius only").SetValue(false);
            var red = new MenuItem(key + "red", "Red").SetValue(new Slider(255, 0, 255));
            var green = new MenuItem(key + "green", "Green").SetValue(new Slider(0, 0, 255));
            var blue = new MenuItem(key + "blue", "Blue").SetValue(new Slider(0, 0, 255));

            enable.ValueChanged += (sender, arg) =>
                {
                    var enabled = arg.GetNewValue<bool>();
                    abilityMenu.DisplayName =
                        enabled ? abilityMenu.DisplayName + "*" : abilityMenu.DisplayName.Replace("*", "");
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
            }
            abilityMenu.AddItem(red.SetFontColor(Color.IndianRed));
            abilityMenu.AddItem(green.SetFontColor(Color.LightGreen));
            abilityMenu.AddItem(blue.SetFontColor(Color.LightBlue));

            if (customRange == Ranges.CustomRange.None)
            {
                Menu abilitiesMenu;
                var dictionary = isItem ? abilityMenus : itemMenus;
                if (!dictionary.TryGetValue(hero, out abilitiesMenu))
                {
                    abilitiesMenu = new Menu(isItem ? "Items" : "Abilities", key + (isItem ? "items" : "abilities"));
                    heroMenus.First(x => x.Key.Equals(hero)).Value.AddSubMenu(abilitiesMenu);
                    dictionary.Add(hero, abilitiesMenu);
                }

                abilitiesMenu.AddSubMenu(abilityMenu);
            }
            else
            {
                heroMenus.First(x => x.Key.Equals(hero)).Value.AddSubMenu(abilityMenu);
            }

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

            abilityMenu.DisplayName =
                enable.IsActive() ? abilityMenu.DisplayName + "  *" : abilityMenu.DisplayName + "  ";
        }

        public bool IsItemsMenuEnabled(Hero hero)
        {
            MenuItem item;
            return enabledItemsMenu.TryGetValue(hero, out item) && item.IsActive();
        }

        public void OnClose()
        {
            menu.RemoveFromMainMenu();
        }

        private void AddAbilities(Hero hero)
        {
            foreach (var ability in hero.Spellbook.Spells.Where(
                x => !x.IsHidden && !x.Name.Contains("special_bonus") && !x.Name.Contains("empty")))
            {
                AddMenuItem(hero, ability);
            }
        }

        private void AddAItems(Hero hero)
        {
            foreach (var item in hero.Inventory.Items)
            {
                var itemName = item.GetDefaultName();

                if (string.IsNullOrEmpty(itemName) || addedItems.ContainsKey(hero.StoredName() + itemName))
                {
                    continue;
                }

                addedItems.Add(hero.StoredName() + itemName, item);

                if (item.GetRealCastRange() > 500)
                {
                    AddMenuItem(hero, item);
                }
            }
        }

        private void AddCreepRangesMenu()
        {
            var creepsMenu = new Menu("Creeps", "creepRanges");
            var aggroRangeMenu = new Menu("Aggro range", "aggroRangeMenu");

            var creepsAggro = new MenuItem("creepsAggro", "Aggro range").SetValue(false);
            creepsAggro.ValueChanged += (sender, args) =>
                {
                    ShowCreepAggroRange = args.GetNewValue<bool>();
                    OnCreepChange?.Invoke(
                        this,
                        new BoolEventArgs
                        {
                            Enabled = ShowCreepAggroRange
                        });
                };
            ShowCreepAggroRange = creepsAggro.IsActive();
            aggroRangeMenu.AddItem(creepsAggro);

            var red = new MenuItem("creepsAggroRed", "Red").SetValue(new Slider(255, 0, 255));
            red.ValueChanged += (sender, args) =>
                {
                    CreepRedColor = args.GetNewValue<Slider>().Value;
                    OnCreepColorChange?.Invoke(this, EventArgs.Empty);
                };
            CreepRedColor = red.GetValue<Slider>().Value;
            aggroRangeMenu.AddItem(red);

            var green = new MenuItem("creepsAggroGreen", "Green").SetValue(new Slider(0, 0, 255));
            green.ValueChanged += (sender, args) =>
                {
                    CreepGreenColor = args.GetNewValue<Slider>().Value;
                    OnCreepColorChange?.Invoke(this, EventArgs.Empty);
                };
            CreepGreenColor = green.GetValue<Slider>().Value;
            aggroRangeMenu.AddItem(green);

            var blue = new MenuItem("creepsAggroBlue", "Blue").SetValue(new Slider(0, 0, 255));
            blue.ValueChanged += (sender, args) =>
                {
                    CreepBlueColor = args.GetNewValue<Slider>().Value;
                    OnCreepColorChange?.Invoke(this, EventArgs.Empty);
                };
            CreepBlueColor = blue.GetValue<Slider>().Value;
            aggroRangeMenu.AddItem(blue);

            creepsMenu.AddSubMenu(aggroRangeMenu);
            menu.AddSubMenu(creepsMenu);
        }
    }
}