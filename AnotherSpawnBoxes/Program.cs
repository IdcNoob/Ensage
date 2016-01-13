using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Menu;
using SharpDX;

namespace AnotherSpawnBoxes {
    internal class Program {
        private static Hero hero;
        private static bool inGame;

        private static readonly List<ParticleEffect> ParticleEffects = new List<ParticleEffect>();

        private static readonly Vector4[] Spots = {
            new Vector4(2690, -4409, 3529, -5248),
            new Vector4(3936, -3277, 5007, -4431),
            new Vector4(1088, -3200, 2303, -4543),
            new Vector4(-3307, 383, -2564, -413),
            new Vector4(-1023, -2728, 63, -3455),
            new Vector4(-2227, -3968, -1463, -4648),
            new Vector4(-4383, 1295, -3136, 400),
            new Vector4(3344, 942, 4719, 7),
            new Vector4(-3455, 4927, -2688, 3968),
            new Vector4(-4955, 4071, -3712, 3264),
            new Vector4(3456, -384, 4543, -1151),
            new Vector4(-1967, 3135, -960, 2176),
            new Vector4(-831, 4095, 0, 3200),
            new Vector4(448, 3775, 1663, 2816)
        };

        private static readonly Menu Menu = new Menu("Another Spawn Boxes", "spawnBoxes", true);

        private static void Main() {
            Menu.AddItem(new MenuItem("enabled", "Enabled")).SetValue(true).ValueChanged +=
                (sender, arg) => { ValueChanged(arg.GetNewValue<bool>()); };

            Menu.AddItem(new MenuItem("red", "Red").SetValue(new Slider(0, 0, 255)))
                .SetFontStyle(fontColor: Color.IndianRed)
                .ValueChanged +=
                (sender, arg) => { ChangeColor(arg.GetNewValue<Slider>().Value, Color.Red); };

            Menu.AddItem(new MenuItem("green", "Green").SetValue(new Slider(255, 0, 255)))
                .SetFontStyle(fontColor: Color.LightGreen)
                .ValueChanged +=
                (sender, arg) => { ChangeColor(arg.GetNewValue<Slider>().Value, Color.Green); };

            Menu.AddItem(new MenuItem("blue", "Blue").SetValue(new Slider(0, 0, 255)))
                .SetFontStyle(fontColor: Color.LightBlue)
                .ValueChanged +=
                (sender, arg) => { ChangeColor(arg.GetNewValue<Slider>().Value, Color.Blue); };

            Menu.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args) {
            if (!Utils.SleepCheck("anotherSpawnBoxes"))
                return;

            if (!inGame) {
                hero = ObjectMgr.LocalHero;

                if (!Game.IsInGame || hero == null) {
                    Utils.Sleep(1000, "anotherSpawnBoxes");
                    return;
                }

                if (ParticleEffects.Any()) {
                    foreach (var particleEffect in ParticleEffects)
                        particleEffect.Dispose();

                    ParticleEffects.Clear();
                }

                if (Menu.Item("enabled").GetValue<bool>())
                    DrawRectangles();

                inGame = true;
            }

            if (!Game.IsInGame) {
                inGame = false;
                return;
            }

            Utils.Sleep(10000, "anotherSpawnBoxes");
        }

        private static void ChangeColor(int value, Color color) {
            foreach (var effect in ParticleEffects) {
                effect.SetControlPoint(1, new Vector3(
                    color == Color.Red ? value : Menu.Item("red").GetValue<Slider>().Value,
                    color == Color.Green ? value : Menu.Item("green").GetValue<Slider>().Value,
                    color == Color.Blue ? value : Menu.Item("blue").GetValue<Slider>().Value));
            }
        }

        private static void ValueChanged(bool enabled) {
            if (enabled) DrawRectangles();
            else DeleteRectangles();
        }

        private static void DeleteRectangles() {
            foreach (var particleEffect in ParticleEffects)
                particleEffect.Dispose();
        }

        private static void DrawRectangles() {
            foreach (var spot in Spots)
                CreateRectangle(new Vector3(spot.X, spot.Y, 0), new Vector3(spot.Z, spot.W, 0));
        }

        private static void CreateRectangle(Vector3 position1, Vector3 position2) {
            const double bonus = 1.115;

            DrawLine(new Vector3((position1.X + position2.X) / 2, position1.Y, 0),
                (float) ((position2.X - position1.X) / 2 * bonus), 1, 0);
            DrawLine(new Vector3((position1.X + position2.X) / 2, position2.Y, 0),
                (float) ((position2.X - position1.X) / 2 * bonus), 1, 0);
            DrawLine(new Vector3(position1.X, (position1.Y + position2.Y) / 2, 0),
                (float) ((position1.Y - position2.Y) / 2 * bonus), 0, 1);
            DrawLine(new Vector3(position2.X, (position1.Y + position2.Y) / 2, 0),
                (float) ((position1.Y - position2.Y) / 2 * bonus), 0, 1);
        }

        private static void DrawLine(Vector3 position, float size, int directionf, int directionu) {
            var effect = new ParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf", position);
            ParticleEffects.Add(effect);

            effect.SetControlPoint(1, new Vector3(
                Menu.Item("red").GetValue<Slider>().Value,
                Menu.Item("green").GetValue<Slider>().Value,
                Menu.Item("blue").GetValue<Slider>().Value));

            effect.SetControlPoint(2, new Vector3(size, 255, 0));

            effect.SetControlPointOrientation(4, new Vector3(directionf, 0, 0), new Vector3(directionu, 0, 0),
                new Vector3(0, 0, 0));
        }
    }
}