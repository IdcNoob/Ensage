namespace SimpleAbilityLeveling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Objects;

    internal class AbilityLeveling
    {
        #region Fields

        private IEnumerable<Ability> abilities;

        private Hero hero;

        private MenuManager menuManager;

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            menuManager.OnClose();
        }

        public void OnLoad()
        {
            hero = ObjectManager.LocalHero;
            abilities =
                hero.Spellbook.Spells.Where(x => !x.IsHidden && !IgnoredAbilities.List.Contains(x.StoredName()));
            menuManager = new MenuManager(abilities.Select(x => x.StoredName()).ToList(), hero.Name);

            Utils.Sleep(10000, "AbilityLeveling.Sleep");
        }

        public void OnUpdate()
        {
            if (!Utils.SleepCheck("AbilityLeveling.Sleep"))
            {
                return;
            }

            Utils.Sleep(1000, "AbilityLeveling.Sleep");

            if (Game.IsPaused || !hero.IsAlive || hero.AbilityPoints <= 0 || !menuManager.IsEnabled)
            {
                return;
            }

            var learnableAbilities =
                abilities.OrderByDescending(x => menuManager.GetAbilityPriority(x.StoredName()))
                    .Where(
                        x => menuManager.AbilityActive(x.StoredName()) && IsLearnable(x)
                    // x.IsLearnable
                    ).ToList();

            var forceLearn = learnableAbilities.FirstOrDefault(ForceLearn);
            var unlockedAbility = learnableAbilities.FirstOrDefault(x => !IsLocked(x));
            Player.UpgradeAbility(hero, forceLearn ?? unlockedAbility ?? learnableAbilities.FirstOrDefault());
        }

        #endregion

        #region Methods

        private bool ForceLearn(Ability ability)
        {
            var forceAbilityLearnLevel = menuManager.ForceAbilityLearn(ability.StoredName());

            if (forceAbilityLearnLevel <= 0)
            {
                return false;
            }

            return hero.Level == forceAbilityLearnLevel;
        }

        private bool IsLearnable(Ability ability)
        {
            var abilityLevel = ability.Level;
            var heroLevel = hero.Level;

            switch (ability.AbilityType)
            {
                case AbilityType.Ultimate:
                    if (hero.ClassID == ClassID.CDOTA_Unit_Hero_Invoker)
                    {
                        abilityLevel--;
                    }
                    if (abilityLevel >= 3)
                    {
                        return false;
                    }
                    switch (hero.ClassID)
                    {
                        case ClassID.CDOTA_Unit_Hero_Meepo:
                            switch (abilityLevel)
                            {
                                case 0:
                                    if (heroLevel < 3)
                                    {
                                        return false;
                                    }
                                    break;
                                case 1:
                                    if (heroLevel < 10)
                                    {
                                        return false;
                                    }
                                    break;
                                case 2:
                                    if (heroLevel < 17)
                                    {
                                        return false;
                                    }
                                    break;
                            }
                            break;
                        default:
                            switch (abilityLevel)
                            {
                                case 0:
                                    if (heroLevel < 6)
                                    {
                                        return false;
                                    }
                                    break;
                                case 1:
                                    if (heroLevel < 11)
                                    {
                                        return false;
                                    }
                                    break;
                                case 2:
                                    if (heroLevel < 16)
                                    {
                                        return false;
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
                case AbilityType.Basic:
                    switch (hero.ClassID)
                    {
                        case ClassID.CDOTA_Unit_Hero_Invoker:
                            if (abilityLevel >= 7)
                            {
                                return false;
                            }
                            switch (abilityLevel)
                            {
                                case 0:
                                    if (heroLevel < 1)
                                    {
                                        return false;
                                    }
                                    break;
                                case 1:
                                    if (heroLevel < 3)
                                    {
                                        return false;
                                    }
                                    break;
                                case 2:
                                    if (heroLevel < 5)
                                    {
                                        return false;
                                    }
                                    break;
                                case 3:
                                    if (heroLevel < 7)
                                    {
                                        return false;
                                    }
                                    break;
                                case 4:
                                    if (heroLevel < 9)
                                    {
                                        return false;
                                    }
                                    break;
                                case 5:
                                    if (heroLevel < 11)
                                    {
                                        return false;
                                    }
                                    break;
                                case 6:
                                    if (heroLevel < 13)
                                    {
                                        return false;
                                    }
                                    break;
                            }
                            break;
                        default:
                            if (abilityLevel >= 4)
                            {
                                return false;
                            }
                            switch (abilityLevel)
                            {
                                case 0:
                                    if (heroLevel < 1)
                                    {
                                        return false;
                                    }
                                    break;
                                case 1:
                                    if (heroLevel < 3)
                                    {
                                        return false;
                                    }
                                    break;
                                case 2:
                                    if (heroLevel < 5)
                                    {
                                        return false;
                                    }
                                    break;
                                case 3:
                                    if (heroLevel < 7)
                                    {
                                        return false;
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
                case AbilityType.Attribute:
                    if (abilityLevel >= 10)
                    {
                        return false;
                    }
                    switch (abilityLevel)
                    {
                        case 0:
                            if (heroLevel < 1)
                            {
                                return false;
                            }
                            break;
                        case 1:
                            if (heroLevel < 3)
                            {
                                return false;
                            }
                            break;
                        case 2:
                            if (heroLevel < 5)
                            {
                                return false;
                            }
                            break;
                        case 3:
                            if (heroLevel < 7)
                            {
                                return false;
                            }
                            break;
                        case 4:
                            if (heroLevel < 9)
                            {
                                return false;
                            }
                            break;
                        case 5:
                            if (heroLevel < 11)
                            {
                                return false;
                            }
                            break;
                        case 6:
                            if (heroLevel < 13)
                            {
                                return false;
                            }
                            break;
                        case 7:
                            if (heroLevel < 15)
                            {
                                return false;
                            }
                            break;
                        case 8:
                            if (heroLevel < 17)
                            {
                                return false;
                            }
                            break;
                        case 9:
                            if (heroLevel < 19)
                            {
                                return false;
                            }
                            break;
                    }
                    break;
            }
            return true;
        }

        private bool IsLocked(Ability ability)
        {
            var lockLevel = menuManager.AbilityLockLevel(ability.StoredName());

            if (lockLevel <= 0)
            {
                return false;
            }

            var abilityLevel = ability.Level;

            var spells =
                abilities.Where(
                    x =>
                    !x.Equals(ability) && x.AbilityType != AbilityType.Attribute && menuManager.AbilityActive(x.StoredName())
                    && IsLearnable(x) && menuManager.AbilityLockLevel(ability.StoredName()) >= 1);

            return abilityLevel >= lockLevel && (spells.Any() || menuManager.AbilityFullLocked(ability.StoredName()));
        }

        #endregion
    }
}