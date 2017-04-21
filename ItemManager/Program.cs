namespace ItemManager
{
    using Ensage.Common;

    internal class Program
    {
        private static readonly Bootstrap Bootstrap = new Bootstrap();

        private static void Main()
        {
            Events.OnLoad += OnLoad;
            Events.OnClose += OnClose;
        }

        private static void OnClose(object sender, System.EventArgs eventArgs)
        {
            Bootstrap.OnClose();
        }

        private static void OnLoad(object sender, System.EventArgs eventArgs)
        {
            Bootstrap.OnLoad();
        }
    }
}