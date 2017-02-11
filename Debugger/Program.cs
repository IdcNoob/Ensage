namespace Debugger
{
    using System;

    using Ensage.Common;

    internal class Program
    {
        #region Static Fields

        private static readonly Debugger Debugger = new Debugger();

        #endregion

        #region Methods

        private static void OnClose(object sender, EventArgs eventArgs)
        {
            Debugger.OnClose();
        }

        private static void OnLoad(object sender, EventArgs eventArgs)
        {
            Debugger.OnLoad();
        }

        private static void Main()
        {
            Events.OnLoad += OnLoad;
            Events.OnClose += OnClose;
        }

        #endregion
    }
}
