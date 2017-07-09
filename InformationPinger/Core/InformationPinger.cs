namespace InformationPinger.Core
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading.Tasks;

    using Ensage;
    using Ensage.SDK.Helpers;

    using Interfaces;

    [Export(typeof(IInformationPinger))]
    internal class InformationPinger : IInformationPinger
    {
        private readonly Queue<IPing> queue = new Queue<IPing>();

        private float canPingTime;

        private bool processing;

        private bool CanPing => Game.RawGameTime > canPingTime && !Game.IsPaused;

        public void AddPing(IPing ping)
        {
            queue.Enqueue(ping);

            if (processing)
            {
                return;
            }

            processing = true;
            UpdateManager.BeginInvoke(ProceedQueue);
        }

        private async void ProceedQueue()
        {
            while (queue.Any())
            {
                while (!CanPing)
                {
                    await Task.Delay(300);
                }

                var item = queue.Dequeue();
                item.Ping();
                canPingTime = Game.RawGameTime + item.Cooldown;

                await Task.Delay(1000);
            }

            processing = false;
        }
    }
}