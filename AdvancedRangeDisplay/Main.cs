using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Objects;

namespace AdvancedRangeDisplay {
    using Ensage.Common.Objects.UtilityObjects;

    internal static class Main {
        public static Dictionary<string, uint> AbilitiesDictionary = new Dictionary<string, uint>();
        public static Dictionary<string, uint> ItemsDictionary = new Dictionary<string, uint>();
        public static Dictionary<string, float> CustomRangesDictionary = new Dictionary<string, float>();
        public static HashSet<string> ItemsSet = new HashSet<string>();
        private static readonly List<Hero> HeroesLens = new List<Hero>();

        public static Hero Hero { get; private set; }

        public static void Init() {
            Events.OnLoad += Events_OnLoad;
            Events.OnClose += Events_OnClose;
        }

        private static void Events_OnClose(object sender, EventArgs e)
        {
            Game.OnIngameUpdate -= Game_OnUpdate;
            AbilitiesDictionary.Clear();
            ItemsDictionary.Clear();
            CustomRangesDictionary.Clear();
            HeroesLens.Clear();
            ItemsSet.Clear();
            HeroesAdded.Clear();
            RangesMenu.HeroMenu.Clear();
            RangesMenu.Menu.RemoveFromMainMenu();
            RangesMenu.Menu = null;
        }

        private static Sleeper sleeper;

        private static void Events_OnLoad(object sender, EventArgs e)
        {
            Hero = ObjectManager.LocalHero;

            RangesMenu.Init();
            Drawings.Init();

            sleeper = new Sleeper();

            Game.OnIngameUpdate += Game_OnUpdate;
        }

        private static readonly List<uint> HeroesAdded = new List<uint>(); 

        private static void Game_OnUpdate(EventArgs args) {
            if (sleeper.Sleeping)
                return;

            sleeper.Sleep(5555);

            if (Game.IsPaused)
                return;

            var allHeroes = Heroes.All.Where(x => x.IsValid && !x.IsIllusion);

            foreach (var hero in allHeroes) {
                var heroName = hero.StoredName();

                if (!RangesMenu.HeroMenu.ContainsKey(hero)) {
                    if (!HeroesAdded.Contains(hero.Handle))
                    {
                        RangesMenu.AddSpells(hero);
                        RangesMenu.AddCustomItem(hero, "attribute_bonus", 1300, "Experience range");
                        HeroesAdded.Add(hero.Handle);
                    }
                    continue;
                }

                foreach (var spell in
                    hero.Spellbook.Spells.Where(x => Drawings.ParticleDictionary.ContainsKey(heroName + x.StoredName()))
                    ) {
                    var key = heroName + spell.StoredName();
                    uint savedLevel;
                    AbilitiesDictionary.TryGetValue(key, out savedLevel);
                    var spellLevel = spell.Level;
                    if (savedLevel == spellLevel) continue;
                    AbilitiesDictionary[key] = spellLevel;
                    Drawings.ChangeRange(key, hero);
                }

                if (hero.FindItem("item_aether_lens") != null) {
                    if (!HeroesLens.Contains(hero)) {
                        Drawings.UpdateAbilityRanges(hero);
                        HeroesLens.Add(hero);
                    }
                }
                else {
                    if (HeroesLens.Contains(hero)) {
                        Drawings.UpdateAbilityRanges(hero);
                        HeroesLens.Remove(hero);
                    }
                }

                foreach (var item in hero.Inventory.Items) {
                    var key = heroName + item.StoredName().GetDefaultName();

                    if (!ItemsSet.Contains(key)) {
                        RangesMenu.AddItem(hero, item);
                    }
                    else if (Drawings.ParticleDictionary.ContainsKey(key)) {
                        uint savedLevel;
                        ItemsDictionary.TryGetValue(key, out savedLevel);
                        var spellLevel = item.Level;
                        if (savedLevel == spellLevel) continue;
                        ItemsDictionary[key] = spellLevel;
                        Drawings.ChangeRange(key, hero);
                    }
                }

                foreach (var key in AbilitiesDictionary.Keys.Where(x => x.StartsWith(heroName))
                    .Concat(ItemsDictionary.Keys.Where(x => x.StartsWith(heroName)))) {
                    var ability = hero.FindAbility(key.Substring(heroName.Length));

                    if (ability == null) {
                        if (Drawings.ParticleDictionary.ContainsKey(key)) {
                            Drawings.DisposeRange(key);
                        }
                    }
                    else {
                        if (RangesMenu.Menu.Item(key + "enabled").GetValue<bool>() &&
                            !Drawings.ParticleDictionary.ContainsKey(key) && ability.Level > 0) {
                            Drawings.DrawRange(hero,
                                ability is Item ? ability.StoredName().GetDefaultName() : ability.StoredName(),
                                true);
                        }
                    }
                }

                foreach (var key in CustomRangesDictionary.Keys.Where(x =>
                    !Drawings.ParticleDictionary.ContainsKey(x) &&
                    x.StartsWith(heroName) &&
                    RangesMenu.Menu.Item(x + "enabled").GetValue<bool>())) {
                    Drawings.DrawRange(hero, key.Substring(hero.StoredName().Length), true, customRange: true);
                }
            }
        }

        public static string GetDefaultName(this string name) {
            return char.IsDigit(name[name.Length - 1]) ? name.Remove(name.Length - 2) : name;
        }

        public static Ability FindAbility(this Unit unit, string name) {
            return unit.FindSpell(name) ?? unit.FindItem(name) ?? unit.GetLeveledItem(name.GetDefaultName());
        }
    }
}