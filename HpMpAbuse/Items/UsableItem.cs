namespace HpMpAbuse.Items
{
    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using Helpers;

    using Menu;

    internal abstract class UsableItem
    {
        #region Constructors and Destructors

        protected UsableItem(string name)
        {
            Name = name;
        }

        #endregion

        #region Public Properties

        public Item Item { get; set; }

        #endregion

        #region Properties

        protected static Hero Hero => Variables.Hero;

        protected static MenuManager Menu => Variables.Menu;

        protected static MultiSleeper Sleeper => Variables.Sleeper;

        protected virtual float HealthRestore { get; set; }

        protected virtual float ManaRestore { get; set; }

        protected string Name { get; }

        #endregion

        #region Public Methods and Operators

        public virtual bool CanBeCasted()
        {
            return !Sleeper.Sleeping(Name) && Item != null && Item.CanBeCasted();
        }

        public virtual void FindItem()
        {
            Item = Hero.FindItem(Name);
        }

        public virtual ItemsStats.Stats GetDropItemStats()
        {
            var stats = ItemsStats.Stats.None;

            if (HealthRestore > 0 && Hero.MaximumHealth - Hero.Health > HealthRestore)
            {
                stats |= ItemsStats.Stats.Health;
            }
            if (ManaRestore > 0 && Hero.MaximumMana - Hero.Mana > ManaRestore)
            {
                stats |= ItemsStats.Stats.Mana;
            }

            return stats == ItemsStats.Stats.All ? ItemsStats.Stats.Any : stats;
        }

        public abstract Attribute GetPowerTreadsAttribute();

        public virtual void Use(bool queue = true)
        {
            Item.UseAbility(queue);
            Sleeper.Sleep(1000, Name);
        }

        #endregion
    }
}