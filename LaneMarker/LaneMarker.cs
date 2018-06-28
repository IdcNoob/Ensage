namespace LaneMarker
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Input;

    using Ensage;
    using Ensage.SDK.Helpers;

    using Renderer;

    using SharpDX;

    using Color = System.Drawing.Color;

    internal class LaneMarker : IDisposable
    {
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;

        private const int MOUSEEVENTF_LEFTUP = 0x04;

        private const uint WM_KEYDOWN = 0x0100;

        private readonly double[,,] coordinateMultipliers =
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

        private readonly string[] laneList = { "None", "Safe", "Mid", "Hard", "Jungle" };

        private readonly int ratioAdjustment;

        private readonly SimpleRenderer renderer;

        private uint selectedLane;

        public LaneMarker(SimpleRenderer renderer)
        {
            if (Game.GameState != GameState.Init)
            {
                return;
            }

            this.renderer = renderer;
            var ratio = (float)Drawing.Width / Drawing.Height;

            if (Math.Abs(ratio - (16f / 9)) < 0.1)
            {
                this.ratioAdjustment = 0;
            }
            else if (Math.Abs(ratio - (16f / 10)) < 0.1)
            {
                this.ratioAdjustment = 1;
            }
            else
            {
                this.ratioAdjustment = 2;
            }

            Game.OnUpdate += this.OnUpdate;
            Game.OnWndProc += this.OnWndProc;
            this.renderer.Draw += this.OnDraw;
        }

        public void Dispose()
        {
            Game.OnWndProc -= this.OnWndProc;
            Game.OnUpdate -= this.OnUpdate;
            this.renderer.Draw -= this.OnDraw;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetCursorPos(int x, int y);

        private void MarkLane()
        {
            if (this.selectedLane == 0)
            {
                return;
            }

            var teamAdjustment = ObjectManager.LocalPlayer.Team == Team.Radiant ? 0 : this.laneList.Length - 1;
            var xMultiplier = this.coordinateMultipliers[(this.selectedLane - 1) + teamAdjustment, this.ratioAdjustment, 0];
            var yMultiplier = this.coordinateMultipliers[(this.selectedLane - 1) + teamAdjustment, this.ratioAdjustment, 1];

            SetCursorPos((int)(Drawing.Width * xMultiplier), (int)(Drawing.Height * yMultiplier));
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        private void OnDraw(object sender, EventArgs eventArgs)
        {
            this.renderer.DrawText(new Vector2(20, Drawing.Height * 0.08f), "Lane: " + this.laneList[this.selectedLane], Color.Orange, 20);
            this.renderer.DrawText(new Vector2(20, (Drawing.Height * 0.08f) + 20), "Change lane with Tab", Color.Orange, 20);
        }

        private void OnUpdate(EventArgs args)
        {
            if (Game.GameState != GameState.HeroSelection)
            {
                return;
            }

            UpdateManager.BeginInvoke(this.MarkLane, 300);
            this.Dispose();
        }

        private void OnWndProc(WndEventArgs args)
        {
            if (args.Msg != WM_KEYDOWN)
            {
                return;
            }

            if (KeyInterop.KeyFromVirtualKey((int)args.WParam) == Key.Tab)
            {
                this.selectedLane = this.selectedLane < this.laneList.Length - 1 ? this.selectedLane + 1 : 0;
            }
        }
    }
}