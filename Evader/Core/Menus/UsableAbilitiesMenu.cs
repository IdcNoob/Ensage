namespace Evader.Core.Menus
{
    using System.Collections.Generic;

    using Data;

    using Ensage.Common.Menu;

    internal class UsableAbilitiesMenu
    {
        #region Fields

        private readonly Dictionary<string, bool> blinkAbilities = new Dictionary<string, bool>();

        private readonly Dictionary<string, bool> counterAbilities = new Dictionary<string, bool>();

        private readonly Dictionary<string, bool> disableAbilities = new Dictionary<string, bool>();

        private readonly Menu specials;

        private readonly MenuItem usableBlinkAbilities;

        private readonly MenuItem usableCounterAbilities;

        private readonly MenuItem usableDiasbleAbilities;

        private AbilityToggler blinkAbilityToggler;

        private AbilityToggler counterAbilityToggler;

        private AbilityToggler disableAbilityToggler;

        #endregion

        #region Constructors and Destructors

        public UsableAbilitiesMenu(Menu rootMenu)
        {
            var menu = new Menu("Abilities", "usableAbilities");
            menu.AddItem(
                usableBlinkAbilities =
                    new MenuItem("usableBlinkAbilities", "Blink:").SetValue(
                        blinkAbilityToggler = new AbilityToggler(blinkAbilities)));
            menu.AddItem(
                usableCounterAbilities =
                    new MenuItem("usableCounterAbilities", "Counter:").SetValue(
                        counterAbilityToggler = new AbilityToggler(counterAbilities)));
            menu.AddItem(
                usableDiasbleAbilities =
                    new MenuItem("usableDiasbleAbilities", "Disable:").SetValue(
                        disableAbilityToggler = new AbilityToggler(disableAbilities)));

            specials = new Menu("Specials", "specials");
            menu.AddSubMenu(specials);

            rootMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Properties

        public int ArmetHpThreshold { get; private set; }

        public bool ArmletAutoToggle { get; private set; }

        public bool BloodstoneAutoSuicide { get; private set; }

        public int BloodstoneEnemyRange { get; private set; }

        public int BloodstoneHpThreshold { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void AddAbility(string abilityName, AbilityType abilityType)
        {
            switch (abilityType)
            {
                case AbilityType.Counter:
                    if (counterAbilities.ContainsKey(abilityName))
                    {
                        return;
                    }
                    counterAbilityToggler.Add(abilityName);
                    usableCounterAbilities.SetValue(counterAbilityToggler);
                    break;
                case AbilityType.Blink:
                    if (blinkAbilities.ContainsKey(abilityName))
                    {
                        return;
                    }
                    blinkAbilityToggler.Add(abilityName);
                    usableBlinkAbilities.SetValue(blinkAbilityToggler);
                    break;
                case AbilityType.Disable:
                    if (disableAbilities.ContainsKey(abilityName))
                    {
                        return;
                    }
                    disableAbilityToggler.Add(abilityName);
                    usableDiasbleAbilities.SetValue(disableAbilityToggler);
                    break;
            }

            switch (abilityName)
            {
                case "item_bloodstone":
                    var bloodstoneMenu = new Menu(string.Empty, "bloodstoneMenu", textureName: abilityName);

                    var autoSuicide = new MenuItem("bsAutoSuicide", "Auto suicide").SetValue(false);
                    bloodstoneMenu.AddItem(autoSuicide);
                    autoSuicide.ValueChanged += (sender, args) => BloodstoneAutoSuicide = args.GetNewValue<bool>();
                    BloodstoneAutoSuicide = autoSuicide.IsActive();

                    var hpCheck = new MenuItem("bsHpCheck", "Use when HP% lower than").SetValue(new Slider(15, 1, 50));
                    bloodstoneMenu.AddItem(hpCheck);
                    hpCheck.ValueChanged += (sender, args) => BloodstoneHpThreshold = args.GetNewValue<Slider>().Value;
                    BloodstoneHpThreshold = hpCheck.GetValue<Slider>().Value;

                    var enemyCheck =
                        new MenuItem("bsEnemyCheck", "Use only when enemy in range of").SetValue(
                            new Slider(750, 0, 2000));
                    bloodstoneMenu.AddItem(enemyCheck);
                    enemyCheck.ValueChanged += (sender, args) => BloodstoneEnemyRange = args.GetNewValue<Slider>().Value;
                    BloodstoneEnemyRange = enemyCheck.GetValue<Slider>().Value;

                    specials.AddSubMenu(bloodstoneMenu);
                    break;
                case "item_armlet":
                    var armletMenu = new Menu(string.Empty, "armletMenu", textureName: abilityName);

                    var autoToggle = new MenuItem("armletAutoToggle", "Auto toggle").SetValue(true);
                    armletMenu.AddItem(autoToggle);
                    autoToggle.ValueChanged += (sender, args) => ArmletAutoToggle = args.GetNewValue<bool>();
                    ArmletAutoToggle = autoToggle.IsActive();

                    var armetHpThreshold =
                        new MenuItem("armletHpThreshold", "Minimum hp").SetValue(new Slider(222, 100, 300));
                    armletMenu.AddItem(armetHpThreshold);
                    armetHpThreshold.ValueChanged +=
                        (sender, args) => ArmetHpThreshold = args.GetNewValue<Slider>().Value;
                    ArmetHpThreshold = armetHpThreshold.GetValue<Slider>().Value;

                    specials.AddSubMenu(armletMenu);
                    break;
            }
        }

        public bool Enabled(string abilityName, AbilityType abilityType)
        {
            var enabled = false;

            switch (abilityType)
            {
                case AbilityType.Counter:
                    counterAbilities.TryGetValue(abilityName, out enabled);
                    break;
                case AbilityType.Blink:
                    blinkAbilities.TryGetValue(abilityName, out enabled);
                    break;
                case AbilityType.Disable:
                    disableAbilities.TryGetValue(abilityName, out enabled);
                    break;
            }

            return enabled;
        }

        #endregion
    }
}