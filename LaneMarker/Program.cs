using System;
using System.Runtime.InteropServices;
using Ensage;
using Ensage.Common;
using Ensage.Common.Menu;
using SharpDX;
using SharpDX.Direct3D9;

namespace LaneMarker {
    internal class Program {
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private static bool inGame;
        private static Font textFont;
        private static int SelectedLane;

        private static readonly string[] LaneList = {"Disabled", "Safe", "Mid", "Hard", "Jungle"};

        private static readonly string[][] SayText = {
            new[] {
                // safe 
                "Disabled",
                "carry",
                "carry pls",
                "farm",
                "farm pls",
                "safe lane",
                "safe lane farm pls",
                "pick support boys",
                "afk farm",
                "playing carry since 1972"
            },
            new[] {
                // mid
                "Disabled",
                "mid",
                "mid pls",
                "pro mid here",
                "mid or feed",
                "mid or mid",
                "mid or techies",
                "mid or double mid",
                "dont even think to take my mid",
                "we lost"
            },
            new[] {
                // hard
                "Disabled",
                "hard",
                "hard lane",
                "solo hard",
                "solo hard pls",
                "off lane",
                "solo off pls",
                "i got this"
            },
            new[] {
                // jungle
                "Disabled",
                "jungle",
                "woods"
            }
        };

        private static readonly float[][] CoordinateMultiplayers = {
            new[] {0.19f, 0.96f}, // radiant safe
            new[] {0.14f, 0.87f}, // radiant mid
            new[] {0.09f, 0.82f}, // radiant hard
            new[] {0.16f, 0.94f}, // radiant jungle
            new[] {0.11f, 0.79f}, //dire safe
            new[] {0.14f, 0.87f}, //dire mid
            new[] {0.19f, 0.90f}, //dire hard
            new[] {0.13f, 0.81f} //dire jungle
        };

        private static readonly Menu Menu = new Menu("Lane Marker", "laneMarker", true);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetCursorPos(int x, int y);

        private static void Main() {
            Menu.AddItem(new MenuItem("hotkey", "Change lane key").SetValue(new KeyBind('Q', KeyBindType.Press)))
                .SetTooltip("Only works when not in game");
            Menu.AddItem(new MenuItem("defaultLane", "Default lane").SetValue(new StringList(LaneList)))
                .SetTooltip("This lane wil be marked by default when you restart dota");
            Menu.AddItem(new MenuItem("SafeText", "Safe lane text").SetValue(new StringList(SayText[0])))
                .SetTooltip("This will be said in team chat");
            Menu.AddItem(new MenuItem("MidText", "Mid lane text").SetValue(new StringList(SayText[1])))
                .SetTooltip("This will be said in team chat");
            Menu.AddItem(new MenuItem("HardText", "Hard lane text").SetValue(new StringList(SayText[2])))
                .SetTooltip("This will be said in team chat");
            Menu.AddItem(new MenuItem("JungleText", "Jungle text").SetValue(new StringList(SayText[3])))
                .SetTooltip("This will be said in team chat");

            SelectedLane = Menu.Item("defaultLane").GetValue<StringList>().SelectedIndex;

            Menu.AddToMainMenu();

            textFont = new Font(
                Drawing.Direct3DDevice9,
                new FontDescription {
                    FaceName = "Tahoma",
                    Height = 39,
                    OutputPrecision = FontPrecision.Raster,
                    Quality = FontQuality.ClearTypeNatural,
                    CharacterSet = FontCharacterSet.Hangul,
                    MipLevels = 3,
                    PitchAndFamily = FontPitchAndFamily.Modern,
                    Weight = FontWeight.Heavy,
                    Width = 15
                });

            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;

            Game.OnIngameUpdate += Game_OnUpdate;
            Game.OnFireEvent += Game_OnFireEvent;

            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void Game_OnWndProc(WndEventArgs args) {
            if (inGame || args.Msg != (uint) Utils.WindowsMessages.WM_KEYUP ||
                args.WParam != Menu.Item("hotkey").GetValue<KeyBind>().Key) return;
            SelectedLane = SelectedLane < LaneList.Length - 1 ? SelectedLane + 1 : 0;
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        private static void Game_OnFireEvent(FireEventEventArgs args) {
            if (inGame || SelectedLane == 0 || args.GameEvent.Name != "hero_picker_shown") return;

            var team = ObjectManager.LocalPlayer.Team == Team.Radiant ? 0 : LaneList.Length - 1;

            SetCursorPos((int) (HUDInfo.ScreenSizeX() * CoordinateMultiplayers[SelectedLane - 1 + team][0]),
                (int) (HUDInfo.ScreenSizeY() * CoordinateMultiplayers[SelectedLane - 1 + team][1]));

            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);

            var sayTextIndex = Menu.Item(LaneList[SelectedLane] + "Text").GetValue<StringList>().SelectedIndex;

            if (sayTextIndex != 0)
                Game.ExecuteCommand("say_team " + SayText[SelectedLane - 1][sayTextIndex]);
        }

        public static void Game_OnUpdate(EventArgs args) {
            if (!Utils.SleepCheck("laneMarkerDelay"))
                return;

            if (!inGame) {
                if (!Game.IsInGame || ObjectManager.LocalHero == null) {
                    Utils.Sleep(3000, "laneMarkerDelay");
                    return;
                }

                inGame = true;
            }

            if (!Game.IsInGame) {
                inGame = false;
                return;
            }

            Utils.Sleep(5000, "laneMarkerDelay");
        }


        private static void Drawing_OnEndScene(EventArgs args) {
            if (inGame || SelectedLane == 0 || Drawing.Direct3DDevice9 == null)
                return;

            textFont.DrawText(null, 
                LaneList[SelectedLane], 
                (int) (HUDInfo.ScreenSizeX() * 0.01),
                (int) (HUDInfo.ScreenSizeY() * 0.06), 
                Color.Yellow);
        }

        private static void Drawing_OnPostReset(EventArgs args) {
            textFont.OnResetDevice();
        }

        private static void Drawing_OnPreReset(EventArgs args) {
            textFont.OnLostDevice();
        }
    }
}