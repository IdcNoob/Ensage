namespace HpMpAbuse.Menu
{
    using System.Collections.Generic;

    using Ensage.Common.Menu;

    internal class Recovery
    {
        #region Fields

        private readonly MenuItem bottleFountain;

        private readonly MenuItem bottleFountainIgnoreAllies;

        private readonly MenuItem forcePickEnemyDistance;

        private readonly MenuItem itemsToUse;

        private readonly MenuItem soulRingFountain;

        private bool active;

        #endregion

        #region Constructors and Destructors

        public Recovery(Menu mainMenu)
        {
            var menu = new Menu("Recovery Abuse", "recoveryAbuse", false, "item_bottle", true);

            var forcePickMenu = new Menu("Force Item picking", "forcePick");
            forcePickMenu.AddItem(
                forcePickEnemyDistance =
                new MenuItem("forcePickEnemyNearDistance", "When enemy in range").SetValue(new Slider(500, 0, 1000))
                    .SetTooltip("If enemy is closer then pick items"));

            var items = new Dictionary<string, bool>
                {
                    { "item_arcane_boots", true },
                    { "item_bottle", true },
                    { "item_guardian_greaves", true },
                    { "item_magic_stick", true },
                    { "item_mekansm", true },
                    { "item_soul_ring", true },
                    { "item_urn_of_shadows", true },
                };

            var itemsMenu = new Menu("Items to use", "itemsToUse");
            itemsMenu.AddItem(
                itemsToUse = new MenuItem("itemsToUseEnabled", "Use: ").SetValue(new AbilityToggler(items)));

            menu.AddItem(new MenuItem("recoveryKey", "Recovery key").SetValue(new KeyBind('T', KeyBindType.Press)))
                .ValueChanged += (sender, args) => Active = args.GetNewValue<KeyBind>().Active;
            menu.AddItem(
                soulRingFountain = new MenuItem("soulRingFountain", "Use soul ring at fountain").SetValue(true));
            menu.AddItem(bottleFountain = new MenuItem("bottleFountain", "Auto bottle").SetValue(true))
                .SetTooltip(
                    "Will auto use bottle on you and your allies while under the effect of fountain regeneration");
            menu.AddItem(
                bottleFountainIgnoreAllies =
                new MenuItem("bottleFountainIgnoreAllies", "Auto bottle ignore allies").SetValue(false))
                .SetTooltip("If enabled auto bottle will be used only on yourself");

            menu.AddSubMenu(forcePickMenu);
            menu.AddSubMenu(itemsMenu);

            mainMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Properties

        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                active = value;
                if (active)
                {
                    Variables.Sleeper.Reset("Main");
                }
            }
        }

        public bool BottleAtFountain => bottleFountain.IsActive();

        public bool BottleAtFountainIgnoreAllies => bottleFountainIgnoreAllies.IsActive();

        public int ForcePickEnemyDistance => forcePickEnemyDistance.GetValue<Slider>().Value;

        public bool SoulRingAtFountain => soulRingFountain.IsActive();

        #endregion

        #region Public Methods and Operators

        public bool IsEnabled(string abilityName)
        {
            return itemsToUse.GetValue<AbilityToggler>().IsEnabled(abilityName);
        }

        #endregion
    }
}