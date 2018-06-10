namespace AdvancedRangeDisplay
{
    using System;

    using Ensage;
    using Ensage.Common;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    [ExportPlugin("Advanced ranges")]
    internal class Bootstrap : Plugin
    {
        protected override void OnActivate()
        {
            this.ranges = new Ranges();

            ranges.OnLoad();
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        protected override void OnDeactivate()
        {
            Game.OnUpdate -= Game_OnUpdate;
            Drawing.OnDraw -= Drawing_OnDraw;
            ranges.OnClose();
        }

        private Ranges ranges;

        private void Drawing_OnDraw(EventArgs args)
        {
            ranges.OnDraw();
        }

        private void Game_OnUpdate(EventArgs args)
        {
            ranges.OnUpdate();
        }
    }
}