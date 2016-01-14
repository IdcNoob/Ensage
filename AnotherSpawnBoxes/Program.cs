using System;
using Ensage;
using Ensage.Common;
using Ensage.Common.Menu;
using SharpDX;

namespace AnotherSpawnBoxes {
    internal static class Program {
        private static bool inGame;
        private static readonly Menu Menu = new Menu("Another Spawn Boxes", "spawnBoxes", true);

        private static void Main() {
            Menu.AddItem(new MenuItem("enabled", "Enabled")).SetValue(true).ValueChanged +=
                (sender, arg) => { ValueChanged(arg.GetNewValue<bool>()); };

            Menu.AddItem(new MenuItem("red", "Red").SetValue(new Slider(0, 0, 255)))
                .SetFontStyle(fontColor: Color.IndianRed)
                .ValueChanged +=
                (sender, arg) => { SpawnBoxes.ChangeColor(arg.GetNewValue<Slider>().Value, Color.Red, false); };

            Menu.AddItem(new MenuItem("green", "Green").SetValue(new Slider(255, 0, 255)))
                .SetFontStyle(fontColor: Color.LightGreen)
                .ValueChanged +=
                (sender, arg) => {
                    SpawnBoxes.ChangeColor(arg.GetNewValue<Slider>().Value, Color.Green, false);
                };

            Menu.AddItem(new MenuItem("blue", "Blue").SetValue(new Slider(0, 0, 255)))
                .SetFontStyle(fontColor: Color.LightBlue)
                .ValueChanged +=
                (sender, arg) => { SpawnBoxes.ChangeColor(arg.GetNewValue<Slider>().Value, Color.Blue, false); };

            Menu.AddItem(new MenuItem("enabledB", "Blocked"))
                .SetValue(true)
                .SetTooltip("Change color when spawn blocked");

            Menu.AddItem(new MenuItem("redB", "Red").SetValue(new Slider(255, 0, 255)))
                .SetFontStyle(fontColor: Color.IndianRed)
                .ValueChanged +=
                (sender, arg) => { SpawnBoxes.ChangeColor(arg.GetNewValue<Slider>().Value, Color.Red, true); };

            Menu.AddItem(new MenuItem("greenB", "Green").SetValue(new Slider(0, 0, 255)))
                .SetFontStyle(fontColor: Color.LightGreen)
                .ValueChanged +=
                (sender, arg) => {
                    SpawnBoxes.ChangeColor(arg.GetNewValue<Slider>().Value, Color.Green, true);
                };

            Menu.AddItem(new MenuItem("blueB", "Blue").SetValue(new Slider(0, 0, 255)))
                .SetFontStyle(fontColor: Color.LightBlue)
                .ValueChanged +=
                (sender, arg) => { SpawnBoxes.ChangeColor(arg.GetNewValue<Slider>().Value, Color.Blue, true); };

            Menu.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args) {
            if (!Utils.SleepCheck("anotherSpawnBoxes"))
                return;

            if (!inGame) {
                if (!Game.IsInGame || ObjectMgr.LocalHero == null) {
                    Utils.Sleep(1000, "anotherSpawnBoxes");
                    return;
                }

                SpawnBoxes.Clear();

                if (Menu.Item("enabled").GetValue<bool>())
                    SpawnBoxes.Draw();

                inGame = true;
            }

            if (!Game.IsInGame) {
                inGame = false;
                return;
            }

            if (Menu.Item("enabledB").GetValue<bool>()) {
                SpawnBoxes.BlockCheck();
                Utils.Sleep(250, "anotherSpawnBoxes");
            } else {
                Utils.Sleep(2000, "anotherSpawnBoxes");
            }
        }

        private static void ValueChanged(bool enabled) {
            if (enabled) SpawnBoxes.Draw();
            else SpawnBoxes.Clear();
        }

        public static float GetMenuValue(string item) {
            return Menu.Item(item).GetValue<Slider>().Value;
        }
    }
}