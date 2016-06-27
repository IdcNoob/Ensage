namespace Timbersaw
{
    internal class Program
    {
        #region Static Fields

        private static readonly Bootstrap BootstrapInstance = new Bootstrap();

        #endregion

        #region Methods

        private static void Main()
        {
            BootstrapInstance.Initialize();
        }

        #endregion
    }
}