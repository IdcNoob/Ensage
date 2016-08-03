namespace HpMpAbuse.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Objects;
    using Ensage.Common.Objects.UtilityObjects;

    using HpMpAbuse.Menu;

    internal class AbilityUpdater
    {
        #region Fields

        public readonly List<string> IgnoredAbilities = new List<string>
            {
                "item_tpscroll",
                "item_travel_boots",
                "item_travel_boots_2"
            };

        private readonly List<string> addedAbilities = new List<string>();

        #endregion

        #region Constructors and Destructors

        public AbilityUpdater()
        {
            foreach (var ability in Hero.Spellbook.Spells.Reverse())
            {
                if (!IgnoredAbilities.Contains(ability.StoredName()) && !ability.IsHidden && ability.GetManaCost(1) > 0)
                {
                    Menu.AddAbility(ability.StoredName());
                }
                addedAbilities.Add(ability.StoredName());
            }

            Menu.ReloadAbilityMenu();

            Sleeper.Sleep(5000, "AbilityUpdater");
            Game.OnIngameUpdate += OnUpdate;
        }

        #endregion

        #region Properties

        private static Hero Hero => Variables.Hero;

        private static MenuManager Menu => Variables.Menu;

        private static MultiSleeper Sleeper => Variables.Sleeper;

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            Game.OnIngameUpdate -= OnUpdate;
        }

        #endregion

        #region Methods

        private void OnUpdate(EventArgs args)
        {
            if (Sleeper.Sleeping("AbilityUpdater"))
            {
                return;
            }

            Sleeper.Sleep(3000, "AbilityUpdater");

            var reload = false;

            foreach (var ability in Hero.Inventory.Items.Where(x => !addedAbilities.Contains(x.StoredName())))
            {
                if (!IgnoredAbilities.Contains(ability.StoredName()) && ability.GetManaCost(1) > 0)
                {
                    Menu.AddAbility(ability.StoredName());
                    reload = true;
                }
                addedAbilities.Add(ability.StoredName());
            }

            if (reload)
            {
                Menu.ReloadAbilityMenu();
            }
        }

        #endregion
    }
}