using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;

//using Ensage.Common.Menu;

namespace AdvancedRangeDisplay {
    internal static class Main {
        public static Dictionary<string, uint> AbilitiesDictionary = new Dictionary<string, uint>();
        public static Dictionary<string, uint> ItemsDictionary = new Dictionary<string, uint>();
        public static Dictionary<string, float> CustomRangesDictionary = new Dictionary<string, float>();
        public static HashSet<string> ItemsSet = new HashSet<string>();

        private static readonly List<Hero> HeroesLens = new List<Hero>();
        //private static readonly List<string> RemoveFromMenu = new List<string>();

        private static bool nightChanged;
        private static bool inGame;
        public static Hero Hero { get; private set; }

        public static void Init() {
            Game.OnUpdate += Game_OnUpdate;
            Game.OnFireEvent += Game_OnFireEvent;
        }

        private static void Game_OnFireEvent(FireEventEventArgs args) {
            if (args.GameEvent.Name == "dota_tower_kill")
                Drawings.DisposeDestroeydTowers();
        }

        private static void Game_OnUpdate(EventArgs args) {
            if (!Utils.SleepCheck("advancedRangeDisplay"))
                return;

            if (!inGame) {
                Hero = ObjectMgr.LocalHero;

                if (!Game.IsInGame || Hero == null) {
                    Utils.Sleep(1000, "advancedRangeDisplay");
                    return;
                }

                AbilitiesDictionary.Clear();
                ItemsDictionary.Clear();
                CustomRangesDictionary.Clear();
                HeroesLens.Clear();
                ItemsSet.Clear();

                MainMenu.Init();
                Drawings.Init();

                inGame = true;
            }

            if (!Game.IsInGame) {
                inGame = false;
                return;
            }

            if (Game.IsPaused)
                return;

            var allHeroes = ObjectMgr.GetEntities<Hero>().Where(x => !x.IsIllusion).ToList();

            foreach (var hero in allHeroes) {
                var heroName = hero.Name;

                if (!MainMenu.RangesMenu.ContainsKey(hero)) {
                    MainMenu.AddSpells(hero);
                    MainMenu.AddCustomItem(hero, "attribute_bonus", 1300, "Experience range");
                }

                foreach (var spell in
                    hero.Spellbook.Spells.Where(x => Drawings.ParticleDictionary.ContainsKey(heroName + x.Name))) {
                    var key = heroName + spell.Name;
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
                } else {
                    if (HeroesLens.Contains(hero)) {
                        Drawings.UpdateAbilityRanges(hero);
                        HeroesLens.Remove(hero);
                    }
                }

                foreach (var item in hero.Inventory.Items) {
                    var key = heroName + item.Name.GetDefaultName();

                    if (!ItemsSet.Contains(key)) {
                        MainMenu.AddItem(hero, item);
                    } else if (Drawings.ParticleDictionary.ContainsKey(key)) {
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
                    } else {
                        if (MainMenu.Menu.Item(key + "enabled").GetValue<bool>() &&
                            !Drawings.ParticleDictionary.ContainsKey(key) && ability.Level > 0) {
                            Drawings.DrawRange(hero, ability is Item ? ability.Name.GetDefaultName() : ability.Name,
                                true);
                        }
                    }
                }

                if (Game.GameTime % 480 > 240) {
                    if (!nightChanged) {
                        nightChanged = true;
                        Drawings.ChangeTowerRanges(865);
                    }
                } else {
                    if (nightChanged) {
                        nightChanged = false;
                        Drawings.ChangeTowerRanges(950);
                    }
                }

                //    if (RemoveFromMenu.Any()) {
                //        foreach (var key in RemoveFromMenu) {
                //            if (ItemsSet.Contains(key))
                //                ItemsSet.Remove(key);
                //            if (ItemsDictionary.ContainsKey(key))
                //                ItemsDictionary.Remove(key);
                //            if (AbilitiesDictionary.ContainsKey(key))
                //                AbilitiesDictionary.Remove(key);

                //            Menu rangeMenu;
                //            MainMenu.RangesMenu.TryGetValue(hero, out rangeMenu);
                //            if (rangeMenu == null) continue;

                //            rangeMenu.RemoveSubMenu(key);
                //        }
                //        RemoveFromMenu.Clear();
                //}

                foreach (var key in CustomRangesDictionary.Keys.Where(x =>
                    !Drawings.ParticleDictionary.ContainsKey(x) &&
                    x.StartsWith(heroName) &&
                    MainMenu.Menu.Item(x + "enabled").GetValue<bool>())) {
                    Drawings.DrawRange(hero, key.Substring(hero.Name.Length), true, customRange: true);
                }
            }

            Utils.Sleep(1000, "advancedRangeDisplay");
        }

        public static string GetDefaultName(this string name) {
            return char.IsDigit(name[name.Length - 1]) ? name.Remove(name.Length - 2) : name;
        }

        public static Ability FindAbility(this Unit unit, string name) {
            return unit.FindSpell(name) ?? unit.FindItem(name) ?? unit.GetLeveledItem(name.GetDefaultName());
        }
    }
}