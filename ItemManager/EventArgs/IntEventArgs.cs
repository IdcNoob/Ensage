namespace ItemManager.EventArgs
{
    using System;

    internal class IntEventArgs : EventArgs
    {
        public IntEventArgs(int time)
        {
            Time = time;
        }

        public int Time { get; }
    }
}