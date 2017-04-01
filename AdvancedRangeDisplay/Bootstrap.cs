namespace AdvancedRangeDisplay
{
    using System;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Threading;

    internal class Bootstrap
    {
        private readonly Ranges ranges = new Ranges();

        public void Initialize()
        {
            Events.OnLoad += OnLoad;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            ranges.OnDraw();
        }

        private void Game_OnUpdate(EventArgs args)
        {
            ranges.OnUpdate();
        }

        private void OnClose(object sender, EventArgs e)
        {
            Events.OnClose -= OnClose;
            GameDispatcher.OnIngameUpdate -= Game_OnUpdate;
            Drawing.OnDraw -= Drawing_OnDraw;
            ranges.OnClose();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            ranges.OnLoad();
            Events.OnClose += OnClose;
            GameDispatcher.OnIngameUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }
    }
}