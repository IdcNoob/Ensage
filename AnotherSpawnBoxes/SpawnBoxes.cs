using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common.Extensions;
using SharpDX;

namespace AnotherSpawnBoxes {
    internal static class SpawnBoxes {
        private static readonly Dictionary<Vector3, IEnumerable<ParticleEffect>> ParticleEffects =
            new Dictionary<Vector3, IEnumerable<ParticleEffect>>();

        private static readonly Dictionary<Vector3, IEnumerable<ParticleEffect>> ParticleEffectsChanged =
            new Dictionary<Vector3, IEnumerable<ParticleEffect>>();

        private static readonly Vector3[][] SpawnBoxLocations = {
            new[] {new Vector3(2690, -4409, 384), new Vector3(3529, -5248, 384)},
            new[] {new Vector3(3873, -3396, 384), new Vector3(4895, -4431, 384)},
            new[] {new Vector3(1088, -3200, 384), new Vector3(2303, -4543, 327)},
            new[] {new Vector3(-3307, 383, 384), new Vector3(-2564, -413, 384)},
            new[] {new Vector3(-1023, -2728, 256), new Vector3(63, -3455, 256)},
            new[] {new Vector3(-2227, -3968, 256), new Vector3(-1463, -4670, 256)},
            new[] {new Vector3(-4383, 1295, 384), new Vector3(-3136, 400, 384)},
            new[] {new Vector3(3434, 942, 384), new Vector3(4719, 7, 384)},
            new[] {new Vector3(-3455, 4927, 384), new Vector3(-2688, 3968, 384)},
            new[] {new Vector3(-4955, 4071, 384), new Vector3(-3712, 3264, 384)},
            new[] {new Vector3(3456, -384, 320), new Vector3(4543, -1151, 256)},
            new[] {new Vector3(-1967, 3135, 384), new Vector3(-960, 2176, 256)},
            new[] {new Vector3(-831, 4095, 384), new Vector3(0, 3200, 384)},
            new[] {new Vector3(448, 3775, 384), new Vector3(1663, 2816, 384)}
        };
        
        public static void Clear() {
            foreach (var effect in ParticleEffects.SelectMany(particleEffect => particleEffect.Value))
                effect.Dispose();

            ParticleEffects.Clear();
            ParticleEffectsChanged.Clear();
        }

        public static void Draw() {
            foreach (var box in SpawnBoxLocations)
                DrawRectangle(new Vector2(box[0].X, box[0].Y), new Vector2(box[1].X, box[1].Y), box.GetBoxCenter());
        }

        public static void ChangeColor(int value, Color color, bool blocked) {
            foreach (
                var effect in
                    (blocked ? ParticleEffectsChanged : ParticleEffects).SelectMany(
                        particleEffect => particleEffect.Value)) {
                effect.SetControlPoint(1, new Vector3(
                    color == Color.Red ? value : Program.GetMenuValue(blocked ? "redB" : "red"),
                    color == Color.Green ? value : Program.GetMenuValue(blocked ? "greenB" : "green"),
                    color == Color.Blue ? value : Program.GetMenuValue(blocked ? "blueB" : "blue")));
            }
        }

        public static void BlockCheck() {
            var units =
                ObjectMgr.GetEntities<Unit>().Where(x => x.IsAlive && x.IsVisible && x.Team != Team.Neutral).ToList();

            foreach (var box in SpawnBoxLocations) {
                var center = box.GetBoxCenter();

                var blocked = false;

                if (units.Select(unit => unit.Position).Any(position => position.IsInCube(box))) {
                    blocked = true;

                    if (ParticleEffectsChanged.ContainsKey(center))
                        continue;

                    IEnumerable<ParticleEffect> effects;
                    ParticleEffects.TryGetValue(center, out effects);

                    if (effects == null)
                        continue;

                    var particleEffects = effects as ParticleEffect[] ?? effects.ToArray();
                    ParticleEffectsChanged.Add(center, particleEffects);

                    foreach (var effect in particleEffects) {
                        effect.SetControlPoint(1, new Vector3(
                            Program.GetMenuValue("redB"),
                            Program.GetMenuValue("greenB"),
                            Program.GetMenuValue("blueB")));
                    }
                }

                if (blocked) continue;

                foreach (
                    var effect in
                        ParticleEffectsChanged.Where(x => x.Key == center)
                            .SelectMany(particleEffect => particleEffect.Value)) {
                    effect.SetControlPoint(1, new Vector3(
                        Program.GetMenuValue("red"),
                        Program.GetMenuValue("green"),
                        Program.GetMenuValue("blue")));
                }

                ParticleEffectsChanged.Remove(center);
            }
        }

        private static void DrawRectangle(Vector2 position1, Vector2 position2, Vector3 center) {
            const float bonus = 1.115f;

            var effects = new ParticleEffect[4];

            effects[0] = DrawLine(new Vector2((position1.X + position2.X) / 2, position1.Y),
                (position2.X - position1.X) / 2 * bonus, 1);
            effects[1] = DrawLine(new Vector2((position1.X + position2.X) / 2, position2.Y),
                (position2.X - position1.X) / 2 * bonus, 1);
            effects[2] = DrawLine(new Vector2(position1.X, (position1.Y + position2.Y) / 2),
                (position1.Y - position2.Y) / 2 * bonus, vertical: 1);
            effects[3] = DrawLine(new Vector2(position2.X, (position1.Y + position2.Y) / 2),
                (position1.Y - position2.Y) / 2 * bonus, vertical: 1);

            ParticleEffects.Add(center, effects);
        }

        private static ParticleEffect DrawLine(Vector2 position, float size, int horizontal = 0, int vertical = 0) {
            var position3 = position.ToVector3();
            var effect = new ParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf", position3);

            effect.SetControlPoint(1, new Vector3(
                Program.GetMenuValue("red"),
                Program.GetMenuValue("green"),
                Program.GetMenuValue("blue")));

            effect.SetControlPoint(2, new Vector3(size, 255, 0));

            effect.SetControlPointOrientation(4, new Vector3(horizontal, 0, 0), new Vector3(vertical, 0, 0),
                new Vector3(0, 0, 0));

            return effect;
        }

        private static bool IsInCube(this Vector3 point, IList<Vector3> box) {
            return point.X >= Math.Min(box[0].X, box[1].X) && point.X <= Math.Max(box[0].X, box[1].X)
                   && point.Y >= Math.Min(box[0].Y, box[1].Y) && point.Y <= Math.Max(box[0].Y, box[1].Y)
                   && point.Z >= Math.Min(box[0].Z, box[1].Z) && point.Z <= Math.Max(box[0].Z, box[1].Z);
        }

        private static Vector3 GetBoxCenter(this IList<Vector3> box) {
            return new Vector3((box[1].X + box[0].X) / 2, (box[0].Y + box[1].Y) / 2, 0);
        }
    }
}
