namespace ItemManager.Menus.Modules.Recovery.ItemSettings
{
    using Ensage.Common.Menu;

    internal class BottleSettings : ItemSettingsMenu
    {
        public BottleSettings(Menu mainMenu, string name, int? initialHp = null, int? initialMp = null)
            : base(mainMenu, name, initialHp, initialMp)
        {
            var overheal = new MenuItem("bottleOverHeal", "Overheal").SetValue(true);
            overheal.SetTooltip("If disabled bottle won't be used if either hp or mp is full");
            Menu.AddItem(overheal);
            overheal.ValueChanged += (sender, args) => OverhealEnabled = args.GetNewValue<bool>();
            OverhealEnabled = overheal.IsActive();
        }

        public bool OverhealEnabled { get; private set; }
    }
}