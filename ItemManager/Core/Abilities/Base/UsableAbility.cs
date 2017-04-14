namespace ItemManager.Core.Abilities.Base
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using Utils;

    internal class UsableAbility
    {
        protected Ability Ability;

        protected Manager Manager;

        protected Sleeper Sleeper = new Sleeper();

        public UsableAbility(Ability ability, Manager manager)
        {
            Manager = manager;
            Ability = ability;
            IsItem = ability is Item;
            Handle = ability.Handle;
            Id = ability.Id;
            Name = ability.Name;
        }

        public uint Handle { get; }

        public AbilityId Id { get; }

        public bool IsItem { get; }

        public bool IsSleeping => Sleeper.Sleeping;

        public string Name { get; }

        public virtual bool CanBeCasted()
        {
            return !IsSleeping && Ability.IsValid && Ability.CanBeCasted() && Manager
                       .GetMyItems(ItemUtils.StoredPlace.Inventory)
                       .Any(x => x.Handle == Handle);
        }

        public float GetCastRange()
        {
            return Ability.GetCastRange();
        }

        public void SetSleep(float time)
        {
            Sleeper.Sleep(time);
        }

        public virtual void Use(bool queue = false)
        {
            if (Ability.IsAbilityBehavior(AbilityBehavior.NoTarget))
            {
                if (!Ability.UseAbility(queue))
                {
                    return;
                }
            }
            else if (Ability.IsAbilityBehavior(AbilityBehavior.UnitTarget))
            {
                if (!Ability.UseAbility(Manager.MyHero, queue))
                {
                    return;
                }
            }

            SetSleep(200);
        }
    }
}