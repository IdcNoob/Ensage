namespace ItemManager.Core.Modules.HpMpAbuse.Items
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using Menus;

    using Utils;

    internal class PowerTreads
    {
        private Ensage.Items.PowerTreads powerTreads;

        public PowerTreads(string name)
        {
            Name = name;
        }

        public Attribute ActiveAttribute => powerTreads.ActiveAttribute;

        public Attribute DefaultAttribute { get; set; } = Attribute.Strength;

        protected static Hero Hero => ObjectManager.LocalHero;

        protected static MultiSleeper Sleeper => HpMpAbuse.Sleeper;

        private static MenuManager Menu => HpMpAbuse.Menu;

        private string Name { get; }

        public virtual bool CanBeCasted()
        {
            return !Sleeper.Sleeping(Name) && powerTreads != null;
        }

        public bool DelaySwitch()
        {
            return Hero.HasModifiers(
                ModifierUtils.DisablePowerTreadsSwitchBack.Concat(ModifierUtils.Invisibility).ToArray(),
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

        //public Attribute GetAttackAttribute()
        //{
        //    var swithOnAttackIndex = Menu.PowerTreads.SwitchOnAttack;
        //    if (swithOnAttackIndex != 0)
        //    {
        //        return Menu.PowerTreads.Attributes.ElementAt(swithOnAttackIndex).Value;
        //    }
        //    return ActiveAttribute;
        //}

        //public Attribute GetMoveAttribute()
        //{
        //    var swithOnMoveIndex = Menu.PowerTreads.SwitchOnMove;
        //    if (swithOnMoveIndex != 0)
        //    {
        //        return Menu.PowerTreads.Attributes.ElementAt(swithOnMoveIndex).Value;
        //    }
        //    return ActiveAttribute;
        //}

        public bool IsDisabled()
        {
            //var disable = Menu.PowerTreads.AutoDisableTime;
            //return !Menu.PowerTreads.Enabled || disable > 0 && Game.GameTime / 60 > disable;

            return !Menu.PowerTreadsSwitcherMenu.IsActive;
        }

        public bool IsValid()
        {
            return powerTreads != null && powerTreads.IsValid;
        }

        public void SwitchTo(Attribute attribute)
        {
            if (!CanBeCasted() || !IsValid() || IsDisabled() && !Menu.RecoveryMenu.IsActive)
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
    }
}