namespace ItemManager.EventArgs
{
    using System;

    internal class BoolEventArgs : EventArgs
    {
        public BoolEventArgs(bool enabled)
        {
            Enabled = enabled;
        }

        public bool Enabled { get; }
    }
}