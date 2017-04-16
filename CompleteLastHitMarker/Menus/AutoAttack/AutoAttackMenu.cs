namespace CompleteLastHitMarker.Menus.AutoAttack
{
    using Ensage.Common.Menu;

    internal class AutoAttackMenu
    {
        public AutoAttackMenu(Menu rootMenu)
        {
            var menu = new Menu("Auto attack", "autoAttack");

            var enabled = new MenuItem("autoAttackEnabled", "Enabled").SetValue(true);
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) => IsEnabled = args.GetNewValue<bool>();
            IsEnabled = enabled.IsActive();

            var damageFromRight = new MenuItem("autoAttackRightDamage", "My damage on the right").SetValue(false);
            damageFromRight.SetTooltip("Damage will be shown on the right side of hp bar otherwise on the left");
            menu.AddItem(damageFromRight);
            damageFromRight.ValueChanged += (sender, args) => ShowDamageFromRight = args.GetNewValue<bool>();
            ShowDamageFromRight = damageFromRight.IsActive();

            var fillHpBar = new MenuItem("autoAttackMaximizeHpBar", "Fill hp bar").SetValue(false);
            fillHpBar.SetTooltip("When unit can be killed hp bar will be fully filled");
            menu.AddItem(fillHpBar);
            fillHpBar.ValueChanged += (sender, args) => FillHpBar = args.GetNewValue<bool>();
            FillHpBar = fillHpBar.IsActive();

            AutoAttackColors = new AutoAttackColors(menu);
            AutoAttackHealthBar = new AutoAttackHealthBar(menu);

            rootMenu.AddSubMenu(menu);
        }

        public AutoAttackColors AutoAttackColors { get; }

        public AutoAttackHealthBar AutoAttackHealthBar { get; }

        public bool FillHpBar { get; private set; }

        public bool IsEnabled { get; private set; }

        public bool ShowDamageFromRight { get; private set; }
    }
}