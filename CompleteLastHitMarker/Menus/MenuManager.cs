namespace CompleteLastHitMarker.Menus
{
    using Abilities;

    using AutoAttack;

    using Ensage.Common.Menu;

    internal class MenuManager
    {
        private readonly Menu rootMenu;

        public MenuManager()
        {
            rootMenu = new Menu("CLH Marker", "completeLastHitMarker", true);

            AutoAttackMenu = new AutoAttackMenu(rootMenu);
            AbilitiesMenu = new AbilitiesMenu(rootMenu);

            var enabled = new MenuItem("enabled", "Enabled").SetValue(true);
            rootMenu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) => IsEnabled = args.GetNewValue<bool>();
            IsEnabled = enabled.IsActive();

            rootMenu.AddToMainMenu();
        }

        public AbilitiesMenu AbilitiesMenu { get; }

        public AutoAttackMenu AutoAttackMenu { get; }

        public bool IsEnabled { get; private set; }

        public void OnClose()
        {
            rootMenu.RemoveFromMainMenu();
        }
    }
}