namespace HpMpAbuse.Items
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;

    using Helpers;

    internal sealed class Bottle : UsableItem
    {
        #region Fields

        private readonly float restoreTime;

        #endregion

        #region Constructors and Destructors

        public Bottle(string name)
            : base(name)
        {
            ManaRestore =
                Ability.GetAbilityDataByName(Name).AbilitySpecialData.First(x => x.Name == "mana_restore").Value;

            HealthRestore =
                Ability.GetAbilityDataByName(Name).AbilitySpecialData.First(x => x.Name == "health_restore").Value;

            restoreTime =
                Ability.GetAbilityDataByName(Name).AbilitySpecialData.First(x => x.Name == "restore_time").Value;
        }

        #endregion

        #region Public Properties

        public float CastRange => Item.GetCastRange();

        #endregion

        #region Public Methods and Operators

        public bool CanBeAutoCasted()
        {
            return base.CanBeCasted() && Item.CurrentCharges > 0 && Menu.Recovery.BottleAtFountain
                   && !Hero.IsChanneling();
        }

        public override bool CanBeCasted()
        {
            var bottleRegen = Hero.FindModifier(Modifiers.BottleRegeneration);
            return base.CanBeCasted() && Menu.Recovery.IsEnabled(Name) && Item.CurrentCharges > 0
                   && (bottleRegen == null || bottleRegen.RemainingTime < Game.Ping / 1000
                       || Sleeper.Sleeping("AutoBottle"))
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

        public void Use(Hero target)
        {
            if (target == null)
            {
                Item.UseAbility();
            }
            else
            {
                Item.UseAbility(target);
            }
            Sleeper.Sleep(restoreTime * 1000, "AutoBottle");
            Sleeper.Sleep(300, "Used");
            SetSleep(200 + Game.Ping);
        }

        #endregion
    }
}