namespace ExperienceTracker
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;

    internal class Enemy
    {
        #region Fields

        private readonly float bonusXp;

        private readonly Ability xpTalent;

        private float timeToShowWarning;

        #endregion

        #region Constructors and Destructors

        public Enemy(Hero enemy)
        {
            Handle = enemy.Handle;
            Hero = enemy;
            NewExperience = OldExperience = (int)enemy.CurrentXP;
            xpTalent = enemy.Spellbook.Spells.FirstOrDefault(x => x.Name.Contains("special_bonus_exp_boost"));
            if (xpTalent != null)
            {
                bonusXp = xpTalent.AbilitySpecialData.First(x => x.Name == "value").Value / 100 + 1;
            }
        }

        #endregion

        #region Public Properties

        public int EnemiesWarning { get; private set; }

        public uint Handle { get; }

        public Hero Hero { get; }

        public int NewExperience { get; private set; }

        public int OldExperience { get; private set; }

        #endregion

        #region Public Methods and Operators

        public int CalculateGainedExperience(int exp)
        {
            return Hero.Level >= 25 ? 0 : xpTalent?.Level > 0 ? (int)(exp * bonusXp) : exp;
        }

        public bool IsInExperienceRange(Creep creep)
        {
            return creep != null && creep.IsValid && Hero.IsVisible && Hero.IsAlive && Hero.Distance2D(creep) <= 1300;
        }

        public void SetExperience(int oldExp, int newExp)
        {
            if (newExp == NewExperience || oldExp > newExp)
            {
                return;
            }

            OldExperience = oldExp;
            NewExperience = newExp;
        }

        public void SetWarning(int totalExp, int enemies, int warningTime)
        {
            var earnedExp = NewExperience - OldExperience;
            if (earnedExp <= 0)
            {
                return;
            }

            EnemiesWarning = (totalExp - earnedExp * enemies) / earnedExp;

            if (EnemiesWarning <= 0)
            {
                return;
            }

            timeToShowWarning = Game.RawGameTime + warningTime;
        }

        public bool WarningIsActive()
        {
            return Hero.IsVisible && timeToShowWarning >= Game.RawGameTime;
        }

        #endregion
    }
}