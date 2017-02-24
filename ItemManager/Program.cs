namespace ItemManager
{
    using System;

    using Core;

    using Ensage.Common;

    internal class Program
    {
        #region Static Fields

        private static readonly ItemManager ItemManager = new ItemManager();

        #endregion

        #region Methods

        private static void Main()
        {
            Events.OnLoad += OnLoad;
            Events.OnClose += OnClose;
        }

        private static void OnClose(object sender, EventArgs eventArgs)
        {
            ItemManager.OnClose();
        }

        private static void OnLoad(object sender, EventArgs eventArgs)
        {
            ItemManager.OnLoad();
        }

        #endregion
    }
}