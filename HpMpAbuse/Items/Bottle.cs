namespace HpMpAbuse.Items
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;

    using HpMpAbuse.Helpers;

    internal sealed class Bottle : UsableItem
    {
        #region Constructors and Destructors

        public Bottle(string name)
            : base(name)
        {
            ManaRestore =
                Ability.GetAbilityDataByName(Name).AbilitySpecialData.First(x => x.Name == "mana_restore").Value;

            HealthRestore =
                Ability.GetAbilityDataByName(Name).AbilitySpecialData.First(x => x.Name == "health_restore").Value;
        }

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted()
        {
            var bottleRegen = Hero.FindModifier(Modifiers.BottleRegeneration);
            return base.CanBeCasted() && Item.CurrentCharges > 0
                   && (bottleRegen == null || bottleRegen.RemainingTime < Game.Ping / 1000)
                   && (Hero.Health < Hero.MaximumHealth || Hero.Mana < Hero.MaximumMana);
        }

        public override Attribute GetPowerTreadsAttribute()
        {
            var stats = GetDropItemStats();
            var attribute = Attribute.Invalid;

            if (stats.HasFlag(ItemsStats.Stats.Mana))
            {
                attribute = Hero.PrimaryAttribute == Attribute.Agility ? Attribute.Agility : Attribute.Strength;
            }
            else if (stats.HasFlag(ItemsStats.Stats.Health))
            {
                attribute = Hero.PrimaryAttribute == Attribute.Intelligence ? Attribute.Intelligence : Attribute.Agility;
            }
            else if (stats.HasFlag(ItemsStats.Stats.Any))
            {
                attribute = Attribute.Agility;
            }

            return attribute;
        }

        public void SetSleep(float sleep)
        {
            Sleeper.Sleep(sleep, Name);
        }

        #endregion
    }
}