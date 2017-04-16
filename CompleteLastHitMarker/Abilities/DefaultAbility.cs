namespace CompleteLastHitMarker.Abilities
{
    using System.Linq;

    using Ensage;

    using Units.Base;

    internal abstract class DefaultAbility
    {
        protected Ability Ability;

        protected DefaultAbility(Ability ability)
        {
            Ability = ability;
            AbilityId = ability.Id;
            DamageType = ability.DamageType;
            Handle = ability.Handle;
            Name = ability.Name;
            IsItem = ability is Item;
        }

        public AbilityId AbilityId { get; }

        public DamageType DamageType { get; protected set; }

        public uint Handle { get; }

        public uint Level => Ability.Level;

        public string Name { get; }

        protected float[] Damage { get; } = new float[8];

        protected bool IsItem { get; }

        public virtual bool IsValid(Hero hero, KillableUnit unit)
        {
            return Ability != null && Ability.IsValid && Ability.Level > 0
                   && (!IsItem || hero.Inventory.Items.Any(x => x.Handle == Ability.Handle));
        }
    }
}