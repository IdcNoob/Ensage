using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using Ensage.Common.Objects;
using SharpDX;

namespace AdvancedRangeDisplay {
    using System.Threading.Tasks;

    internal static class RangesMenu {
        public static Dictionary<Hero, Menu> HeroMenu = new Dictionary<Hero, Menu>();

        public static Menu Menu { get; set; }

        public static void Init() {
            InitMenu();
        }

        public static async void AddSpells(Hero hero) {
            var heroName = hero.StoredName();

            var heroMenu = new Menu(string.Empty, heroName, false, heroName);

            foreach (
                var spell in
                    hero.Spellbook.Spells.Where(x => x.ClassID != ClassID.CDOTA_Ability_AttributeBonus)) {
                var spellName = spell.StoredName();
                var key = heroName + spellName;

                var spellMenu = new Menu(string.Empty, key, false, spellName);

                spellMenu.AddItem(new MenuItem(key + "enabled", "Enabled")).SetValue(false)
                    .ValueChanged += (sender, arg) => { Drawings.DrawRange(hero, spellName, arg.GetNewValue<bool>()); };
                await Task.Delay(300);
                spellMenu.AddItem(new MenuItem(key + "bonus", "Bonus range").SetValue(new Slider(0, -300, 300)))
                    .ValueChanged +=
                    (sender, arg) => { Drawings.ChangeRange(key, hero, arg.GetNewValue<Slider>().Value); };
                await Task.Delay(300);
                spellMenu.AddItem(new MenuItem(key + "red", "Red").SetValue(new Slider(255, 0, 255)))
                    .SetFontColor(Color.IndianRed)
                    .ValueChanged +=
                    (sender, arg) => { Drawings.ChangeColor(key, arg.GetNewValue<Slider>().Value, Color.Red); };
                await Task.Delay(300);
                spellMenu.AddItem(new MenuItem(key + "green", "Green").SetValue(new Slider(0, 0, 255)))
                    .SetFontColor(Color.LightGreen)
                    .ValueChanged +=
                    (sender, arg) => { Drawings.ChangeColor(key, arg.GetNewValue<Slider>().Value, Color.Green); };
                await Task.Delay(300);
                spellMenu.AddItem(new MenuItem(key + "blue", "Blue").SetValue(new Slider(0, 0, 255)))
                    .SetFontColor(Color.LightBlue)
                    .ValueChanged +=
                    (sender, arg) => { Drawings.ChangeColor(key, arg.GetNewValue<Slider>().Value, Color.Blue); };
                await Task.Delay(300);
                heroMenu.AddSubMenu(spellMenu);

                Main.AbilitiesDictionary.Add(key, Math.Max(spell.Level, 1));
                await Task.Delay(500);
            }

            HeroMenu.Add(hero, heroMenu);
            Menu.AddSubMenu(heroMenu);
        }

        public static void AddItem(Hero hero, Item item) {
            Menu rangeMenu;
            HeroMenu.TryGetValue(hero, out rangeMenu);

            if (rangeMenu == null)
                return;

            var castRange = item.GetCastRange();
            if (castRange >= 5000) castRange = item.GetRadius();

            var key = hero.StoredName() + item.StoredName().GetDefaultName();
            var itemName = item.StoredName().GetDefaultName();

            // yolo temp fixes
            var tempFix = hero;
            var tempFix1 =  hero.StoredName() + item.StoredName().GetDefaultName();
            var tempFix2 = hero.StoredName() + item.StoredName().GetDefaultName();
            var tempFix3 = hero.StoredName() + item.StoredName().GetDefaultName();

            if (castRange > 0 && castRange <= 5000) {
                var addItem = new Menu(string.Empty, key, false, itemName);

                addItem.AddItem(new MenuItem(key + "enabled", "Enabled"))
                    .SetValue(false)
                    .ValueChanged += (sender, arg) => { Drawings.DrawRange(tempFix, itemName, arg.GetNewValue<bool>()); };

                addItem.AddItem(
                    new MenuItem(key + "bonus", "Bonus range").SetValue(new Slider(0, -300, 300)))
                    .ValueChanged +=
                    (sender, arg) => { Drawings.ChangeRange(key, hero, arg.GetNewValue<Slider>().Value); };

                addItem.AddItem(new MenuItem(key + "red", "Red").SetValue(new Slider(255, 0, 255)))
                    .SetFontColor(Color.IndianRed)
                    .ValueChanged +=
                    (sender, arg) => { Drawings.ChangeColor(tempFix1, arg.GetNewValue<Slider>().Value, Color.Red); };

                addItem.AddItem(new MenuItem(key + "green", "Green").SetValue(new Slider(0, 0, 255)))
                    .SetFontColor(Color.LightGreen)
                    .ValueChanged +=
                    (sender, arg) => { Drawings.ChangeColor(tempFix2, arg.GetNewValue<Slider>().Value, Color.Green); };

                addItem.AddItem(new MenuItem(key + "blue", "Blue").SetValue(new Slider(0, 0, 255)))
                    .SetFontColor(Color.LightBlue)
                    .ValueChanged +=
                    (sender, arg) => { Drawings.ChangeColor(tempFix3, arg.GetNewValue<Slider>().Value, Color.Blue); };

                rangeMenu.AddSubMenu(addItem);

                Main.ItemsDictionary.Add(key, item.Level);
            }

            Main.ItemsSet.Add(key);
        }

        public static void AddCustomItem(Hero hero, string texture, float range, string tooltip = null) {
            Menu rangeMenu;
            HeroMenu.TryGetValue(hero, out rangeMenu);

            if (rangeMenu == null)
                return;

            var key = hero.StoredName() + texture;

            // yolo temp fixes
            var tempFix = hero;
            var tempFix1 = texture + hero.StoredName();
            var tempFix2 = texture + hero.StoredName();
            var tempFix3 = texture + hero.StoredName();

            var addItem = new Menu(string.Empty, key, false, texture);

            addItem.AddItem(new MenuItem(key + "enabled", "Enabled"))
                .SetValue(false)
                .SetTooltip(tooltip)
                .ValueChanged +=
                (sender, arg) => { Drawings.DrawRange(tempFix, texture, arg.GetNewValue<bool>(), customRange: true); };

            addItem.AddItem(
                new MenuItem(key + "bonus", "Bonus range").SetValue(new Slider(0, -300, 300)))
                .ValueChanged +=
                (sender, arg) => { Drawings.ChangeRange(key, hero, arg.GetNewValue<Slider>().Value, true); };

            addItem.AddItem(new MenuItem(key + "red", "Red").SetValue(new Slider(255, 0, 255)))
                .SetFontColor(Color.IndianRed)
                .ValueChanged +=
                (sender, arg) => { Drawings.ChangeColor(tempFix1, arg.GetNewValue<Slider>().Value, Color.Red); };

            addItem.AddItem(new MenuItem(key + "green", "Green").SetValue(new Slider(0, 0, 255)))
                .SetFontColor(Color.LightGreen)
                .ValueChanged +=
                (sender, arg) => { Drawings.ChangeColor(tempFix2, arg.GetNewValue<Slider>().Value, Color.Green); };

            addItem.AddItem(new MenuItem(key + "blue", "Blue").SetValue(new Slider(0, 0, 255)))
                .SetFontColor(Color.LightBlue)
                .ValueChanged +=
                (sender, arg) => { Drawings.ChangeColor(tempFix3, arg.GetNewValue<Slider>().Value, Color.Blue); };

            rangeMenu.AddSubMenu(addItem);

            Main.CustomRangesDictionary.Add(key, range);
        }

        private static void InitMenu() {
            Menu = new Menu("Advanced Ranges", "advancedRanges", true);
            Menu.AddToMainMenu();
        }
    }
}