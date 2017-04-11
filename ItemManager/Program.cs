namespace ItemManager
{
    using System;

    using Core;

    using Ensage.Common;

    internal class Program
    {
        private static readonly Bootstrap Bootstrap = new Bootstrap();

        private static void Main()
        {
            Events.OnLoad += OnLoad;
            Events.OnClose += OnClose;
        }

        private static void OnClose(object sender, EventArgs eventArgs)
        {
            Bootstrap.OnClose();
        }

        private static void OnLoad(object sender, EventArgs eventArgs)
        {
            Bootstrap.OnLoad();
        }
    }
}