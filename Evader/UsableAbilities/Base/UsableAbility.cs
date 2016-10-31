namespace Evader.UsableAbilities.Base
{
    using Common;

    using Core;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal abstract class UsableAbility
    {
        #region Fields

        protected Sleeper Sleeper;

        #endregion

        #region Constructors and Destructors

        protected UsableAbility(Ability ability, AbilityType type, AbilityCastTarget target)
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
            CanBeUsedOnEnemy = target.HasFlag(AbilityCastTarget.Enemy) || type == AbilityType.Disable;
            CanBeUsedOnAlly = target.HasFlag(AbilityCastTarget.Ally);

            Debugger.WriteLine("///////// UsableAbility // " + Name);
            Debugger.WriteLine("// Type: " + Type);
            Debugger.WriteLine("// Cast point: " + CastPoint);
            Debugger.WriteLine("// Cast range: " + Ability.GetRealCastRange());
            Debugger.WriteLine("// Ignores linkens sphere: " + IgnoresLinkensSphere);
            Debugger.WriteLine("// Pierces magic immunity: " + PiercesMagicImmunity);
            Debugger.WriteLine("// Can be used on ally: " + CanBeUsedOnAlly);
            Debugger.WriteLine("// Can be used on enemy: " + CanBeUsedOnEnemy);
        }

        #endregion

        #region Public Properties

        public bool CanBeUsedOnAlly { get; }

        public bool CanBeUsedOnEnemy { get; }

        public ClassID ClassID { get; }

        public uint Handle { get; }

        public bool IgnoresLinkensSphere { get; set; }

        public bool IsItem { get; }

        public string Name { get; }

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

        public virtual bool CanBeCasted(EvadableAbility ability, Unit unit)
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