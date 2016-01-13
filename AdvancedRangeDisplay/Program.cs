using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using SharpDX;

namespace AdvancedRangeDisplay {
    internal class Program {
        private static bool inGame;
        private static Hero Hero;

        private static readonly Dictionary<string, ParticleEffect> ParticleDictionary =
            new Dictionary<string, ParticleEffect>();
        private static readonly Dictionary<uint, float> AbilityRangeDictionary =
            new Dictionary<uint, float>();
        private static readonly Dictionary<string, bool> HeroesDictionary = new Dictionary<string, bool>();

        private static readonly Menu Menu = new Menu("Advanced Ranges", "advancedRanges", true);
        private static readonly Menu HeroesMenu = new Menu("Heroes", "rangeHeroes");
        private static readonly Menu TowersMenu = new Menu("Towers", "rangeTowers");

        private static void Main() {
            Menu.AddSubMenu(HeroesMenu);

            var allyTowers = new Menu("Ally", "ally");
            var enemyTowers = new Menu("Enemy", "enemy");

            allyTowers.AddItem(new MenuItem("allyTowers", "Enabled")).SetValue(true).ValueChanged +=
                (sender, arg) => { ShowTowerRange(Hero.Team, arg.GetNewValue<bool>()); };

            allyTowers.AddItem(new MenuItem("allyTowersR", "Red").SetValue(new Slider(0, 0, 255)))
                .SetFontStyle(fontColor: Color.IndianRed)
                .ValueChanged +=
                (sender, arg) => { ChangeColor(Hero.Team, arg.GetNewValue<Slider>().Value, Color.Red); };

            allyTowers.AddItem(new MenuItem("allyTowersG", "Green").SetValue(new Slider(255, 0, 255)))
                .SetFontStyle(fontColor: Color.LightGreen)
                .ValueChanged +=
                (sender, arg) => { ChangeColor(Hero.Team, arg.GetNewValue<Slider>().Value, Color.Green); };

            allyTowers.AddItem(new MenuItem("allyTowersB", "Blue").SetValue(new Slider(0, 0, 255)))
                .SetFontStyle(fontColor: Color.LightBlue)
                .ValueChanged +=
                (sender, arg) => { ChangeColor(Hero.Team, arg.GetNewValue<Slider>().Value, Color.Blue); };

            enemyTowers.AddItem(new MenuItem("enemyTowers", "Enabled")).SetValue(true).ValueChanged +=
                (sender, arg) => { ShowTowerRange(Hero.GetEnemyTeam(), arg.GetNewValue<bool>()); };

            enemyTowers.AddItem(new MenuItem("enemyTowersR", "Red").SetValue(new Slider(255, 0, 255)))
                .SetFontStyle(fontColor: Color.IndianRed)
                .ValueChanged +=
                (sender, arg) => { ChangeColor(Hero.GetEnemyTeam(), arg.GetNewValue<Slider>().Value, Color.Red); };

            enemyTowers.AddItem(new MenuItem("enemyTowersG", "Green").SetValue(new Slider(0, 0, 255)))
                .SetFontStyle(fontColor: Color.LightGreen)
                .ValueChanged +=
                (sender, arg) => { ChangeColor(Hero.GetEnemyTeam(), arg.GetNewValue<Slider>().Value, Color.Green); };

            enemyTowers.AddItem(new MenuItem("enemyTowersB", "Blue").SetValue(new Slider(0, 0, 255)))
                .SetFontStyle(fontColor: Color.LightBlue)
                .ValueChanged +=
                (sender, arg) => { ChangeColor(Hero.GetEnemyTeam(), arg.GetNewValue<Slider>().Value, Color.Blue); };

            TowersMenu.AddSubMenu(allyTowers);
            TowersMenu.AddSubMenu(enemyTowers);

            Menu.AddSubMenu(TowersMenu);

            Menu.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
            Game.OnFireEvent += Game_OnFireEvent;
        }

        private static void Game_OnFireEvent(FireEventEventArgs args) {
            if (args.GameEvent.Name == "dota_tower_kill")
                DisposeDestroeydTowers();
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

                foreach (var particleEffect in ParticleDictionary)
                    particleEffect.Value.Dispose();

                ParticleDictionary.Clear();
                HeroesDictionary.Clear();

                ShowTowerRange(Hero.Team, TowersMenu.Item("allyTowers").GetValue<bool>());
                ShowTowerRange(Hero.GetEnemyTeam(), TowersMenu.Item("enemyTowers").GetValue<bool>());

                inGame = true;
            }

            if (!Game.IsInGame) {
                inGame = false;
                return;
            }

            var allHeroes = ObjectMgr.GetEntities<Hero>().Where(x => !x.IsIllusion).ToList();

            foreach (var hero in allHeroes) {
                foreach (
                    var spell in
                        hero.Spellbook.Spells.Where(
                            x =>
                                x.ClassID != ClassID.CDOTA_Ability_AttributeBonus &&
                                AbilityRangeDictionary.ContainsKey(x.Handle))) {
                    var castRange = 0f;
                    AbilityRangeDictionary.TryGetValue(spell.Handle, out castRange);

                    if (castRange <= 0)
                        continue;

                    var newCastRange = spell.GetCastRange();

                    if (!newCastRange.Equals(castRange)) {
                        AbilityRangeDictionary.Remove(spell.Handle);
                        ChangeRange(hero, spell);
                    }
                }
            }

            foreach (var hero in allHeroes.Where(enemy => !HeroesDictionary.ContainsKey(enemy.Name))) {
                HeroesDictionary.Add(hero.Name, true);

                var add = new Menu("", hero.Name + "range", false, hero.Name);

                foreach (
                    var spell in
                        hero.Spellbook.Spells.Where(
                            x =>
                                !x.IsAbilityBehavior(AbilityBehavior.Passive) ||
                                x.ClassID == ClassID.CDOTA_Ability_AttributeBonus)) {
                    var addSpell = new Menu("", hero.Name + spell.Name, false, spell.Name);

                    addSpell.AddItem(new MenuItem(hero.Name + spell.Name + "spell", "Enabled"))
                        .SetValue(false)
                        .DontSave()
                        .ValueChanged += (sender, arg) => { ShowRange(hero, spell, arg.GetNewValue<bool>()); };

                    addSpell.AddItem(
                        new MenuItem(hero.Name + spell.Name + "bonus", "Bonus range").SetValue(new Slider(0, -300, 300)))
                        .DontSave()
                        .ValueChanged += (sender, arg) => { ChangeRange(hero, spell, arg.GetNewValue<Slider>().Value); };

                    addSpell.AddItem(new MenuItem(hero.Name + spell.Name + "r", "Red").SetValue(new Slider(255, 0, 255)))
                        .SetFontStyle(fontColor: Color.IndianRed)
                        .DontSave()
                        .ValueChanged +=
                        (sender, arg) => {
                            ChangeColor(hero.Name + spell.Name, arg.GetNewValue<Slider>().Value, Color.Red);
                        };

                    addSpell.AddItem(new MenuItem(hero.Name + spell.Name + "g", "Green").SetValue(new Slider(0, 0, 255)))
                        .SetFontStyle(fontColor: Color.LightGreen)
                        .DontSave()
                        .ValueChanged +=
                        (sender, arg) => {
                            ChangeColor(hero.Name + spell.Name, arg.GetNewValue<Slider>().Value, Color.Green);
                        };

                    addSpell.AddItem(new MenuItem(hero.Name + spell.Name + "b", "Blue").SetValue(new Slider(0, 0, 255)))
                        .SetFontStyle(fontColor: Color.LightBlue)
                        .DontSave()
                        .ValueChanged +=
                        (sender, arg) => {
                            ChangeColor(hero.Name + spell.Name, arg.GetNewValue<Slider>().Value, Color.Blue);
                        };

                    add.AddSubMenu(addSpell);
                }

                HeroesMenu.AddSubMenu(add);
            }

            Utils.Sleep(1000, "advancedRangeDisplay");
        }

        private static void ChangeColor(string key, int value, Color color) {
            ParticleEffect particleEffect;
            ParticleDictionary.TryGetValue(key, out particleEffect);

            if (particleEffect == null)
                return;

            particleEffect.SetControlPoint(1, new Vector3(
                color == Color.Red ? value : Menu.Item(key + "r").GetValue<Slider>().Value,
                color == Color.Green ? value : Menu.Item(key + "g").GetValue<Slider>().Value,
                color == Color.Blue ? value : Menu.Item(key + "b").GetValue<Slider>().Value));
        }

        private static void ChangeColor(Team team, int value, Color color) {
            var towers =
                ObjectMgr.GetEntities<Unit>()
                    .Where(x => x.ClassID == ClassID.CDOTA_BaseNPC_Tower && x.Team == team && x.IsAlive)
                    .ToList();

            foreach (var tower in towers) {
                ParticleEffect particleEffect;
                ParticleDictionary.TryGetValue(tower.Handle.ToString(), out particleEffect);

                if (particleEffect == null)
                    continue;

                var key = Hero.Team == team ? "allyTowers" : "enemyTowers";

                particleEffect.SetControlPoint(1, new Vector3(
                    color == Color.Red ? value : Menu.Item(key + "R").GetValue<Slider>().Value,
                    color == Color.Green ? value : Menu.Item(key + "G").GetValue<Slider>().Value,
                    color == Color.Blue ? value : Menu.Item(key + "B").GetValue<Slider>().Value));
            }
        }

        private static void ChangeRange(Hero hero, Ability ability, int value = 0) {
            ParticleEffect particleEffect;
            ParticleDictionary.TryGetValue(hero.Name + ability.Name, out particleEffect);

            if (particleEffect == null)
                return;

            particleEffect.Dispose();
            ParticleDictionary.Remove(hero.Name + ability.Name);

            ShowRange(hero, ability, true, value);
        }

        private static void DisposeDestroeydTowers() {
            var towers = ObjectMgr.GetEntities<Unit>().Where(x => x.ClassID == ClassID.CDOTA_BaseNPC_Tower).ToList();

            foreach (var tower in towers) {
                ParticleEffect particleEffect;
                ParticleDictionary.TryGetValue(tower.Handle.ToString(), out particleEffect);

                if (tower.IsAlive || particleEffect == null) continue;

                particleEffect.Dispose();
                ParticleDictionary.Remove(tower.Handle.ToString());
            }
        }

        private static void ShowTowerRange(Team team, bool show) {
            var towers =
                ObjectMgr.GetEntities<Unit>()
                    .Where(x => x.ClassID == ClassID.CDOTA_BaseNPC_Tower && x.Team == team && x.IsAlive)
                    .ToList();

            foreach (var tower in towers) {
                ParticleEffect particleEffect;
                ParticleDictionary.TryGetValue(tower.Handle.ToString(), out particleEffect);

                if (!show) {
                    if (particleEffect == null) continue;
                    particleEffect.Dispose();
                    ParticleDictionary.Remove(tower.Handle.ToString());
                } else {
                    particleEffect = tower.AddParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf");
                    ParticleDictionary.Add(tower.Handle.ToString(), particleEffect);

                    var key = Hero.Team == team ? "allyTowers" : "enemyTowers";

                    particleEffect.SetControlPoint(1, new Vector3(
                        Menu.Item(key + "R").GetValue<Slider>().Value,
                        Menu.Item(key + "G").GetValue<Slider>().Value,
                        Menu.Item(key + "B").GetValue<Slider>().Value));
                    particleEffect.SetControlPoint(2, new Vector3(950, 255, 0));
                }
            }
        }

        private static void ShowRange(Hero hero, Ability ability, bool visible, int bonus = 0) {
            var key = hero.Name + ability.Name;

            ParticleEffect particleEffect;
            ParticleDictionary.TryGetValue(key, out particleEffect);

            if (visible) {
                particleEffect = hero.AddParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf");
                ParticleDictionary.Add(key, particleEffect);

                particleEffect.SetControlPoint(1, new Vector3(
                    Menu.Item(key + "r").GetValue<Slider>().Value,
                    Menu.Item(key + "g").GetValue<Slider>().Value,
                    Menu.Item(key + "b").GetValue<Slider>().Value));

                var castRange = ability.GetCastRange();

                if (ability.ClassID == ClassID.CDOTA_Ability_AttributeBonus)
                    castRange = 1175;

                if (AbilityRangeDictionary.ContainsKey(ability.Handle))
                    AbilityRangeDictionary.Remove(ability.Handle);

                AbilityRangeDictionary.Add(ability.Handle, castRange);

                particleEffect.SetControlPoint(2,
                    new Vector3(
                        (float) (castRange * 1.15) +
                        (bonus != 0 ? bonus : Menu.Item(key + "bonus").GetValue<Slider>().Value),
                        255, 0));
            } else {
                if (particleEffect == null)
                    return;
                particleEffect.Dispose();
                ParticleDictionary.Remove(key);
            }
        }
    }
}