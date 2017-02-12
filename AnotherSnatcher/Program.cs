namespace AnotherSnatcher
{
    using System;

    using Ensage.Common;

    internal class Program
    {
        #region Static Fields

        private static readonly Snatcher Snatcher = new Snatcher();

        #endregion

        #region Methods

        private static void Main()
        {
            Events.OnLoad += OnLoad;
            Events.OnClose += OnClose;
        }

        private static void OnClose(object sender, EventArgs e)
        {
            Snatcher.OnClose();
        }

        private static void OnLoad(object sender, EventArgs e)
        {
            Snatcher.OnLoad();
        }

        #endregion
    }
}