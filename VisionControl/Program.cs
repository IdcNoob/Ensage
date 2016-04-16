using System;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Menu;
using SharpDX;
using SharpDX.Direct3D9;

namespace VisionControl {
    internal static class Program {
        private static bool inGame;
        public static Hero Hero;

        private static readonly Menu Menu = new Menu("Vision Control", "visionControl", true);

        public static bool IsEnabled {
            get { return Menu.Item("enabled").GetValue<bool>(); }
        }

        public static bool SmartHide {
            get { return Menu.Item("hide").GetValue<bool>(); }
        }

        public static bool IsRangesEnabled {
            get { return Menu.Item("enabledRanges").GetValue<bool>(); }
        }

        public static int GetMenuItem(string item) {
            return Menu.Item(item).GetValue<Slider>().Value;
        }

        private static void Main() {
            Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
            Menu.AddItem(new MenuItem("hide", "Smart icon hide").SetValue(true)
                .SetTooltip("Ward icon will be hidden if enemy ward is visible"));
            Menu.AddItem(new MenuItem("size", "Icon size").SetValue(new Slider(4, 2, 8)));

            var rangesMenu = new Menu("Ward ranges", "rangesMenu");
            rangesMenu.AddItem(new MenuItem("enabledRanges", "Show ranges").SetValue(true));

            rangesMenu.AddItem(new MenuItem("observer", "Observer colors"));
            rangesMenu.AddItem(new MenuItem("red", "Red").SetValue(new Slider(255, 0, 255)))
                .SetFontStyle(fontColor: Color.IndianRed);
            rangesMenu.AddItem(new MenuItem("green", "Green").SetValue(new Slider(255, 0, 255)))
                .SetFontStyle(fontColor: Color.LightGreen);
            rangesMenu.AddItem(new MenuItem("blue", "Blue").SetValue(new Slider(0, 0, 255)))
                .SetFontStyle(fontColor: Color.LightBlue);

            rangesMenu.AddItem(new MenuItem("sentry", "Sentry colors"));
            rangesMenu.AddItem(new MenuItem("redS", "Red").SetValue(new Slider(0, 0, 255)))
                .SetFontStyle(fontColor: Color.IndianRed);
            rangesMenu.AddItem(new MenuItem("greenS", "Green").SetValue(new Slider(135, 0, 255)))
                .SetFontStyle(fontColor: Color.LightGreen);
            rangesMenu.AddItem(new MenuItem("blueS", "Blue").SetValue(new Slider(255, 0, 255)))
                .SetFontStyle(fontColor: Color.LightBlue);
            rangesMenu.AddItem(new MenuItem("hint", "Reload assembly after changes"))
                .SetFontStyle(fontColor: Color.Yellow);

            var timerMenu = new Menu("Ward timers", "timerMenu");
            timerMenu.AddItem(new MenuItem("enabledTimer", "Show timers").SetValue(true));
            timerMenu.AddItem(new MenuItem("sizeTimer", "Text size").SetValue(new Slider(4, 2, 8)));
            timerMenu.AddItem(new MenuItem("timer", "Timer colors"));
            timerMenu.AddItem(new MenuItem("redT", "Red").SetValue(new Slider(0, 0, 255)))
                .SetFontStyle(fontColor: Color.IndianRed);
            timerMenu.AddItem(new MenuItem("greenT", "Green").SetValue(new Slider(255, 0, 255)))
                .SetFontStyle(fontColor: Color.LightGreen);
            timerMenu.AddItem(new MenuItem("blueT", "Blue").SetValue(new Slider(0, 0, 255)))
                .SetFontStyle(fontColor: Color.LightBlue);

            Menu.AddSubMenu(rangesMenu);
            Menu.AddSubMenu(timerMenu);

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

            if (IsEnabled) {
                HeroWard.Update();
                MapWard.Update();
            }

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

            if (!Menu.Item("enabledTimer").GetValue<bool>())
                return;

            var color = new Color(GetMenuItem("redT"), GetMenuItem("greenT"), GetMenuItem("blueT"));
            var size = Menu.Item("sizeTimer").GetValue<Slider>().Value * 7;

            foreach (var ward in MapWard.MapWards) {
                var time = TimeSpan.FromSeconds(ward.EndTime - Game.GameTime);

                Vector2 screenPos;
                Drawing.WorldToScreen(ward.Position, out screenPos);

                Drawing.DrawText(
                    time.ToString(@"m\:ss"),
                    "Arial",
                    new Vector2(screenPos.X - 20, screenPos.Y),
                    new Vector2(size, size),
                    color,
                    FontFlags.None);

            }

        }
        
    }
}