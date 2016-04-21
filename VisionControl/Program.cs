using System;
using Ensage;
using Ensage.Common;
using Ensage.Common.Menu;
using Ensage.Common.Objects;
using SharpDX;

namespace VisionControl {
    internal static class Program {
        private static bool inGame;
        public static Hero Hero;

        public static readonly Menu Menu = new Menu("Vision Control", "visionControl", true);

        public static int GetMenuSliderValue(string item) {
            return Menu.Item(item).GetValue<Slider>().Value;
        }

        private static void Main() {
            Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true))
                .ValueChanged += (sender, arg) => { MapWard.ChangeParticles(arg.GetNewValue<bool>()); };
            Menu.AddItem(new MenuItem("hide", "Smart icon hide").SetValue(true)
                .SetTooltip("Ward icon will be hidden if enemy ward is visible"))
                .ValueChanged += (sender, arg) => { MapWard.ResetIconHide(arg.GetNewValue<bool>()); };
            Menu.AddItem(new MenuItem("notification", "Notification").SetValue(true)
                .SetTooltip("Shows side message when enemy placed ward"));
            Menu.AddItem(new MenuItem("size", "Icon size").SetValue(new Slider(4, 2, 8)));

            var rangesMenu = new Menu("Ward ranges", "rangesMenu");
            rangesMenu.AddItem(new MenuItem("enabledRanges", "Show ranges").SetValue(true))
                .ValueChanged += (sender, arg) => { MapWard.ChangeParticles(arg.GetNewValue<bool>(), true); };

            rangesMenu.AddItem(new MenuItem("observer", "Observer colors"));
            rangesMenu.AddItem(new MenuItem("red", "Red").SetValue(new Slider(255, 0, 255)))
                .SetFontStyle(fontColor: Color.IndianRed)
                .ValueChanged +=
                (sender, arg) => {
                    MapWard.ChangeColor(ClassID.CDOTA_NPC_Observer_Ward, Color.Red, arg.GetNewValue<Slider>().Value);
                };
            rangesMenu.AddItem(new MenuItem("green", "Green").SetValue(new Slider(255, 0, 255)))
                .SetFontStyle(fontColor: Color.LightGreen)
                .ValueChanged +=
                (sender, arg) => {
                    MapWard.ChangeColor(ClassID.CDOTA_NPC_Observer_Ward, Color.Green, arg.GetNewValue<Slider>().Value);
                };
            rangesMenu.AddItem(new MenuItem("blue", "Blue").SetValue(new Slider(0, 0, 255)))
                .SetFontStyle(fontColor: Color.LightBlue).ValueChanged +=
                (sender, arg) => {
                    MapWard.ChangeColor(ClassID.CDOTA_NPC_Observer_Ward, Color.Blue, arg.GetNewValue<Slider>().Value);
                };
            rangesMenu.AddItem(new MenuItem("sentry", "Sentry colors"));
            rangesMenu.AddItem(new MenuItem("redS", "Red").SetValue(new Slider(0, 0, 255)))
                .SetFontStyle(fontColor: Color.IndianRed)
                .ValueChanged +=
                (sender, arg) => {
                    MapWard.ChangeColor(ClassID.CDOTA_NPC_Observer_Ward_TrueSight, Color.Red,
                        arg.GetNewValue<Slider>().Value);
                };
            rangesMenu.AddItem(new MenuItem("greenS", "Green").SetValue(new Slider(135, 0, 255)))
                .SetFontStyle(fontColor: Color.LightGreen)
                .ValueChanged +=
                (sender, arg) => {
                    MapWard.ChangeColor(ClassID.CDOTA_NPC_Observer_Ward_TrueSight, Color.Green,
                        arg.GetNewValue<Slider>().Value);
                };
            rangesMenu.AddItem(new MenuItem("blueS", "Blue").SetValue(new Slider(255, 0, 255)))
                .SetFontStyle(fontColor: Color.LightBlue).ValueChanged +=
                (sender, arg) => {
                    MapWard.ChangeColor(ClassID.CDOTA_NPC_Observer_Ward_TrueSight, Color.Blue,
                        arg.GetNewValue<Slider>().Value);
                };

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

                Heroes.GetByTeam(Team.Radiant);

                MapWard.Clear();
                HeroWard.Clear();

                inGame = true;
            }

            if (!Game.IsInGame) {
                inGame = false;
                return;
            }

            if (Menu.Item("enabled").GetValue<bool>()) {
                HeroWard.Update();
                MapWard.Update();
            }

            Utils.Sleep(333, "VisionControlDelay");
        }

        private static void Drawing_OnDraw(EventArgs args) {
            if (!inGame || !Menu.Item("enabled").GetValue<bool>()) return;

            var iconSizeMultiplier = Menu.Item("size").GetValue<Slider>().Value;

            var timeColor = new Color(GetMenuSliderValue("redT"), GetMenuSliderValue("greenT"),
                GetMenuSliderValue("blueT"));
            var timerSize = Menu.Item("sizeTimer").GetValue<Slider>().Value * 7;

            foreach (var ward in MapWard.MapWards) {
                Vector2 screenPos;
                Drawing.WorldToScreen(ward.Position, out screenPos);

                if (screenPos.IsZero)
                    continue;

                if (ward.Show) {
                    Drawing.DrawRect(
                        new Vector2(screenPos.X - (float) 15 * iconSizeMultiplier / 3,
                            screenPos.Y - 10 * iconSizeMultiplier),
                        new Vector2(15 * iconSizeMultiplier, 10 * iconSizeMultiplier),
                        ward.Texture);
                }

                if (!Menu.Item("enabledTimer").GetValue<bool>())
                    continue;

                var time = TimeSpan.FromSeconds(ward.EndTime - Game.GameTime);

                Drawing.DrawText(
                    time.ToString(@"m\:ss"),
                    "Arial",
                    new Vector2(screenPos.X - 20, screenPos.Y),
                    new Vector2(timerSize, timerSize),
                    timeColor,
                    FontFlags.None);
            }
        }
    }
}