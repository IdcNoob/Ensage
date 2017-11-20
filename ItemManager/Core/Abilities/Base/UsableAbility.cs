namespace ItemManager.Core.Abilities.Base
{
    using System.Linq;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using Utils;

    [Ability(AbilityId.item_phase_boots)]
    [Ability(AbilityId.treant_living_armor)]
    [Ability(AbilityId.item_hand_of_midas)]
    [Ability(AbilityId.item_cheese)]
    [Ability(AbilityId.item_faerie_fire)]
    [Ability(AbilityId.item_dust)]
    internal class UsableAbility
    {
        protected Ability Ability;

        protected Manager Manager;

        protected Sleeper Sleeper = new Sleeper();

        public UsableAbility(Ability ability, Manager manager)
        {
            Manager = manager;

            if (ability == null)
            {
                //shrine
                Name = "filler_ability";
                return;
            }

            Ability = ability;
            IsItem = ability is Item;
            Handle = ability.Handle;
            Id = ability.Id;
            Name = ability.Name;
            CastPoint = ability.GetCastPoint(0) * 1000;
        }

        public uint Handle { get; }

        public AbilityId Id { get; }

        public bool IsItem { get; }

        public bool IsSleeping => Sleeper.Sleeping;

        public string Name { get; protected set; }

        protected float CastPoint { get; }

        public virtual bool CanBeCasted()
        {
            return !IsSleeping && Ability.IsValid && Ability.CanBeCasted()
                   && (!IsItem || Manager.MyHero.GetItems(ItemStoredPlace.Inventory).Any(x => x.Handle == Handle)
                       && ((Item)Ability).IsEnabled);
        }

        public void ChangeName(string name)
        {
            Name = name;
        }

        public virtual float GetCastRange()
        {
            return Ability.GetCastRange();
        }

        public void SetSleep(float time)
        {
            Sleeper.Sleep(time);
        }

        public virtual void Use(Unit target = null, bool queue = false)
        {
            SetSleep(CastPoint + 200);

            if (Ability.IsAbilityBehavior(AbilityBehavior.NoTarget))
            {
                Ability.UseAbility(queue);
            }
            else if (Ability.IsAbilityBehavior(AbilityBehavior.UnitTarget))
            {
                Ability.UseAbility(target ?? Manager.MyHero.Hero, queue);
            }
        }
    }
}