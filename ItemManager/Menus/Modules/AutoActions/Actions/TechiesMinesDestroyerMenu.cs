namespace ItemManager.Menus.Modules.AutoActions.Actions
{
    using System;

    using Ensage.Common.Menu;

    using EventArgs;

    internal class TechiesMinesDestroyerMenu
    {
        public TechiesMinesDestroyerMenu(Menu mainMenu)
        {
            var menu = new Menu("Mines destroyer", "minesDestroyer");

            var enabled = new MenuItem("destroyerMines", "Enabled").SetValue(true);
            enabled.SetTooltip("Auto use quelling blade and battle fury on remote/stasis mines");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) =>
                {
                    IsEnabled = args.GetNewValue<bool>();
                    OnEnabledChange?.Invoke(null, new BoolEventArgs(IsEnabled));
                };
            IsEnabled = enabled.IsActive();

            var attackMines = new MenuItem("attackMines", "Attack techies mines").SetValue(true);
            attackMines.SetTooltip("Auto attack techies mines when they can be killed with 1 hit and no other mines around");
            menu.AddItem(attackMines);
            attackMines.ValueChanged += (sender, args) => AttackMines = args.GetNewValue<bool>();
            AttackMines = attackMines.IsActive();

            var attackMinesInvisible = new MenuItem("attackMinesInvis", "Attack when invisible").SetValue(true);
            attackMinesInvisible.SetTooltip("Attack techies mines when your hero is invisible");
            menu.AddItem(attackMinesInvisible);
            attackMinesInvisible.ValueChanged += (sender, args) => AttackMinesInvisible = args.GetNewValue<bool>();
            AttackMinesInvisible = attackMinesInvisible.IsActive();

            var updateRate = new MenuItem("techiesMinesUpdateRate", "Update rate").SetValue(new Slider(200, 1, 500));
            updateRate.SetTooltip("Lower value => faster reaction, but requires more resources");
            menu.AddItem(updateRate);
            updateRate.ValueChanged += (sender, args) =>
                {
                    UpdateRate = args.GetNewValue<Slider>().Value;
                    OnUpdateRateChange?.Invoke(null, new IntEventArgs(UpdateRate));
                };
            UpdateRate = updateRate.GetValue<Slider>().Value;

            mainMenu.AddSubMenu(menu);
        }

        public event EventHandler<BoolEventArgs> OnEnabledChange;

        public event EventHandler<IntEventArgs> OnUpdateRateChange;

        public bool AttackMines { get; private set; }

        public bool AttackMinesInvisible { get; private set; }

        public bool IsEnabled { get; private set; }

        public int UpdateRate { get; private set; }
    }
}