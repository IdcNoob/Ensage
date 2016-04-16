using System;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Menu;
using SharpDX;

namespace VisionControl {
    internal static class Program {
        public static bool inGame;
        public static Hero Hero;

        private static readonly Menu Menu = new Menu("Vision Control", "visionControl", true);

        public static bool IsEnabled {
            get { return Menu.Item("enabled").GetValue<bool>(); }
        }

        public static bool SmartHide {
            get { return Menu.Item("hide").GetValue<bool>(); }
        }

        private static void Main() {
            Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
            Menu.AddItem(new MenuItem("hide", "Smart icon hide").SetValue(true)
                .SetTooltip("Ward icon will be hidden if enemy ward is visible"));
            Menu.AddItem(new MenuItem("size", "Icon size").SetValue(new Slider(4, 2, 8)));

            Menu.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Game_OnUpdate(EventArgs args) {
            if (!Utils.SleepCheck("VisionControlDelay"))
                return;

            if (!inGame) {
                Hero = ObjectManager.LocalHero;

                if (!Game.IsInGame || Hero == null) {
                    Utils.Sleep(1000, "VisionControlDelay");
                    return;
                }

                MapWard.Clear();
                HeroWard.Clear();

                inGame = true;
            }

            if (!Game.IsInGame) {
                inGame = false;
                return;
            }

            HeroWard.Update();
            MapWard.Update();

            Utils.Sleep(333, "VisionControlDelay");
        }

        private static void Drawing_OnDraw(EventArgs args) {
            if (!inGame || !IsEnabled) return;

            foreach (var ward in MapWard.MapWards.Where(x => x.Show)) {
                Vector2 screenPos;
                Drawing.WorldToScreen(ward.Position, out screenPos);

                var sizeMultiplier = Menu.Item("size").GetValue<Slider>().Value;

                Drawing.DrawRect(
                    new Vector2(screenPos.X - (float) 15 * sizeMultiplier / 3, screenPos.Y - 10 * sizeMultiplier),
                    new Vector2(15 * sizeMultiplier, 10 * sizeMultiplier),
                    ward.Texture);
            }
        }
    }
}