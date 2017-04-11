namespace ItemManager.Core.Modules.HpMpAbuse.Items
{
    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using Menus;

    using Utils;

    internal abstract class UsableItem
    {
        protected static Hero Hero;

        protected UsableItem(string name)
        {
            Name = name;
            Hero = ObjectManager.LocalHero;
        }

        public Item Item { get; set; }

        protected static MenuManager Menu => HpMpAbuse.Menu;

        protected static MultiSleeper Sleeper => HpMpAbuse.Sleeper;

        protected virtual float HealthRestore { get; set; }

        protected virtual float ManaRestore { get; set; }

        protected string Name { get; }

        public virtual bool CanBeCasted()
        {
            return !Sleeper.Sleeping(Name) && Item != null && Item.CanBeCasted();
        }

        public virtual void FindItem()
        {
            Item = Hero.FindItem(Name);
        }

        public virtual ItemUtils.Stats GetDropItemStats()
        {
            var stats = ItemUtils.Stats.None;

            if (HealthRestore > 0 && Hero.MaximumHealth - Hero.Health > HealthRestore)
            {
                stats |= ItemUtils.Stats.Health;
            }
            if (ManaRestore > 0 && Hero.MaximumMana - Hero.Mana > ManaRestore)
            {
                stats |= ItemUtils.Stats.Mana;
            }

            return stats == ItemUtils.Stats.All ? ItemUtils.Stats.Any : stats;
        }

        public abstract Attribute GetPowerTreadsAttribute();

        public virtual void Use(bool queue = true)
        {
            Item.UseAbility(queue);
            Sleeper.Sleep(1000, Name);
        }
    }
}