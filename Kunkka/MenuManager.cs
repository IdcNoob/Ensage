namespace Kunkka
{
    using Ensage.Common.Menu;

    internal class MenuManager
    {
        #region Fields

        private readonly MenuItem autoReturn;

        private readonly MenuItem enabled;

        private readonly MenuItem hitAndRunDamage;

        private readonly Menu menu;

        #endregion

        #region Constructors and Destructors

        public MenuManager(string heroName)
        {
            menu = new Menu("Kunkka", "kunkka", true, heroName, true);

            menu.AddItem(enabled = new MenuItem("enabled", "Enabled").SetValue(true));
            menu.AddItem(autoReturn = new MenuItem("autoReturn", "Auto return").SetValue(true))
                .SetTooltip("Will auto return enemy on Torrent, Ship, Arrow or Hook");
            menu.AddItem(new MenuItem("combo", "Combo").SetValue(new KeyBind('D', KeyBindType.Press)))
                .SetTooltip("X Mark => Torrent => Return")
                .ValueChanged += (sender, arg) => { ComboEnabled = arg.GetNewValue<KeyBind>().Active; };
            menu.AddItem(new MenuItem("fullCombo", "Full combo").SetValue(new KeyBind('F', KeyBindType.Press)))
                    .SetTooltip("X Mark => Ghost Ship => Torrent => Return")
                    .ValueChanged +=
                (sender, arg) => { ComboEnabled = FullComboEnabled = arg.GetNewValue<KeyBind>().Active; };
            menu.AddItem(new MenuItem("tpHome", "X home").SetValue(new KeyBind('G', KeyBindType.Press)))
                .SetTooltip("X Mark on self => Teleport to base")
                .ValueChanged += (sender, arg) => { TpHomeEanbled = arg.GetNewValue<KeyBind>().Active; };
            menu.AddItem(new MenuItem("hitRun", "Hit & run").SetValue(new KeyBind('H', KeyBindType.Press)))
                .SetTooltip("X Mark on self => Dagger => Hit => Return")
                .ValueChanged += (sender, arg) => { HitAndRunEnabled = arg.GetNewValue<KeyBind>().Active; };
            menu.AddItem(
                    new MenuItem("torrentStatic", "Torrent on static objects").SetValue(new KeyBind('J', KeyBindType.Press)))
                .SetTooltip("Will cast torrent on rune or aegis/wk reincarnation before spawn")
                .ValueChanged += (sender, arg) => { TorrentOnStaticObjectsEnabled = arg.GetNewValue<KeyBind>().Active; };
            menu.AddItem(hitAndRunDamage = new MenuItem("hitAndRunDamage", "Hit & run AD").SetValue(true))
                .SetTooltip("Use additional damage when using hit & run (shadow blade etc.)");

            menu.AddToMainMenu();
        }

        #endregion

        #region Public Properties

        public bool AutoReturnEnabled => autoReturn.GetValue<bool>();

        public bool ComboEnabled { get; private set; }

        public bool FullComboEnabled { get; private set; }

        public bool HitAndRunDamageEnabled => hitAndRunDamage.GetValue<bool>();

        public bool HitAndRunEnabled { get; private set; }

        public bool IsEnabled => enabled.GetValue<bool>();

        public bool TorrentOnStaticObjectsEnabled { get; private set; }

        public bool TpHomeEanbled { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            menu.RemoveFromMainMenu();
        }

        #endregion
    }
}