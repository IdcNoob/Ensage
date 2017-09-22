namespace ItemManager.Core.Abilities
{
    using Attributes;

    using Base;

    using Ensage;

    using Utils;

    [Ability(AbilityId.item_power_treads)]
    internal class PowerTreads : UsableAbility
    {
        private readonly Ensage.Items.PowerTreads powerTreads;

        public PowerTreads(Ability ability, Manager manager)
            : base(ability, manager)
        {
            powerTreads = (Ensage.Items.PowerTreads)ability;
            DefaultAttribute = powerTreads.ActiveAttribute;
        }

        public Attribute ActiveAttribute => powerTreads.ActiveAttribute;

        public Attribute DefaultAttribute { get; private set; }

        public override bool CanBeCasted()
        {
            return base.CanBeCasted() && !Manager.MyHero.HasModifier(ModifierUtils.IceBlastDebuff);
        }

        public void ChangeDefaultAttribute(Attribute? attribute = null)
        {
            if (attribute == null)
            {
                switch (powerTreads.ActiveAttribute)
                {
                    case Attribute.Strength:
                        DefaultAttribute = Attribute.Intelligence;
                        break;
                    case Attribute.Intelligence:
                        DefaultAttribute = Attribute.Agility;
                        break;
                    case Attribute.Agility:
                        DefaultAttribute = Attribute.Strength;
                        break;
                }
            }
            else
            {
                DefaultAttribute = attribute.Value;
            }

            SetSleep(200);
        }

        public void SwitchTo(Attribute attribute, bool queue = false)
        {
            SetSleep(200);

            switch (attribute)
            {
                case Attribute.Strength:
                    if (ActiveAttribute == Attribute.Intelligence)
                    {
                        powerTreads.UseAbility(queue);
                        powerTreads.UseAbility(queue);
                    }
                    else if (ActiveAttribute == Attribute.Agility)
                    {
                        powerTreads.UseAbility(queue);
                    }
                    break;
                case Attribute.Intelligence:
                    if (ActiveAttribute == Attribute.Agility)
                    {
                        powerTreads.UseAbility(queue);
                        powerTreads.UseAbility(queue);
                    }
                    else if (ActiveAttribute == Attribute.Strength)
                    {
                        powerTreads.UseAbility(queue);
                    }
                    break;
                case Attribute.Agility:
                    if (ActiveAttribute == Attribute.Strength)
                    {
                        powerTreads.UseAbility(queue);
                        powerTreads.UseAbility(queue);
                    }
                    else if (ActiveAttribute == Attribute.Intelligence)
                    {
                        powerTreads.UseAbility(queue);
                    }
                    break;
            }
        }

        public override void Use(Unit target = null, bool queue = false)
        {
        }
    }
}