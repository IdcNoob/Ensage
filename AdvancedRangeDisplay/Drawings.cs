using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using SharpDX;

namespace AdvancedRangeDisplay {
    internal static class Drawings {
        private static ParticleEffect particleEffect;

        public static readonly Dictionary<string, ParticleEffect> ParticleDictionary =
            new Dictionary<string, ParticleEffect>();

        public static void Init() {
            Clear();
            DrawTowerRange(Main.Hero.Team, MainMenu.TowersMenu.Item("allyTowers").GetValue<bool>());
            DrawTowerRange(Main.Hero.GetEnemyTeam(), MainMenu.TowersMenu.Item("enemyTowers").GetValue<bool>());
        }

        public static void Clear() {
            foreach (var effect in ParticleDictionary.Values)
                effect.Dispose();

            ParticleDictionary.Clear();
        }

        public static void DisposeDestroeydTowers() {
            var towers = ObjectMgr.GetEntities<Unit>().Where(x => x.ClassID == ClassID.CDOTA_BaseNPC_Tower).ToList();

            foreach (var tower in towers) {
                ParticleDictionary.TryGetValue(tower.Handle.ToString(), out particleEffect);

                if (tower.IsAlive || particleEffect == null) continue;

                particleEffect.Dispose();
                ParticleDictionary.Remove(tower.Handle.ToString());
            }
        }

        public static void ChangeColor(string key, int value, Color color) {
            ParticleDictionary.TryGetValue(key, out particleEffect);

            if (particleEffect == null)
                return;

            particleEffect.SetControlPoint(1, new Vector3(
                color == Color.Red ? value : MainMenu.Menu.Item(key + "red").GetValue<Slider>().Value,
                color == Color.Green ? value : MainMenu.Menu.Item(key + "green").GetValue<Slider>().Value,
                color == Color.Blue ? value : MainMenu.Menu.Item(key + "blue").GetValue<Slider>().Value));
        }

        public static void DisposeRange(string key) {
            ParticleDictionary.TryGetValue(key, out particleEffect);

            if (particleEffect == null)
                return;

            particleEffect.Dispose();
            ParticleDictionary.Remove(key);
        }

        public static void ChangeColor(Team team, int value, Color color) {
            var towers =
                ObjectMgr.GetEntities<Unit>()
                    .Where(x => x.ClassID == ClassID.CDOTA_BaseNPC_Tower && x.Team == team && x.IsAlive)
                    .ToList();

            foreach (var tower in towers) {
                ParticleDictionary.TryGetValue(tower.Handle.ToString(), out particleEffect);

                if (particleEffect == null)
                    continue;

                var key = Main.Hero.Team == team ? "allyTowers" : "enemyTowers";

                particleEffect.SetControlPoint(1, new Vector3(
                    color == Color.Red ? value : MainMenu.Menu.Item(key + "R").GetValue<Slider>().Value,
                    color == Color.Green ? value : MainMenu.Menu.Item(key + "G").GetValue<Slider>().Value,
                    color == Color.Blue ? value : MainMenu.Menu.Item(key + "B").GetValue<Slider>().Value));
            }
        }

        public static void ChangeRange(string key, Hero hero, float value = 0, bool customRange = false) {
            ParticleDictionary.TryGetValue(key, out particleEffect);

            if (particleEffect == null)
                return;

            var castRange = 0f;

            if (customRange)
                Main.CustomRangesDictionary.TryGetValue(key, out castRange);
            else
                castRange = hero.FindAbility(key.Substring(hero.Name.Length)).GetCastRange();

            particleEffect.SetControlPoint(2, new Vector3((float) (castRange * 1.1 + value) * -1, 255, 0));
            particleEffect.Restart();
        }

        public static void DrawTowerRange(Team team, bool enabled) {
            var towers =
                ObjectMgr.GetEntities<Unit>()
                    .Where(x => x.ClassID == ClassID.CDOTA_BaseNPC_Tower && x.Team == team && x.IsAlive)
                    .ToList();

            foreach (var tower in towers) {
                if (!enabled) {
                    ParticleDictionary.TryGetValue(tower.Handle.ToString(), out particleEffect);
                    if (particleEffect == null) continue;
                    particleEffect.Dispose();
                    ParticleDictionary.Remove(tower.Handle.ToString());
                } else {
                    particleEffect = tower.AddParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf");
                    ParticleDictionary.Add(tower.Handle.ToString(), particleEffect);

                    var key = Main.Hero.Team == team ? "allyTowers" : "enemyTowers";

                    particleEffect.SetControlPoint(1, new Vector3(
                        MainMenu.Menu.Item(key + "R").GetValue<Slider>().Value,
                        MainMenu.Menu.Item(key + "G").GetValue<Slider>().Value,
                        MainMenu.Menu.Item(key + "B").GetValue<Slider>().Value));

                    particleEffect.SetControlPoint(2, new Vector3(950, 255, 0));
                }
            }
        }

        public static void UpdateAbilityRanges(Hero hero) {
            foreach (var key in
                Main.AbilitiesDictionary.Keys.Where(x => x.StartsWith(hero.Name))
                    .Concat(Main.ItemsDictionary.Keys.Where(x => x.StartsWith(hero.Name)))) {
                ChangeRange(key, hero);
            }
        }

        public static void DrawRange(Hero hero, string name, bool visible, int bonus = 0, bool customRange = false) {
            var ability = hero.FindAbility(name);

            if (ability == null && !customRange)
                return;

            var key = hero.Name + name;

            if (visible) {
                particleEffect = hero.AddParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf");
                ParticleDictionary.Add(key, particleEffect);

                particleEffect.SetControlPoint(1, new Vector3(
                    MainMenu.Menu.Item(key + "red").GetValue<Slider>().Value,
                    MainMenu.Menu.Item(key + "green").GetValue<Slider>().Value,
                    MainMenu.Menu.Item(key + "blue").GetValue<Slider>().Value));

                var castRange = 0f;

                if (customRange)
                    Main.CustomRangesDictionary.TryGetValue(key, out castRange);
                else
                    castRange = ability.GetCastRange();

                if (castRange <= 0)
                    return;

                particleEffect.SetControlPoint(2,
                    new Vector3((float)(castRange * 1.1 +
                        (bonus != 0 ? bonus : MainMenu.Menu.Item(key + "bonus").GetValue<Slider>().Value)) * -1, 255, 0));
            } else {
                DisposeRange(key);
            }
        }
    }
}