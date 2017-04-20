namespace PauseControl
{
    using System;

    class TickSleeper
    {
        private float lastSleepTickCount;

        public bool Sleeping => (Environment.TickCount & int.MaxValue) < lastSleepTickCount;

        public void Sleep(float duration)
        {
            lastSleepTickCount = (Environment.TickCount & int.MaxValue) + duration;
        }
    }
}