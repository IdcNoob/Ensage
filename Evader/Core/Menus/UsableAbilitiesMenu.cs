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

        public bool ArmletEnemiesCheck { get; private set; }

        public bool BloodstoneAutoSuicide { get; private set; }

        public int BloodstoneEnemyRange { get; private set; }

        public int BloodstoneHpThreshold { get; private set; }

        public bool MeldBlock { get; private set; }

        public int MeldBlockTime { get; private set; }

        public bool PhaseShiftBlock { get; private set; }

        public int PhaseShiftBlockTime { get; private set; }

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

                    var enemyNearOnly = new MenuItem("armletOnlyNearEnemies", "Only near enemies").SetValue(true);
                    enemyNearOnly.SetTooltip("If enabled, will toggle only when enemies are near");
                    armletMenu.AddItem(enemyNearOnly);
                    enemyNearOnly.ValueChanged += (sender, args) => ArmletEnemiesCheck = args.GetNewValue<bool>();
                    ArmletEnemiesCheck = enemyNearOnly.IsActive();

                    specials.AddSubMenu(armletMenu);
                    break;
                case "puck_phase_shift":
                    var phaseShiftMenu = new Menu(string.Empty, "phaseShiftMenu", textureName: abilityName);

                    var phaseShiftBlock = new MenuItem("phaseShiftBlock", "Action block").SetValue(true);
                    phaseShiftBlock.SetTooltip("Block player movement/attack actions when evader used phase shift");
                    phaseShiftMenu.AddItem(phaseShiftBlock);
                    phaseShiftBlock.ValueChanged += (sender, args) => PhaseShiftBlock = args.GetNewValue<bool>();
                    PhaseShiftBlock = phaseShiftBlock.IsActive();

                    var phaseShiftBlockTime =
                        new MenuItem("phaseShiftBlockTime", "Block for (ms)").SetValue(new Slider(500, 100, 3000));
                    phaseShiftBlockTime.SetTooltip("Action won't be blocked longer than phase shift duration");
                    phaseShiftMenu.AddItem(phaseShiftBlockTime);
                    phaseShiftBlockTime.ValueChanged +=
                        (sender, args) => PhaseShiftBlockTime = args.GetNewValue<Slider>().Value;
                    PhaseShiftBlockTime = phaseShiftBlockTime.GetValue<Slider>().Value;

                    specials.AddSubMenu(phaseShiftMenu);
                    break;
                case "templar_assassin_meld":
                    var meldMenu = new Menu(string.Empty, "meldMenu", textureName: abilityName);

                    var meldBlock = new MenuItem("meldBlock", "Action block").SetValue(true);
                    meldBlock.SetTooltip("Block player movement/attack actions when evader used meld");
                    meldMenu.AddItem(meldBlock);
                    meldBlock.ValueChanged += (sender, args) => MeldBlock = args.GetNewValue<bool>();
                    MeldBlock = meldBlock.IsActive();

                    var meldBlockTime =
                        new MenuItem("meldBlockTime", "Block for (ms)").SetValue(new Slider(500, 100, 3000));
                    meldMenu.AddItem(meldBlockTime);
                    meldBlockTime.ValueChanged += (sender, args) => MeldBlockTime = args.GetNewValue<Slider>().Value;
                    MeldBlockTime = meldBlockTime.GetValue<Slider>().Value;

                    specials.AddSubMenu(meldMenu);
                    break;
            }
        }

        public bool Enabled(string abilityName, AbilityType abilityType)
        {
            switch (abilityType)
            {
                case AbilityType.Counter:
                    return counterAbilityToggler.IsEnabled(abilityName);
                case AbilityType.Blink:
                    return blinkAbilityToggler.IsEnabled(abilityName);
                case AbilityType.Disable:
                    return disableAbilityToggler.IsEnabled(abilityName);
            }

            return false;
        }

        #endregion
    }
}