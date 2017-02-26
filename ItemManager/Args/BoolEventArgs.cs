namespace ItemManager.Args
{
    using System;

    internal class BoolEventArgs : EventArgs
    {
        #region Constructors and Destructors

        public BoolEventArgs(bool enabled)
        {
            Enabled = enabled;
        }

        #endregion

        #region Public Properties

        public bool Enabled { get; }

        #endregion
    }
}