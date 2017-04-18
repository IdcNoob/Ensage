namespace LaneMarker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Text.RegularExpressions;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Menu;

    using SharpDX;
    using SharpDX.Direct3D9;

    internal class Program
    {
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;

        private const int MOUSEEVENTF_LEFTUP = 0x04;

        private static readonly double[,,] CoordinateMultiplayers =
        {
            //     16:9            16:10            4:3
            { { 0.19, 0.96 }, { 0.16, 0.96 }, { 0.18, 0.95 } }, // radiant safe
            { { 0.13, 0.89 }, { 0.09, 0.89 }, { 0.09, 0.89 } }, // radiant mid
            { { 0.09, 0.85 }, { 0.04, 0.85 }, { 0.03, 0.85 } }, // radiant hard
            { { 0.15, 0.93 }, { 0.12, 0.93 }, { 0.12, 0.93 } }, // radiant jungle
            { { 0.11, 0.81 }, { 0.06, 0.81 }, { 0.06, 0.80 } }, // dire safe
            { { 0.15, 0.87 }, { 0.11, 0.87 }, { 0.12, 0.87 } }, // dire mid
            { { 0.21, 0.92 }, { 0.18, 0.91 }, { 0.20, 0.91 } }, // dire hard
            { { 0.13, 0.83 }, { 0.09, 0.82 }, { 0.09, 0.83 } }, // dire jungle
        };

        private static readonly string[] LaneList = { "None", "Safe", "Mid", "Hard", "Jungle" };

        private static readonly Menu Menu = new Menu("Lane Marker", "laneMarker", true);

        private static readonly string[][] SayText =
        {
            new[]
            {
                // safe 
                "None",
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
            new[]
            {
                // mid
                "None",
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
            new[]
            {
                // hard
                "None",
                "hard",
                "hard lane",
                "solo hard",
                "solo hard pls",
                "off lane",
                "solo off pls",
                "i got this"
            },
            new[]
            {
                // jungle
                "None",
                "jungle",
                "woods"
            }
        };

        private static KeyValuePair<string, string> currentPair = new KeyValuePair<string, string>("None", "none");

        private static bool displayTempName;

        private static bool locked;

        private static int ratioAdjustment;

        private static int selectedLane;

        private static string tempName;

        private static Font textFont;

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (Drawing.Direct3DDevice9 == null)
            {
                return;
            }

            textFont.DrawText(
                null,
                "Lane: " + LaneList[selectedLane],
                (int)(HUDInfo.ScreenSizeX() * 0.01),
                (int)(HUDInfo.ScreenSizeY() * 0.06),
                Color.Yellow);

            var heroText = currentPair.Key;

            if (heroText.Contains(","))
            {
                heroText = currentPair.Key.Substring(0, currentPair.Key.IndexOf(",", StringComparison.Ordinal));
            }

            if (locked)
            {
                heroText += " (locked)";
            }

            textFont.DrawText(
                null,
                "Hero: " + heroText,
                (int)(HUDInfo.ScreenSizeX() * 0.01),
                (int)(HUDInfo.ScreenSizeY() * 0.09),
                Color.Yellow);

            if (displayTempName && tempName.Any())
            {
                textFont.DrawText(
                    null,
                    tempName,
                    (int)(HUDInfo.ScreenSizeX() * 0.01),
                    (int)(HUDInfo.ScreenSizeY() * 0.12),
                    Color.Orange);
            }
        }

        private static void Drawing_OnPostReset(EventArgs args)
        {
            textFont.OnResetDevice();
        }

        private static void Drawing_OnPreReset(EventArgs args)
        {
            textFont.OnLostDevice();
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Game.GameState == GameState.HeroSelection)
            {
                DelayAction.Add(Menu.Item("delay").GetValue<Slider>().Value, Pick);
                Unsub();
                return;
            }

            if (!Utils.SleepCheck("laneMarker.Name"))
            {
                return;
            }

            displayTempName = false;
            tempName = string.Empty;
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg != (uint)Utils.WindowsMessages.WM_KEYUP)
            {
                return;
            }

            if (args.WParam == Menu.Item("laneKey").GetValue<KeyBind>().Key)
            {
                selectedLane = selectedLane < LaneList.Length - 1 ? selectedLane + 1 : 0;
            }

            if (args.WParam == Menu.Item("lockKey").GetValue<KeyBind>().Key)
            {
                displayTempName = false;
                tempName = string.Empty;

                if (locked)
                {
                    locked = false;
                    currentPair = new KeyValuePair<string, string>("None", "none");
                }
                else
                {
                    locked = true;
                }
            }

            if (locked)
            {
                return;
            }

            var backspace = false;
            if (args.WParam == 8 && tempName.Any())
            {
                tempName = tempName.Remove(tempName.Length - 1);
                backspace = true;
            }

            var keyChar = Convert.ToChar(args.WParam);
            if ((!char.IsLetter(keyChar) || char.IsLower(keyChar) || args.WParam == 192) && !backspace)
            {
                return;
            }

            if (!backspace)
            {
                tempName = !Utils.SleepCheck("laneMarker.Name") ? tempName + keyChar : keyChar.ToString();
            }

            Utils.Sleep(2000, "laneMarker.Name");
            displayTempName = true;

            var namePair = Names.Heroes.FirstOrDefault(
                x => Regex.IsMatch(Regex.Replace(x.Key, @"\s+", ""), tempName, RegexOptions.IgnoreCase));

            if (string.IsNullOrEmpty(namePair.Key))
            {
                return;
            }

            currentPair = namePair;
        }

        private static void Main()
        {
            Menu.AddItem(new MenuItem("laneKey", "Change lane key").SetValue(new KeyBind(9, KeyBindType.Press)))
                .SetTooltip("Only works when not in game");
            Menu.AddItem(new MenuItem("lockKey", "Lock/Clear").SetValue(new KeyBind(17, KeyBindType.Press)))
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
            Menu.AddItem(new MenuItem("delay", "Delay").SetValue(new Slider(300, 300, 2000)))
                .SetTooltip("Add delay before picking/marking if it's not working");

            selectedLane = Menu.Item("defaultLane").GetValue<StringList>().SelectedIndex;

            Menu.AddToMainMenu();

            if (Drawing.RenderMode == RenderMode.Dx9)
            {
                textFont = new Font(
                    Drawing.Direct3DDevice9,
                    new FontDescription
                    {
                        FaceName = "Tahoma",
                        Height = 30,
                        OutputPrecision = FontPrecision.Raster,
                        Quality = FontQuality.ClearTypeNatural,
                        CharacterSet = FontCharacterSet.Hangul,
                        MipLevels = 3,
                        PitchAndFamily = FontPitchAndFamily.Modern,
                        Weight = FontWeight.Heavy,
                        Width = 12
                    });
            }

            var ratio = HUDInfo.ScreenSizeX() / HUDInfo.ScreenSizeY();

            if (Math.Abs(ratio - (float)16 / 9) < 0.1)
            {
                ratioAdjustment = 0;
            }
            else if (Math.Abs(ratio - (float)16 / 10) < 0.1)
            {
                ratioAdjustment = 1;
            }
            else
            {
                ratioAdjustment = 2;
            }

            Events.OnClose += (sender, args) => { Sub(); };
            Events.OnLoad += (sender, args) => { Unsub(); };

            Sub();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        private static void Pick()
        {
            if (selectedLane != 0)
            {
                var teamAdjustment = ObjectManager.LocalPlayer.Team == Team.Radiant ? 0 : LaneList.Length - 1;
                var xMult = CoordinateMultiplayers[selectedLane - 1 + teamAdjustment, ratioAdjustment, 0];
                var yMult = CoordinateMultiplayers[selectedLane - 1 + teamAdjustment, ratioAdjustment, 1];

                SetCursorPos((int)(HUDInfo.ScreenSizeX() * xMult), (int)(HUDInfo.ScreenSizeY() * yMult));
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);

                var sayTextIndex = Menu.Item(LaneList[selectedLane] + "Text").GetValue<StringList>().SelectedIndex;

                if (sayTextIndex != 0)
                {
                    Game.ExecuteCommand("say_team " + SayText[selectedLane - 1][sayTextIndex]);
                }
            }

            if (currentPair.Value != "None" && locked)
            {
                Game.ExecuteCommand("dota_select_hero " + currentPair.Value);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetCursorPos(int x, int y);

        private static void Sub()
        {
            Game.OnWndProc += Game_OnWndProc;
            Game.OnUpdate += Game_OnUpdate;

            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void Unsub()
        {
            Game.OnWndProc -= Game_OnWndProc;
            Game.OnUpdate -= Game_OnUpdate;

            Drawing.OnPreReset -= Drawing_OnPreReset;
            Drawing.OnPostReset -= Drawing_OnPostReset;
            Drawing.OnEndScene -= Drawing_OnEndScene;
        }
    }
}