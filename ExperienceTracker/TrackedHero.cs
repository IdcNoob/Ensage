namespace ExperienceTracker
{
    using System;
    using System.Linq;

    using Ensage;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Helpers;

    internal class TrackedHero
    {
        private readonly float bonusXp;

        private readonly Team team;

        private readonly Ability xpTalent;

        private float timeToShowWarning;

        public TrackedHero(Hero enemy)
        {
            Hero = enemy;
            Handle = enemy.Handle;
            team = enemy.Team;
            OldExperience = (int)enemy.CurrentXP;
            xpTalent = enemy.Spellbook.Spells.FirstOrDefault(x => x.Name.Contains("special_bonus_exp_boost"));
            if (xpTalent != null)
            {
                bonusXp = (xpTalent.AbilitySpecialData.First(x => x.Name == "value").Value / 100) + 1;
            }
        }

        public int EnemiesWarning { get; private set; }

        public uint Handle { get; }

        public Hero Hero { get; }

        public bool IsValid => Hero.IsValid;

        public int NewExperience { get; private set; }

        public int OldExperience { get; private set; }

        public bool WarningIsActive => Hero.IsVisible && timeToShowWarning >= Game.RawGameTime;

        public int CalculateGainedExperience(int exp)
        {
            return Hero.Level >= 25 ? 0 : xpTalent?.Level > 0 ? (int)(exp * bonusXp) : exp;
        }

        public bool IsInExperienceRange(Creep creep)
        {
            return creep.IsValid && Hero.IsValid && creep.Team != team && Hero.IsVisible && Hero.IsAlive && Hero.Distance2D(creep) <= 1600;
        }

        public void SetExperience(int oldExp, int newExp)
        {
            if (oldExp > newExp)
            {
                return;
            }

            NewExperience = newExp;
            UpdateManager.BeginInvoke(() => OldExperience = oldExp, 130);
        }

        public void SetWarning(int totalExp, int enemies, int warningTime)
        {
            var earnedExp = NewExperience - OldExperience;
            if (earnedExp <= 0)
            {
                return;
            }

            var count = Math.Round((totalExp - (earnedExp * enemies)) / (float)earnedExp);
            if (count <= 0)
            {
                return;
            }

            EnemiesWarning = (int)count;
            timeToShowWarning = Game.RawGameTime + warningTime;
        }
    }
}