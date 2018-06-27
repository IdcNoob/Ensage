namespace LaneMarker
{
    using Ensage;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using Renderer;

    [ExportPlugin("Lane marker")]
    internal class Bootstrap : Plugin
    {
        private static LaneMarker laneMarker;

        private static SimpleRenderer renderer;

        public static void Main()
        {
            if (Game.IsInGame)
            {
                return;
            }

            renderer = new SimpleRenderer();
            laneMarker = new LaneMarker(renderer);
        }

        protected override void OnActivate()
        {
            laneMarker?.Dispose();
            renderer?.Dispose();
        }
    }
}