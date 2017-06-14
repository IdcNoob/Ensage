namespace ItemManager.Utils
{
    using Ensage.SDK.Handlers;

    internal static class ModuleUtils
    {
        public static void SetUpdateRate(this IUpdateHandler updateHandler, int rate)
        {
            var timeoutHandler = updateHandler.Executor as TimeoutHandler;
            if (timeoutHandler != null)
            {
                timeoutHandler.Timeout = rate;
            }
        }
    }
}