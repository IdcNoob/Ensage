namespace HpMpAbuse.Items
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using Helpers;

    using Menu;

    internal class PowerTreads
    {
        #region Fields

        private Ensage.Items.PowerTreads powerTreads;

        #endregion

        #region Constructors and Destructors

        public PowerTreads(string name)
        {
            Name = name;
        }

        #endregion

        #region Public Properties

        public Attribute ActiveAttribute => powerTreads.ActiveAttribute;

        public Attribute DefaultAttribute { get; set; } = Attribute.Strength;

        #endregion

        #region Properties

        protected static Hero Hero => Variables.Hero;

        protected static MultiSleeper Sleeper => Variables.Sleeper;

        private static MenuManager Menu => Variables.Menu;

        private string Name { get; }

        #endregion

        #region Public Methods and Operators

        public virtual bool CanBeCasted()
        {
            return !Sleeper.Sleeping(Name) && powerTreads != null;
        }

        public bool DelaySwitch()
        {
            return Hero.HasModifiers(
                Modifiers.DisablePowerTreadsSwitchBack.Concat(Modifiers.Invisibility).ToArray(),
                false);
        }

        public bool Equals(Ability ability)
        {
            return ability.Equals(powerTreads);
        }

        public void FindItem()
        {
            powerTreads = (Ensage.Items.PowerTreads)Hero.FindItem(Name);
        }

        public Attribute GetAttackAttribute()
        {
            var swithOnAttackIndex = Menu.PowerTreads.SwitchOnAttack;
            if (swithOnAttackIndex != 0)
            {
                return Menu.PowerTreads.Attributes.ElementAt(swithOnAttackIndex).Value;
            }
            return ActiveAttribute;
        }

        public Attribute GetMoveAttribute()
        {
            var swithOnMoveIndex = Menu.PowerTreads.SwitchOnMove;
            if (swithOnMoveIndex != 0)
            {
                return Menu.PowerTreads.Attributes.ElementAt(swithOnMoveIndex).Value;
            }
            return ActiveAttribute;
        }

        public bool IsDisabled()
        {
            var disable = Menu.PowerTreads.AutoDisableTime;
            return !Menu.PowerTreads.Enabled || disable > 0 && Game.GameTime / 60 > disable;
        }

        public bool IsValid()
        {
            return powerTreads != null && powerTreads.IsValid;
        }

        public void SwitchTo(Attribute attribute)
        {
            if (!CanBeCasted() || (IsDisabled() && !Menu.Recovery.Active))
            {
                return;
            }

            var currentAttribute = powerTreads.ActiveAttribute;

            if (attribute == Attribute.Invalid)
            {
                attribute = Hero.PrimaryAttribute;
            }

            switch (attribute)
            {
                case Attribute.Agility:
                    if (currentAttribute == Attribute.Strength)
                    {
                        powerTreads.UseAbility();
                        powerTreads.UseAbility();
                    }
                    else if (currentAttribute == Attribute.Intelligence)
                    {
                        powerTreads.UseAbility();
                    }
                    break;
                case Attribute.Strength:
                    if (currentAttribute == Attribute.Intelligence)
                    {
                        powerTreads.UseAbility();
                        powerTreads.UseAbility();
                    }
                    else if (currentAttribute == Attribute.Agility)
                    {
                        powerTreads.UseAbility();
                    }
                    break;
                case Attribute.Intelligence:
                    if (currentAttribute == Attribute.Agility)
                    {
                        powerTreads.UseAbility();
                        powerTreads.UseAbility();
                    }
                    else if (currentAttribute == Attribute.Strength)
                    {
                        powerTreads.UseAbility();
                    }
                    break;
            }

            Sleeper.Sleep(Game.Ping + 150, Name);
        }

        #endregion
    }
}