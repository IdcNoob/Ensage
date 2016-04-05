using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using Ensage.Common.Objects;
using SharpDX;

namespace AdvancedRangeDisplay {
    internal static class RangesMenu {
        public static Dictionary<Hero, Menu> HeroMenu = new Dictionary<Hero, Menu>();

        public static Menu Menu { get; private set; }

        public static void Init() {
            if (Menu != null) {
                Menu.RemoveFromMainMenu();
                HeroMenu.Clear();
            }
            InitMenu();
        }

        public static void AddSpells(Hero hero) {
            var heroName = hero.StoredName();

            var heroMenu = new Menu(string.Empty, heroName, false, heroName);

            HeroMenu.Add(hero, heroMenu);

            foreach (
                var spell in
                    hero.Spellbook.Spells.Where(x => x.ClassID != ClassID.CDOTA_Ability_AttributeBonus)) {
                var spellName = spell.StoredName();
                var key = heroName + spellName;

                var spellMenu = new Menu(string.Empty, key, false, spellName);

                spellMenu.AddItem(new MenuItem(key + "enabled", "Enabled")).SetValue(false)
                    .ValueChanged += (sender, arg) => { Drawings.DrawRange(hero, spellName, arg.GetNewValue<bool>()); };

                spellMenu.AddItem(new MenuItem(key + "bonus", "Bonus range").SetValue(new Slider(0, -300, 300)))
                    .ValueChanged +=
                    (sender, arg) => { Drawings.ChangeRange(key, hero, arg.GetNewValue<Slider>().Value); };

                spellMenu.AddItem(new MenuItem(key + "red", "Red").SetValue(new Slider(255, 0, 255)))
                    .SetFontStyle(fontColor: Color.IndianRed)
                    .ValueChanged +=
                    (sender, arg) => { Drawings.ChangeColor(key, arg.GetNewValue<Slider>().Value, Color.Red); };

                spellMenu.AddItem(new MenuItem(key + "green", "Green").SetValue(new Slider(0, 0, 255)))
                    .SetFontStyle(fontColor: Color.LightGreen)
                    .ValueChanged +=
                    (sender, arg) => { Drawings.ChangeColor(key, arg.GetNewValue<Slider>().Value, Color.Green); };

                spellMenu.AddItem(new MenuItem(key + "blue", "Blue").SetValue(new Slider(0, 0, 255)))
                    .SetFontStyle(fontColor: Color.LightBlue)
                    .ValueChanged +=
                    (sender, arg) => { Drawings.ChangeColor(key, arg.GetNewValue<Slider>().Value, Color.Blue); };

                heroMenu.AddSubMenu(spellMenu);

                Main.AbilitiesDictionary.Add(key, Math.Max(spell.Level, 1));
            }

            Menu.AddSubMenu(heroMenu);
        }

        public static void AddItem(Hero hero, Item item) {
            Menu rangeMenu;
            HeroMenu.TryGetValue(hero, out rangeMenu);

            if (rangeMenu == null)
                return;

            var castRange = item.GetCastRange();
            if (castRange >= 5000) castRange = item.GetRadius();

            var itemName = item.StoredName().GetDefaultName();
            var key = hero.StoredName() + itemName;

            if (castRange > 0 && castRange <= 5000) {
                var addItem = new Menu(string.Empty, key, false, itemName);

                addItem.AddItem(new MenuItem(key + "enabled", "Enabled"))
                    .SetValue(false)
                    .ValueChanged += (sender, arg) => { Drawings.DrawRange(hero, itemName, arg.GetNewValue<bool>()); };

                addItem.AddItem(
                    new MenuItem(key + "bonus", "Bonus range").SetValue(new Slider(0, -300, 300)))
                    .ValueChanged +=
                    (sender, arg) => { Drawings.ChangeRange(key, hero, arg.GetNewValue<Slider>().Value); };

                addItem.AddItem(new MenuItem(key + "red", "Red").SetValue(new Slider(255, 0, 255)))
                    .SetFontStyle(fontColor: Color.IndianRed)
                    .ValueChanged +=
                    (sender, arg) => { Drawings.ChangeColor(key, arg.GetNewValue<Slider>().Value, Color.Red); };

                addItem.AddItem(new MenuItem(key + "green", "Green").SetValue(new Slider(0, 0, 255)))
                    .SetFontStyle(fontColor: Color.LightGreen)
                    .ValueChanged +=
                    (sender, arg) => { Drawings.ChangeColor(key, arg.GetNewValue<Slider>().Value, Color.Green); };

                addItem.AddItem(new MenuItem(key + "blue", "Blue").SetValue(new Slider(0, 0, 255)))
                    .SetFontStyle(fontColor: Color.LightBlue)
                    .ValueChanged +=
                    (sender, arg) => { Drawings.ChangeColor(key, arg.GetNewValue<Slider>().Value, Color.Blue); };

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

            var addItem = new Menu(string.Empty, key, false, texture);

            addItem.AddItem(new MenuItem(key + "enabled", "Enabled"))
                .SetValue(false)
                .SetTooltip(tooltip)
                .ValueChanged +=
                (sender, arg) => { Drawings.DrawRange(hero, texture, arg.GetNewValue<bool>(), customRange: true); };

            addItem.AddItem(
                new MenuItem(key + "bonus", "Bonus range").SetValue(new Slider(0, -300, 300)))
                .ValueChanged +=
                (sender, arg) => { Drawings.ChangeRange(key, hero, arg.GetNewValue<Slider>().Value, true); };

            addItem.AddItem(new MenuItem(key + "red", "Red").SetValue(new Slider(255, 0, 255)))
                .SetFontStyle(fontColor: Color.IndianRed)
                .ValueChanged +=
                (sender, arg) => { Drawings.ChangeColor(key, arg.GetNewValue<Slider>().Value, Color.Red); };

            addItem.AddItem(new MenuItem(key + "green", "Green").SetValue(new Slider(0, 0, 255)))
                .SetFontStyle(fontColor: Color.LightGreen)
                .ValueChanged +=
                (sender, arg) => { Drawings.ChangeColor(key, arg.GetNewValue<Slider>().Value, Color.Green); };

            addItem.AddItem(new MenuItem(key + "blue", "Blue").SetValue(new Slider(0, 0, 255)))
                .SetFontStyle(fontColor: Color.LightBlue)
                .ValueChanged +=
                (sender, arg) => { Drawings.ChangeColor(key, arg.GetNewValue<Slider>().Value, Color.Blue); };

            rangeMenu.AddSubMenu(addItem);

            Main.CustomRangesDictionary.Add(key, range);
        }

        private static void InitMenu() {
            Menu = new Menu("Advanced Ranges", "advancedRanges", true);
            Menu.AddToMainMenu();
        }
    }
}