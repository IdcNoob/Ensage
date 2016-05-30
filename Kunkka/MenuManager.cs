namespace Kunkka
{
    using Ensage.Common.Menu;

    internal class MenuManager
    {
        #region Fields

        private readonly Menu menu;

        #endregion

        #region Constructors and Destructors

        public MenuManager(string heroName)
        {
            menu = new Menu("Kunkka", "kunkka", true, heroName, true);

            menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
            menu.AddItem(new MenuItem("autoReturn", "Auto return").SetValue(true))
                .SetTooltip("Will auto return enemy on Torrent or Mirana's arrow");
            menu.AddItem(new MenuItem("combo", "Combo").SetValue(new KeyBind('D', KeyBindType.Press)))
                .SetTooltip("X Mark => Torrent => Return");
            menu.AddItem(new MenuItem("fullCombo", "Full combo").SetValue(new KeyBind('F', KeyBindType.Press)))
                .SetTooltip("X Mark => Ghost Ship => Torrent => Return");
            menu.AddItem(new MenuItem("tpHome", "X home").SetValue(new KeyBind('G', KeyBindType.Press)))
                .SetTooltip("X Mark on self => Teleport to base");
            menu.AddItem(new MenuItem("hitRun", "Hit & run").SetValue(new KeyBind('H', KeyBindType.Press)))
                .SetTooltip("X Mark on self => Dagger => Hit Creep => Return");

            menu.AddToMainMenu();
        }

        #endregion

        #region Public Properties

        public bool AutoReturnEnabled => menu.Item("autoReturn").GetValue<bool>();

        public bool ComboEnabled
            => menu.Item("combo").GetValue<KeyBind>().Active || menu.Item("fullCombo").GetValue<KeyBind>().Active;

        public bool FullComboEnabled => menu.Item("fullCombo").GetValue<KeyBind>().Active;

        public bool HitAndRunEnabled => menu.Item("hitRun").GetValue<KeyBind>().Active;

        public bool IsEnabled => menu.Item("enabled").GetValue<bool>();

        public bool TpHomeEanbled => menu.Item("tpHome").GetValue<KeyBind>().Active;

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            menu.RemoveFromMainMenu();
        }

        #endregion
    }
}