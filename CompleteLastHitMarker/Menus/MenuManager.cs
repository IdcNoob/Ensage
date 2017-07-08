namespace CompleteLastHitMarker.Menus
{
    using System;

    using Abilities;

    using AutoAttack;

    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    internal class MenuManager : IDisposable
    {
        private readonly MenuFactory factory;

        public MenuManager()
        {
            factory = MenuFactory.Create("CLH Marker");

            AutoAttackMenu = new AutoAttackMenu(factory);
            AbilitiesMenu = new AbilitiesMenu(factory);

            IsEnabled = factory.Item("Enabled", true);
            UpdateRate = factory.Item("Update rate", new Slider(500, 100, 1000));
        }

        public AbilitiesMenu AbilitiesMenu { get; }

        public AutoAttackMenu AutoAttackMenu { get; }

        public MenuItem<bool> IsEnabled { get; }

        public MenuItem<Slider> UpdateRate { get; }

        public void Dispose()
        {
            factory.Dispose();
        }
    }
}