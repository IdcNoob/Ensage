namespace CompleteLastHitMarker
{
    using System;

    using Core;

    using Ensage.Common;

    internal class Program
    {
        private static readonly Marker Marker = new Marker();

        private static void Main()
        {
            Events.OnLoad += OnLoad;
            Events.OnClose += OnClose;
        }

        private static void OnClose(object sender, EventArgs eventArgs)
        {
            Marker.OnClose();
        }

        private static void OnLoad(object sender, EventArgs eventArgs)
        {
            Marker.OnLoad();
        }
    }
}