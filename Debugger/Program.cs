namespace Debugger
{
    using System;

    using Ensage.Common;

    internal class Program
    {
        private static readonly Debugger Debugger = new Debugger();

        private static void Main()
        {
            Events.OnLoad += OnLoad;
            Events.OnClose += OnClose;
        }

        private static void OnClose(object sender, EventArgs eventArgs)
        {
            Debugger.OnClose();
        }

        private static void OnLoad(object sender, EventArgs eventArgs)
        {
            Debugger.OnLoad();
        }
    }
}