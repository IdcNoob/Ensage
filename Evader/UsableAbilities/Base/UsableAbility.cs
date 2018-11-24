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
        protected Sleeper Sleeper;

        protected UsableAbility(Ability ability, AbilityType type, AbilityCastTarget target)
        {
            Sleeper = new Sleeper();
            Type = type;

            if (ability == null)
            {
                //Gold spender
                return;
            }

            Ability = ability;
            CastPoint = (float)ability.FindCastPoint();
            Name = ability.Name;
            Handle = ability.Handle;
            IsItem = ability is Item;
            AbilityId = ability.Id;
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

        public bool CanBeUsedOnAlly { get; }

        public bool CanBeUsedOnEnemy { get; }

        public AbilityId AbilityId { get; }

        public uint Handle { get; protected set; }

        public bool IgnoresLinkensSphere { get; protected set; }

        public bool IsItem { get; protected set; }

        public string Name { get; protected set; }

        public AbilityType Type { get; protected set; }

        protected static Team HeroTeam => Variables.HeroTeam;

        protected Ability Ability { get; }

        protected float CastPoint { get; set; }

        protected Hero Hero => Variables.Hero;

        protected bool PiercesMagicImmunity { get; set; }

        public virtual bool CanBeCasted(EvadableAbility ability, Unit unit)
        {
            return !Sleeper.Sleeping && Ability.CanBeCasted()
                   && (IsItem && Hero.CanUseItems() || !IsItem && Hero.CanCast())
                   && Hero.Distance2D(unit) <= GetCastRange() && CheckEnemy(unit);
        }

        public abstract float GetRequiredTime(EvadableAbility ability, Unit unit, float remainingTime);

        public abstract void Use(EvadableAbility ability, Unit target);

        public virtual bool UseCustomSleep(out float sleepTime)
        {
            sleepTime = 0;
            return false;
        }

        protected virtual bool CheckEnemy(Unit unit)
        {
            return !unit.IsInvul() && (CanBeUsedOnAlly || unit.Equals(Hero)
                                       || (PiercesMagicImmunity || !unit.IsMagicImmune())
                                       && (IgnoresLinkensSphere || !unit.IsLinkensProtected()));
        }

        protected virtual float GetCastRange()
        {
            return Ability.GetCastRange();
        }

        protected void Sleep(float time = 1000)
        {
            Sleeper.Sleep(time);
        }
    }
}