namespace ItemManager.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Abilities.Base;

    using Attributes;

    using Ensage;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Helpers;

    using EventArgs;

    using AbilityEventArgs = EventArgs.AbilityEventArgs;

    internal class Manager : IDisposable
    {
        private readonly HashSet<Type> abilityTypes;

        public Manager(Hero hero)
        {
            MyHero = new MyHero(hero);
            MyHero.UsableAbilities.Add(new Shrine(this));

            abilityTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.IsClass && x.Namespace?.Contains("ItemManager.Core.Abilities") == true)
                .ToHashSet();

            UpdateManager.BeginInvoke(AddCurrentObjects, 3000);
        }

        public event EventHandler<AbilityEventArgs> OnAbilityAdd;

        public event EventHandler<AbilityEventArgs> OnAbilityRemove;

        public event EventHandler<ItemEventArgs> OnItemAdd;

        public event EventHandler<ItemEventArgs> OnItemRemove;

        public event EventHandler<UnitEventArgs> OnUnitAdd;

        public event EventHandler<UnitEventArgs> OnUnitRemove;

        public MyHero MyHero { get; }

        public void Dispose()
        {
            ObjectManager.OnAddEntity -= OnAddEntity;
            ObjectManager.OnRemoveEntity -= OnRemoveEntity;

            MyHero.Dispose();
        }

        private void AddCurrentObjects()
        {
            var entities = new List<Entity>();

            entities.AddRange(EntityManager<Unit>.Entities.Where(x => x.IsValid && x.UnitType != 0));
            entities.AddRange(EntityManager<Ability>.Entities.Where(x => x.IsValid));
            entities.AddRange(EntityManager<PhysicalItem>.Entities.Where(x => x.IsValid && x.Item.IsValid).Select(x => x.Item));

            foreach (var entity in entities)
            {
                OnAddEntity(new EntityEventArgs(entity));
            }

            ObjectManager.OnAddEntity += OnAddEntity;
            ObjectManager.OnRemoveEntity += OnRemoveEntity;
        }

        private void OnAddEntity(EntityEventArgs args)
        {
            var ability = args.Entity as Ability;
            if (ability != null && ability.IsValid && ability.Id != AbilityId.ability_base)
            {
                var isMine = ability.Owner?.Handle == MyHero.Handle;
                if (isMine)
                {
                    MyHero.AddAbility(ability);

                    var type = abilityTypes.FirstOrDefault(
                        x => x.GetCustomAttributes<AbilityAttribute>().Any(z => z.AbilityId == ability.Id));

                    if (type != null)
                    {
                        MyHero.UsableAbilities.Add((UsableAbility)Activator.CreateInstance(type, ability, this));
                    }
                }

                OnAbilityAdd?.Invoke(null, new AbilityEventArgs(ability, isMine));
            }

            var item = args.Entity as Item;
            if (item != null && item.IsValid && item.Id != AbilityId.ability_base)
            {
                var isMine = item.Purchaser?.Hero?.Handle == MyHero.Handle;
                if (isMine)
                {
                    MyHero.AddItem(item);
                }

                OnItemAdd?.Invoke(null, new ItemEventArgs(item, isMine));
                return;
            }

            var unit = args.Entity as Unit;
            if (unit != null && unit.IsValid && unit.IsRealUnit())
            {
                OnUnitAdd?.Invoke(null, new UnitEventArgs(unit));
            }
        }

        private void OnRemoveEntity(EntityEventArgs args)
        {
            var physItem = args.Entity as PhysicalItem;
            if (physItem != null && physItem.IsValid && physItem.Item?.Purchaser?.Hero?.Handle == MyHero.Handle)
            {
                MyHero.DroppedItems.Remove(physItem.Item);
                return;
            }

            var ability = args.Entity as Ability;
            if (ability != null && ability.IsValid)
            {
                var isMine = ability.Owner?.Handle == MyHero.Handle;
                if (isMine)
                {
                    MyHero.RemoveAbility(ability);
                    var usableAbility = MyHero.UsableAbilities.FirstOrDefault(x => x.Handle == ability.Handle);
                    if (usableAbility != null)
                    {
                        MyHero.UsableAbilities.Remove(usableAbility);
                    }
                }

                OnAbilityRemove?.Invoke(null, new AbilityEventArgs(ability, isMine));
            }

            var item = args.Entity as Item;
            if (item != null && item.IsValid)
            {
                var isMine = item.Purchaser?.Hero?.Handle == MyHero.Handle;
                if (isMine)
                {
                    MyHero.RemoveItem(item);
                }

                OnItemRemove?.Invoke(null, new ItemEventArgs(item, isMine));
                return;
            }

            var unit = args.Entity as Unit;
            if (unit != null && unit.IsValid && unit.IsRealUnit())
            {
                OnUnitRemove?.Invoke(null, new UnitEventArgs(unit));
            }
        }
    }
}