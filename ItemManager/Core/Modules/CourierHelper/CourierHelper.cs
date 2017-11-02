namespace ItemManager.Core.Modules.CourierHelper
{
    using System;
    using System.Linq;

    using Attributes;

    using Ensage;

    using Menus;
    using Menus.Modules.CourierHelper;

    using Utils;

    [Module]
    internal class CourierHelper : IDisposable
    {
        private readonly Manager manager;

        private readonly CourierHelperMenu menu;

        public CourierHelper(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.CourierHelperMenu;

            Entity.OnInt32PropertyChange += OnInt32PropertyChange;
        }

        public void Dispose()
        {
            Entity.OnInt32PropertyChange -= OnInt32PropertyChange;
        }

        private void OnInt32PropertyChange(Entity sender, Int32PropertyChangeEventArgs args)
        {
            if (args.OldValue == args.NewValue || args.PropertyName != "m_nCourierState")
            {
                return;
            }

            var courier = sender as Courier;
            if (courier == null || !courier.IsAlive)
            {
                return;
            }

            if (!menu.AutoControl)
            {
                return;
            }

            var newState = (CourierState)args.NewValue;
            var oldState = (CourierState)args.OldValue;

            if (oldState != CourierState.Deliver || newState != CourierState.BackToBase)
            {
                return;
            }

            var purchaser = courier.Inventory.Items.Select(x => x.Purchaser?.Hero)
                .OrderByDescending(x => x?.Handle == manager.MyHero.Handle)
                .FirstOrDefault(x => x?.Team == manager.MyHero.Team);

            if (purchaser == null)
            {
                return;
            }

            if (purchaser.Handle == manager.MyHero.Handle)
            {
                courier.DeliverItems();
            }
            else
            {
                courier.Resend(purchaser);
            }
        }
    }
}