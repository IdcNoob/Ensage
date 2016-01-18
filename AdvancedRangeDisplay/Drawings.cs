using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using SharpDX;

namespace AdvancedRangeDisplay {
    internal static class Drawings {
        public static readonly Vector2[][] TowerLocations = {
            new[] {
                new Vector2(-6096, 1840), new Vector2(-1504, -1376), new Vector2(4992, -6080),
                new Vector2(-4672, 6016), new Vector2(1088, 320), new Vector2(6272, -1664)
            },
            new[] {
                new Vector2(-560, -6096), new Vector2(-3512, -2776), new Vector2(-6080, -832),
                new Vector2(6336, 384), new Vector2(2560, 2112), new Vector2(0, 6016)
            },
            new[] {
                new Vector2(-3873, -6112), new Vector2(-4544, -4096), new Vector2(-6624, -3328),
                new Vector2(6276, 2984), new Vector2(4288, 3712), new Vector2(3504, 5776)
            },
            new[] {
                new Vector2(-5392, -5168), new Vector2(-5680, -4880),
                new Vector2(5280, 4432), new Vector2(4960, 4784)
            }
        };

        private static ParticleEffect particleEffect;

        public static readonly Dictionary<string, ParticleEffect> ParticleDictionary =
            new Dictionary<string, ParticleEffect>();

        public static void Init() {
            Clear();

            var team = Main.Hero.Team;
            var eteam = Main.Hero.GetEnemyTeam();

            for (var i = 1; i <= 4; i++) {
                if (MainMenu.TowersMenu.Item("allyTowersT" + i).GetValue<bool>())
                    DrawTowerRange(team, true, TowerLocations[i - 1], "T" + i);
                if (MainMenu.TowersMenu.Item("enemyTowersT" + i).GetValue<bool>())
                    DrawTowerRange(eteam, true, TowerLocations[i - 1], "T" + i);
            }
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

        public static void ChangeColor(Team team, int value, Color color, Vector2[] towerLocations, string towers) {
            foreach (
                var tower in
                    from tower in
                        ObjectMgr.GetEntities<Unit>()
                            .Where(x => x.ClassID == ClassID.CDOTA_BaseNPC_Tower && x.Team == team && x.IsAlive)
                    from towerLocation in towerLocations
                    where tower.Distance2D(towerLocation.ToVector3()) < 300
                    select tower) {
                ParticleDictionary.TryGetValue(tower.Handle.ToString(), out particleEffect);

                if (particleEffect == null)
                    continue;

                var key = Main.Hero.Team == team ? "allyTowers" : "enemyTowers";

                particleEffect.SetControlPoint(1, new Vector3(
                    color == Color.Red ? value : MainMenu.Menu.Item(key + "R" + towers).GetValue<Slider>().Value,
                    color == Color.Green ? value : MainMenu.Menu.Item(key + "G" + towers).GetValue<Slider>().Value,
                    color == Color.Blue ? value : MainMenu.Menu.Item(key + "B" + towers).GetValue<Slider>().Value));
            }
        }

        public static void ChangeRange(string key, Hero hero, float value = 0, bool customRange = false) {
            ParticleDictionary.TryGetValue(key, out particleEffect);

            if (particleEffect == null)
                return;

            var ability = hero.FindAbility(key.Substring(hero.Name.Length));

            particleEffect.SetControlPoint(2,
                new Vector3((ability.GetRealCastRange(key, customRange) + value) * -1, 255, 0));
            particleEffect.Restart();
        }

        public static void ChangeTowerRanges(int range) {
            if (!MainMenu.TowersMenu.Item("towersNightRange").GetValue<bool>())
                return;

            var towers = ObjectMgr.GetEntities<Unit>().Where(x => x.ClassID == ClassID.CDOTA_BaseNPC_Tower && x.IsAlive);

            foreach (var tower in towers) {
                ParticleDictionary.TryGetValue(tower.Handle.ToString(), out particleEffect);

                if (particleEffect == null)
                    continue;

                particleEffect.SetControlPoint(2, new Vector3(range * -1, 255, 0));
                particleEffect.Restart();
            }
        }

        public static void DrawTowerRange(Team team, bool enabled, Vector2[] towerLocations, string towers) {
            foreach (
                var tower in
                    from tower in
                        ObjectMgr.GetEntities<Unit>()
                            .Where(x => x.ClassID == ClassID.CDOTA_BaseNPC_Tower && x.Team == team && x.IsAlive)
                    from towerLocation in towerLocations
                    where tower.Distance2D(towerLocation.ToVector3()) < 300
                    select tower) {
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
                        MainMenu.Menu.Item(key + "R" + towers).GetValue<Slider>().Value,
                        MainMenu.Menu.Item(key + "G" + towers).GetValue<Slider>().Value,
                        MainMenu.Menu.Item(key + "B" + towers).GetValue<Slider>().Value));

                    particleEffect.SetControlPoint(2, new Vector3(950 * -1, 255, 0));
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

                particleEffect.SetControlPoint(2,
                    new Vector3(
                        (ability.GetRealCastRange(key, customRange) +
                         (bonus != 0 ? bonus : MainMenu.Menu.Item(key + "bonus").GetValue<Slider>().Value)) * -1,
                        255, 0));
            } else {
                DisposeRange(key);
            }
        }

        private static float GetRealCastRange(this Ability ability, string key, bool customRange) {
            var castRange = 0f;

            if (customRange) {
                Main.CustomRangesDictionary.TryGetValue(key, out castRange);
                return castRange;
            }

            castRange = ability.GetCastRange();
            if (castRange <= 0 || castRange >= 5000) castRange = ability.GetRadius();

            if (!ability.IsAbilityBehavior(AbilityBehavior.NoTarget)) {
                castRange += (float) Math.Max(castRange / 4.5, 80);
            } else {
                castRange += (float) Math.Max(castRange / 6.7, 40);
            }

            return castRange;
        }
    }
}