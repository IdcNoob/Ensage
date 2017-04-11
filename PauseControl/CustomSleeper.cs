namespace PauseControl
{
    using System;

    internal class CustomSleeper
    {
        private float lastSleepTickCount;

        public bool Sleeping => (Environment.TickCount & int.MaxValue) < lastSleepTickCount;

        public void Sleep(float duration)
        {
            lastSleepTickCount = (Environment.TickCount & int.MaxValue) + duration;
        }
    }
}