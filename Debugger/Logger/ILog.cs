namespace Debugger.Logger
{
    internal interface ILog
    {
        void Display(LogItem newItem);

        bool IsMouseUnderLog();
    }
}