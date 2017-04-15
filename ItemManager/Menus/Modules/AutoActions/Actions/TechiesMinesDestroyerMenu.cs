namespace ItemManager.Menus.Modules.AutoActions.Actions
{
    using Ensage.Common.Menu;

    internal class TechiesMinesDestroyerMenu
    {
        public TechiesMinesDestroyerMenu(Menu mainMenu)
        {
            var menu = new Menu("Mines destroyer", "minesDestroyer");

            var destroyMines = new MenuItem("destroyerMines", "Destroy techies mines").SetValue(true);
            destroyMines.SetTooltip("Auto use quelling blade, iron talon and battle fury on remote/stasis mines");
            menu.AddItem(destroyMines);
            destroyMines.ValueChanged += (sender, args) => DestroyMines = args.GetNewValue<bool>();
            DestroyMines = destroyMines.IsActive();

            var attackMines = new MenuItem("attackMines", "Attack techies mines").SetValue(true);
            attackMines.SetTooltip(
                "Auto attack techies mines when they can be killed with 1 hit and no other mines around");
            menu.AddItem(attackMines);
            attackMines.ValueChanged += (sender, args) => AttackMines = args.GetNewValue<bool>();
            AttackMines = attackMines.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public bool AttackMines { get; private set; }

        public bool DestroyMines { get; private set; }
    }
}