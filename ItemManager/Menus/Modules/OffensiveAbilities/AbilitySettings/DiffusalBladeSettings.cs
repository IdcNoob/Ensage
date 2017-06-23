namespace ItemManager.Menus.Modules.OffensiveAbilities.AbilitySettings
{
    using Ensage.Common.Menu;

    internal class DiffusalBladeSettings : OffensiveAbilitySettings
    {
        public DiffusalBladeSettings(Menu mainMenu, string name, string texture = null)
            : base(mainMenu, name, texture)
        {
            var immunity = new MenuItem("diffusalPhysImmunity", "Only on immunity").SetValue(false);
            immunity.SetTooltip("Use diffusal blade ONLY when enemy has physical damage immunity (ghost, eul etc. + glimmer)");
            Menu.AddItem(immunity);
            immunity.ValueChanged += (sender, args) => ImmunityOnly = args.GetNewValue<bool>();
            ImmunityOnly = immunity.IsActive();
        }

        public bool ImmunityOnly { get; private set; }
    }
}
