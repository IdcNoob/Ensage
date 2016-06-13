namespace SimpleAbilityLeveling
{
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
            abilities = hero.Spellbook.Spells.Where(x => !x.IsHidden && !IgnoredAbilities.List.Contains(x.StoredName()));
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

            if (hero.AbilityPoints <= 0 || !menuManager.IsEnabled || Game.IsPaused)
            {
                return;
            }

            var learnableAbilities =
                abilities.OrderByDescending(x => menuManager.GetAbilityPriority(x.StoredName()))
                    .Where(
                        x => menuManager.AbilityActive(x.StoredName()) && IsLearnable(x) // x.IsLearnable
                    ).ToList();

            var upgrade = learnableAbilities.FirstOrDefault(ForceLearn)
                          ?? learnableAbilities.FirstOrDefault(x => !IsLocked(x, learnableAbilities));

            Player.UpgradeAbility(hero, upgrade ?? learnableAbilities.FirstOrDefault());
        }

        #endregion

        #region Methods

        private bool ForceLearn(Ability ability)
        {
            var forceHeroLevel = menuManager.ForceAbilityLearnHeroLevel(ability.StoredName());

            if (forceHeroLevel <= 0 || ability.Level >= 1)
            {
                return false;
            }

            return hero.Level >= forceHeroLevel;
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
                default:
                    // ability.MaximumLevel
                    var maxLevel = ability.AbilityType == AbilityType.Attribute
                                       ? 10
                                       : hero.ClassID == ClassID.CDOTA_Unit_Hero_Invoker ? 7 : 4;

                    if (abilityLevel >= maxLevel || heroLevel < abilityLevel * 2 + 1)
                    {
                        return false;
                    }

                    break;
            }
            return true;
        }

        private bool IsLocked(Ability ability, IEnumerable<Ability> learnableAbilities)
        {
            var lockLevel = menuManager.AbilityLockLevel(ability.StoredName());

            if (lockLevel <= 0)
            {
                return false;
            }

            var abilityLevel = ability.Level;

            var otherNotLockedAbilities =
                learnableAbilities.Where(
                    x =>
                    !x.Equals(ability) && x.AbilityType != AbilityType.Attribute
                    && menuManager.AbilityLockLevel(ability.StoredName()) > x.Level
                    && !menuManager.AbilityFullyLocked(x.StoredName()));

            return abilityLevel >= lockLevel
                   && (otherNotLockedAbilities.Any() || menuManager.AbilityFullyLocked(ability.StoredName()));
        }

        #endregion
    }
}