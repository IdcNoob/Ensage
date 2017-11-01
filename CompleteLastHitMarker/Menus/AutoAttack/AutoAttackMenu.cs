namespace CompleteLastHitMarker.Menus.AutoAttack
{
    using Ensage.SDK.Menu;

    internal class AutoAttackMenu
    {
        public AutoAttackMenu(MenuFactory factory)
        {
            var subFactory = factory.Menu("Auto attack");

            IsEnabled = subFactory.Item("Enabled", true);
            ShowDamageFromRight = subFactory.Item("My damage on the right", false);
            ShowDamageFromRight.Item.SetTooltip("Damage will be shown on the right side of hp bar otherwise on the left");
            FillHpBar = subFactory.Item("Fill hp bar", false);
            FillHpBar.Item.SetTooltip("When unit can be killed hp bar will be fully filled");

            SplitHpBar = subFactory.Item("Split hp bar", false);
            SplitHpBar.Item.SetTooltip("Split hp bar by my damage");

            AutoAttackColors = new AutoAttackColors(subFactory);
            AutoAttackHealthBar = new AutoAttackHealthBar(subFactory);
        }

        public AutoAttackColors AutoAttackColors { get; }

        public AutoAttackHealthBar AutoAttackHealthBar { get; }

        public MenuItem<bool> FillHpBar { get; }

        public MenuItem<bool> IsEnabled { get; }

        public MenuItem<bool> ShowDamageFromRight { get; }

        public MenuItem<bool> SplitHpBar { get; }
    }
}