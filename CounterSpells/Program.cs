using System;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using SharpDX;
using SharpDX.Direct3D9;

namespace CounterSpells {
    internal static class Program {
        private static bool inGame;
        public static bool CameraCentered;
        private static Font text;
        public static readonly Menu Menu = new Menu("Counter Spells", "counterSpells", true);
        public static Hero Hero { get; private set; }

        private static void Main() {
            Menu.AddItem(new MenuItem("key", "Enabled").SetValue(new KeyBind('P', KeyBindType.Toggle, true)));
            Menu.AddItem(new MenuItem("blink", "Use blink").SetValue(true)
                .SetTooltip("Suports Blink Dagger and most of blink type abilities"));
            Menu.AddItem(new MenuItem("forceBlink", "Force blink dagger").SetValue(true)
                .SetTooltip(
                    "Blink Dagger will be used on your hero position if it's not enough time to blink in fountain direction"));
            Menu.AddItem(new MenuItem("blinkSilenced", "Use blink when silenced").SetValue(true)
                .SetTooltip("\"Use blink\" must be enabled"));
            Menu.AddItem(new MenuItem("center", "Center camera on blink").SetValue(true));
            Menu.AddItem(new MenuItem("disable", "Disable enemy if can't dodge").SetValue(false)
                .SetTooltip("Use hex, stun, silence when you don't have eul, dagger, dark pact etc. to dodge stun"));
            Menu.AddItem(new MenuItem("diffusal", "Use diffusal blade when silenced").SetValue(false));
            Menu.AddItem(new MenuItem("delay", "Delay between abilities").SetValue(new Slider(1000, 200, 2000)))
                .SetTooltip("Time in ms between counter abilities usage");
            Menu.AddItem(new MenuItem("size", "Text Size").SetValue(new Slider(6, 1, 10)))
                .SetTooltip("Reload assembly to apply new size");
            Menu.AddItem(new MenuItem("x", "Text position X").SetValue(new Slider(0, 0, (int) HUDInfo.ScreenSizeX())));
            Menu.AddItem(new MenuItem("y", "Text position Y").SetValue(new Slider(0, 0, (int) HUDInfo.ScreenSizeY())));
            Menu.AddToMainMenu();

            text = new Font(
                Drawing.Direct3DDevice9,
                new FontDescription {
                    FaceName = "Tahoma",
                    Height = 13 * (Menu.Item("size").GetValue<Slider>().Value / 2),
                    OutputPrecision = FontPrecision.Raster,
                    Quality = FontQuality.ClearTypeNatural,
                    CharacterSet = FontCharacterSet.Hangul,
                    MipLevels = 3,
                    PitchAndFamily = FontPitchAndFamily.Modern,
                    Weight = FontWeight.Heavy,
                    Width = 5 * (Menu.Item("size").GetValue<Slider>().Value / 2)
                });

            Game.OnUpdate += Game_OnUpdate;

            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void Game_OnUpdate(EventArgs args) {
            if (!Utils.SleepCheck("CounterDelay"))
                return;

            if (!inGame) {
                Hero = ObjectManager.LocalHero;

                if (!Game.IsInGame || Hero == null) {
                    Utils.Sleep(1000, "CounterDelay");
                    return;
                }

                inGame = true;
            }

            if (!Game.IsInGame) {
                inGame = false;
                return;
            }

            if (!Hero.IsAlive || Game.IsPaused || !Menu.Item("key").GetValue<KeyBind>().Active) {
                Utils.Sleep(500, "CounterDelay");
                return;
            }

            if (CameraCentered) {
                CameraCentered = false;
                Game.ExecuteCommand("-dota_camera_center_on_hero");
            }

            if (!Hero.CanUseItems() || Hero.IsChanneling() || Hero.IsInvul() ||
                (Hero.IsInvisible() && !Hero.IsVisibleToEnemies))
                return;

            Counter.MainCounters();
            Counter.Projectile();
            Counter.Modifier();
            Counter.SpellModifier();
            Counter.Effect();
        }

        private static void Drawing_OnEndScene(EventArgs args) {
            if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !inGame ||
                !Menu.Item("key").GetValue<KeyBind>().Active)
                return;

            text.DrawText(null, "Dodge enabled", Menu.Item("x").GetValue<Slider>().Value,
                Menu.Item("y").GetValue<Slider>().Value, Color.DarkOrange);
        }

        private static void Drawing_OnPostReset(EventArgs args) {
            text.OnResetDevice();
        }

        private static void Drawing_OnPreReset(EventArgs args) {
            text.OnLostDevice();
        }
    }
}