﻿namespace Evader.UsableAbilities.Base
{
    using Core;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using EvadableAbilities.Base;

    using Utils;

    using AbilityType = Core.AbilityType;

    internal abstract class UsableAbility
    {
        #region Fields

        protected Sleeper Sleeper;

        #endregion

        #region Constructors and Destructors

        protected UsableAbility(Ability ability, AbilityType type, AbilityFlags flags)
        {
            Ability = ability;
            CastPoint = (float)ability.FindCastPoint();
            Name = ability.Name;
            Sleeper = new Sleeper();
            Handle = ability.Handle;
            IsItem = ability is Item;
            Type = type;
            ClassID = ability.ClassID;
            IgnoresLinkensSphere = ability.IsAbilityBehavior(AbilityBehavior.NoTarget)
                                   || ability.IsAbilityBehavior(AbilityBehavior.AreaOfEffect);
            PiercesMagicImmunity = ability.PiercesMagicImmunity();
            BasicDispel = flags.HasFlag(AbilityFlags.BasicDispel);
            CanBeUsedOnAlly = flags.HasFlag(AbilityFlags.CanBeCastedOnAlly);
            TargetEnemy = flags.HasFlag(AbilityFlags.TargetEnemy);
            StrongDispel = flags.HasFlag(AbilityFlags.StrongDispel);

            Debugger.WriteLine("///////// " + Name);
            Debugger.WriteLine("// Type: " + Type);
            Debugger.WriteLine("// Cast point: " + CastPoint);
            Debugger.WriteLine("// Cast range: " + Ability.GetRealCastRange());
            Debugger.WriteLine("// Ability flags: " + flags);
            Debugger.WriteLine("// Ignores linkens sphere: " + IgnoresLinkensSphere);
            Debugger.WriteLine("// Pierces magic immunity: " + PiercesMagicImmunity);
            if (type == AbilityType.Counter)
            {
                Debugger.WriteLine("// Can be used on ally: " + CanBeUsedOnAlly);
                Debugger.WriteLine("// Strong dispel: " + StrongDispel);
                Debugger.WriteLine("// Basic dispel: " + BasicDispel);
            }
        }

        #endregion

        #region Public Properties

        public bool BasicDispel { get; }

        public bool CanBeUsedOnAlly { get; }

        public ClassID ClassID { get; }

        public uint Handle { get; }

        public bool IgnoresLinkensSphere { get; set; }

        public bool IsItem { get; }

        public string Name { get; }

        public bool StrongDispel { get; }

        public bool TargetEnemy { get; }

        public AbilityType Type { get; protected set; }

        #endregion

        #region Properties

        protected static Team HeroTeam => Variables.HeroTeam;

        protected Ability Ability { get; }

        protected float CastPoint { get; set; }

        protected Hero Hero => Variables.Hero;

        protected bool PiercesMagicImmunity { get; set; }

        #endregion

        #region Public Methods and Operators

        public virtual bool CanBeCasted(Unit unit)
        {
            return !Sleeper.Sleeping && Ability.CanBeCasted() && Hero.Distance2D(unit) <= GetCastRange()
                   && CheckEnemy(unit);
        }

        public abstract float GetRequiredTime(EvadableAbility ability, Unit unit);

        public abstract void Use(EvadableAbility ability, Unit target);

        #endregion

        #region Methods

        protected virtual bool CheckEnemy(Unit unit)
        {
            return !unit.IsInvul()
                   && (CanBeUsedOnAlly || unit.Equals(Hero)
                       || ((PiercesMagicImmunity || !unit.IsMagicImmune())
                           && (IgnoresLinkensSphere || !unit.IsLinkensProtected())));
        }

        protected virtual float GetCastRange()
        {
            return Ability.GetCastRange();
        }

        protected void Sleep(float time = 1000)
        {
            Sleeper.Sleep(time);
        }

        #endregion
    }
}