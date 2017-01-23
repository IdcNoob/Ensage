namespace Evader.EvadableAbilities.Modifiers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core;

    using Ensage;
    using Ensage.Common.Extensions;

    internal class EvadableModifier
    {
        #region Constructors and Destructors

        public EvadableModifier(
            Team team,
            GetHeroType type,
            float maxRemainingTime = 0,
            int minStacks = 0,
            float maxDistanceToSource = 300,
            bool ignoreRemainingTime = false)
        {
            Team = team;
            Type = type;
            IgnoreRemainingTime = ignoreRemainingTime;
            MaximumRemainingTime = maxRemainingTime;
            MinimumStackCount = minStacks;
            MaximumDistanceToSource = maxDistanceToSource;
        }

        #endregion

        #region Enums

        public enum GetHeroType
        {
            ModifierSource,

            LowestHealth,

            LowestHealthPct,

            ClosestToSource,
        }

        #endregion

        #region Public Properties

        public float AddedTime { get; set; }

        public List<string> AllyCounterAbilities { get; } = new List<string>();

        public List<string> EnemyCounterAbilities { get; } = new List<string>();

        public uint Handle { get; protected set; }

        public bool IgnoreRemainingTime { get; protected set; }

        #endregion

        #region Properties

        protected Hero Hero => Variables.Hero;

        protected Team HeroTeam => Variables.HeroTeam;

        protected float MaximumDistanceToSource { get; }

        protected float MaximumRemainingTime { get; }

        protected int MinimumStackCount { get; }

        protected Modifier Modifier { get; set; }

        protected Hero Source { get; set; }

        protected Team Team { get; set; }

        protected GetHeroType Type { get; set; }

        #endregion

        #region Public Methods and Operators

        public virtual void Add(Modifier modifier, Hero hero)
        {
            if (hero.Team != Team || Modifier != null && Modifier.IsValid)
            {
                return;
            }

            Handle = modifier.Handle;
            Modifier = modifier;
            Source = hero;
            AddedTime = Game.RawGameTime;
        }

        public virtual bool CanBeCountered()
        {
            try
            {
                if (!IsValid())
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            if (MaximumRemainingTime > 0)
            {
                return Modifier.RemainingTime <= MaximumRemainingTime;
            }

            if (MinimumStackCount > 0)
            {
                return Modifier.StackCount >= MinimumStackCount;
            }

            return true;
        }

        public Hero GetAllyHero(ParallelQuery<Hero> allies)
        {
            switch (Type)
            {
                case GetHeroType.ModifierSource:
                    return allies.FirstOrDefault(x => x.Equals(Source));
                case GetHeroType.LowestHealthPct:
                    return
                        allies.Where(x => x.HasModifier(Modifier.Name))
                            .OrderByDescending(x => x.Equals(Hero))
                            .ThenBy(x => x.Health / x.MaximumHealth)
                            .FirstOrDefault();
                case GetHeroType.LowestHealth:
                    return
                        allies.Where(x => x.HasModifier(Modifier.Name))
                            .OrderByDescending(x => x.Equals(Hero))
                            .ThenBy(x => x.Health)
                            .FirstOrDefault();
                case GetHeroType.ClosestToSource:
                    return
                        allies.Where(x => x.Distance2D(Source) <= MaximumDistanceToSource)
                            .OrderBy(x => x.Distance2D(Source))
                            .FirstOrDefault();
            }

            return null;
        }

        public float GetElapsedTime()
        {
            return Game.RawGameTime - AddedTime;
            //return Modifier.ElapsedTime;
        }

        public virtual Hero GetEnemyHero()
        {
            return Source;
        }

        public virtual float GetRemainingTime()
        {
            return !IsValid() ? 0 : Modifier.RemainingTime;
        }

        public Hero GetSource()
        {
            return Source;
        }

        public bool IsValid()
        {
            return Modifier != null && Modifier.IsValid;
        }

        public void Remove()
        {
            Modifier = null;
            Source = null;
        }

        #endregion
    }
}