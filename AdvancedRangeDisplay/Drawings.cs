using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using Ensage.Common.Objects;
using SharpDX;

namespace AdvancedRangeDisplay {
    internal static class Drawings {
        private static ParticleEffect particleEffect;

        public static readonly Dictionary<string, ParticleEffect> ParticleDictionary =
            new Dictionary<string, ParticleEffect>();

        public static void Init() {
            Clear();
        }

        public static void Clear() {
            foreach (var effect in ParticleDictionary.Values)
                effect.Dispose();

            ParticleDictionary.Clear();
        }

        public static void ChangeColor(string key, int value, Color color) {
            ParticleDictionary.TryGetValue(key, out particleEffect);

            if (particleEffect == null)
                return;

            particleEffect.SetControlPoint(1, new Vector3(
                color == Color.Red ? value : RangesMenu.Menu.Item(key + "red").GetValue<Slider>().Value,
                color == Color.Green ? value : RangesMenu.Menu.Item(key + "green").GetValue<Slider>().Value,
                color == Color.Blue ? value : RangesMenu.Menu.Item(key + "blue").GetValue<Slider>().Value));
        }

        public static void DisposeRange(string key) {
            ParticleDictionary.TryGetValue(key, out particleEffect);

            if (particleEffect == null)
                return;

            particleEffect.Dispose();
            ParticleDictionary.Remove(key);
        }

        public static void ChangeRange(string key, Hero hero, float value = 0, bool customRange = false) {
            ParticleDictionary.TryGetValue(key, out particleEffect);

            if (particleEffect == null)
                return;

            var ability = hero.FindAbility(key.Substring(hero.StoredName().Length));

            particleEffect.SetControlPoint(2,
                new Vector3((ability.GetRealCastRange(key, customRange) + value) * -1, 255, 0));
            particleEffect.Restart();
        }

        public static void UpdateAbilityRanges(Hero hero) {
            foreach (var key in
                Main.AbilitiesDictionary.Keys.Where(x => x.StartsWith(hero.StoredName()))
                    .Concat(Main.ItemsDictionary.Keys.Where(x => x.StartsWith(hero.StoredName())))) {
                ChangeRange(key, hero);
            }
        }

        public static void DrawRange(Hero hero, string name, bool draw, int bonus = 0, bool customRange = false) {
            var ability = hero.FindAbility(name);

            if (ability == null && !customRange)
                return;

            var key = hero.StoredName() + name;

            if (draw) {
                particleEffect = hero.AddParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf");
                ParticleDictionary.Add(key, particleEffect);

                var drawRange = ability.GetRealCastRange(key, customRange) +
                                (bonus != 0 ? bonus : RangesMenu.Menu.Item(key + "bonus").GetValue<Slider>().Value);

                particleEffect.SetControlPoint(1, new Vector3(
                    RangesMenu.Menu.Item(key + "red").GetValue<Slider>().Value,
                    RangesMenu.Menu.Item(key + "green").GetValue<Slider>().Value,
                    RangesMenu.Menu.Item(key + "blue").GetValue<Slider>().Value));

                particleEffect.SetControlPoint(2, new Vector3(drawRange * -1, 255, 0));
            }
            else {
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

            if (!ability.IsAbilityBehavior(AbilityBehavior.NoTarget))
                castRange += (float) Math.Max(castRange / 4.5, 80);
            else
                castRange += (float) Math.Max(castRange / 6.7, 40);

            return castRange;
        }
    }
}