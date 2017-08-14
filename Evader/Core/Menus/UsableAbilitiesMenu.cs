namespace Evader.Core.Menus
{
    using System.Collections.Generic;

    using Ensage;
    using Ensage.Common.Menu;

    using AbilityType = Data.AbilityType;

    internal class UsableAbilitiesMenu
    {
        private readonly List<string> addedBlinkAbilities = new List<string>();

        private readonly List<string> addedCounterAbilities = new List<string>();

        private readonly List<string> addedDisableAbilities = new List<string>();

        private readonly Menu menu;

        private AbilityToggler blinkAbilityToggler;

        private AbilityToggler counterAbilityToggler;

        private AbilityToggler disableAbilityToggler;

        private Menu specials;

        public UsableAbilitiesMenu(Menu rootMenu)
        {
            menu = new Menu("Abilities", "usableAbilities");
            menu.AddItem(
                new MenuItem("usableBlinkAbilities", "Blink:").SetValue(
                    blinkAbilityToggler = new AbilityToggler(new Dictionary<string, bool>())));
            menu.AddItem(
                new MenuItem("usableCounterAbilities", "Counter:").SetValue(
                    counterAbilityToggler = new AbilityToggler(new Dictionary<string, bool>())));
            menu.AddItem(
                new MenuItem("usableDiasbleAbilities", "Disable:").SetValue(
                    disableAbilityToggler = new AbilityToggler(new Dictionary<string, bool>())));

            rootMenu.AddSubMenu(menu);
        }

        public bool ArmletAutoToggle { get; private set; }

        public int ArmletCheckDelay { get; private set; }

        public bool ArmletEnemiesCheck { get; private set; }

        public int ArmletHpThreshold { get; private set; }

        public bool AutoDisableAtFountain { get; private set; }

        public bool BloodstoneAutoSuicide { get; private set; }

        public int BloodstoneEnemyRange { get; private set; }

        public int BloodstoneHpThreshold { get; private set; }

        public bool MeldBlock { get; private set; }

        public int MeldBlockTime { get; private set; }

        public bool PhaseShiftBlock { get; private set; }

        public int PhaseShiftBlockTime { get; private set; }

        public void AddAbility(string abilityName, AbilityType abilityType, bool enabled = true)
        {
            switch (abilityType)
            {
                case AbilityType.Counter:
                    if (addedCounterAbilities.Contains(abilityName))
                    {
                        return;
                    }

                    counterAbilityToggler.Add(abilityName, enabled);
                    addedCounterAbilities.Add(abilityName);
                    break;
                case AbilityType.Blink:
                    if (addedBlinkAbilities.Contains(abilityName))
                    {
                        return;
                    }
                    blinkAbilityToggler.Add(abilityName, enabled);
                    addedBlinkAbilities.Add(abilityName);
                    break;
                case AbilityType.Disable:
                    if (addedDisableAbilities.Contains(abilityName))
                    {
                        return;
                    }
                    disableAbilityToggler.Add(abilityName, enabled);
                    addedDisableAbilities.Add(abilityName);
                    break;
            }

            switch (abilityName)
            {
                case "item_bloodstone":
                    InitializeSpecialsMenu();

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
                    enemyCheck.ValueChanged +=
                        (sender, args) => BloodstoneEnemyRange = args.GetNewValue<Slider>().Value;
                    BloodstoneEnemyRange = enemyCheck.GetValue<Slider>().Value;

                    specials.AddSubMenu(bloodstoneMenu);
                    Game.PrintMessage(
                        "<font color='#ff7700'>[Evader]</font> Bloodstone has special settings in the menu");
                    break;
                case "item_armlet":
                    InitializeSpecialsMenu();

                    var armletMenu = new Menu(string.Empty, "armletMenu", textureName: abilityName);

                    var autoToggle = new MenuItem("armletAutoToggle", "Auto toggle").SetValue(true);
                    armletMenu.AddItem(autoToggle);
                    autoToggle.ValueChanged += (sender, args) => ArmletAutoToggle = args.GetNewValue<bool>();
                    ArmletAutoToggle = autoToggle.IsActive();

                    var armletHpThreshold =
                        new MenuItem("armletHpThreshold", "Minimum hp").SetValue(new Slider(222, 100, 300));
                    armletMenu.AddItem(armletHpThreshold);
                    armletHpThreshold.ValueChanged +=
                        (sender, args) => ArmletHpThreshold = args.GetNewValue<Slider>().Value;
                    ArmletHpThreshold = armletHpThreshold.GetValue<Slider>().Value;

                    var checkDelay = new MenuItem("armletCheckDelay", "Check delay").SetValue(new Slider(75, 0, 300));
                    checkDelay.SetTooltip("Lower delay => better calculations, but requires more resources");
                    armletMenu.AddItem(checkDelay);
                    checkDelay.ValueChanged += (sender, args) => ArmletCheckDelay = args.GetNewValue<Slider>().Value;
                    ArmletCheckDelay = checkDelay.GetValue<Slider>().Value;

                    var fountainDisable = new MenuItem("armletAutoDisable", "Auto disable at fountain").SetValue(true);
                    armletMenu.AddItem(fountainDisable);
                    fountainDisable.ValueChanged += (sender, args) => AutoDisableAtFountain = args.GetNewValue<bool>();
                    AutoDisableAtFountain = fountainDisable.IsActive();

                    var enemyNearOnly = new MenuItem("armletOnlyNearEnemies", "Only near enemies").SetValue(true);
                    enemyNearOnly.SetTooltip("If enabled, will toggle only when enemies are near");
                    armletMenu.AddItem(enemyNearOnly);
                    enemyNearOnly.ValueChanged += (sender, args) => ArmletEnemiesCheck = args.GetNewValue<bool>();
                    ArmletEnemiesCheck = enemyNearOnly.IsActive();

                    specials.AddSubMenu(armletMenu);
                    Game.PrintMessage("<font color='#ff7700'>[Evader]</font> Armlet has special settings in the menu");
                    break;
                case "puck_phase_shift":
                    InitializeSpecialsMenu();

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
                    phaseShiftBlockTime.ValueChanged += (sender, args) =>
                        PhaseShiftBlockTime = args.GetNewValue<Slider>().Value;
                    PhaseShiftBlockTime = phaseShiftBlockTime.GetValue<Slider>().Value;

                    specials.AddSubMenu(phaseShiftMenu);
                    Game.PrintMessage(
                        "<font color='#ff7700'>[Evader]</font> Phase Shift has special settings in the menu");
                    break;
                case "templar_assassin_meld":
                    InitializeSpecialsMenu();

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
                    Game.PrintMessage("<font color='#ff7700'>[Evader]</font> Meld has special settings in the menu");
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

        private void InitializeSpecialsMenu()
        {
            if (specials != null)
            {
                return;
            }

            specials = new Menu("Specials", "specials");
            menu.AddSubMenu(specials);
        }
    }
}