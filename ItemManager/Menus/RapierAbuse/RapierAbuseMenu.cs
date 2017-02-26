namespace ItemManager.Menus.RapierAbuse
{
    using System;

    using Args;

    using Ensage.Common.Menu;

    internal class RapierAbuseMenu
    {
        #region Constructors and Destructors

        public RapierAbuseMenu(Menu mainMenu)
        {
            var menu = new Menu("Rapier abuse", "rapierAbuse");

            var abuse = new MenuItem("rapierAbuseEnabled", "Auto combine abuse").SetValue(false);
            menu.AddItem(abuse);
            abuse.ValueChanged += (sender, args) => AbuseEnabled = args.GetNewValue<bool>();
            AbuseEnabled = abuse.IsActive();

            var manualAbuse =
                new MenuItem("rapierManualAbuseEnabled", "Manual combine abuse").SetValue(
                    new KeyBind('-', KeyBindType.Press)).SetTooltip("You can set it to your orbwalker key");
            menu.AddItem(manualAbuse);
            manualAbuse.ValueChanged +=
                (sender, args) =>
                    OnManualRapierAbuse?.Invoke(this, new BoolEventArgs(args.GetNewValue<KeyBind>().Active));

            var hpThreshold = new MenuItem("rapierhpThreshold", "HP Threshold").SetValue(new Slider(500, 0, 2000));
            menu.AddItem(hpThreshold);
            hpThreshold.ValueChanged += (sender, args) => HpThreshold = args.GetNewValue<Slider>().Value;
            HpThreshold = hpThreshold.GetValue<Slider>().Value;

            var attackFix =
                new MenuItem("rapierAttackFix", "Attack fix").SetValue(false)
                    .SetTooltip(
                        "Enable this if rapier sometimes not combining fast enough on first attack (will add small delay before attack)");
            menu.AddItem(attackFix);
            attackFix.ValueChanged += (sender, args) => AttackFix = args.GetNewValue<bool>();
            AttackFix = attackFix.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Events

        public event EventHandler<BoolEventArgs> OnManualRapierAbuse;

        #endregion

        #region Public Properties

        public bool AbuseEnabled { get; private set; }

        public bool AttackFix { get; private set; }

        public int HpThreshold { get; private set; }

        #endregion
    }
}