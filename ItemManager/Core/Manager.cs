namespace ItemManager.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Abilities.Base;

    using Attributes;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    using EventArgs;

    internal class Manager : IDisposable
    {
        private readonly List<Type> types;

        public Manager()
        {
            MyHero = new MyHero(ObjectManager.LocalHero);

            types = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.Namespace == "ItemManager.Core.Abilities")
                .ToList();

            DelayAction.Add(3000, AddCurrentObjects);
        }

        public event EventHandler<AbilityEventArgs> OnAbilityAdd;

        public event EventHandler<AbilityEventArgs> OnAbilityRemove;

        public event EventHandler<ItemEventArgs> OnItemAdd;

        public event EventHandler<ItemEventArgs> OnItemRemove;

        public event EventHandler<UnitEventArgs> OnUnitAdd;

        public event EventHandler<UnitEventArgs> OnUnitRemove;

        public MyHero MyHero { get; }

        public List<Unit> Units { get; } = new List<Unit>();

        public void Dispose()
        {
            ObjectManager.OnAddEntity -= OnAddEntity;
            ObjectManager.OnRemoveEntity -= OnRemoveEntity;

            MyHero.Dispose();
            Units.Clear();
            types.Clear();
        }

        private void AddCurrentObjects()
        {
            var added = new List<uint>();

            foreach (var hero in ObjectManager.GetEntitiesParallel<Player>()
                .Where(x => x != null && x.IsValid && x.Hero != null && x.Hero.IsValid)
                .Select(x => x.Hero)
                .OrderByDescending(x => x.Team == MyHero.Team))
            {
                OnAddEntity(new EntityEventArgs(hero));
                added.Add(hero.Handle);

                var abilities = new List<Ability>();

                abilities.AddRange(hero.Spellbook.Spells);
                abilities.AddRange(hero.Inventory.Items);
                abilities.AddRange(hero.Inventory.Stash);
                abilities.AddRange(hero.Inventory.Backpack);

                foreach (var ability in abilities)
                {
                    OnAddEntity(new EntityEventArgs(ability));
                    added.Add(ability.Handle);
                }
            }

            foreach (var unit in ObjectManager.GetEntitiesParallel<Unit>()
                .Where(x => !added.Contains(x.Handle) && x.IsValid))
            {
                OnAddEntity(new EntityEventArgs(unit));
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
                    MyHero.Abilities.Add(ability);

                    var type = types.FirstOrDefault(
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
                    MyHero.Items.Add(item);
                }

                OnItemAdd?.Invoke(null, new ItemEventArgs(item, isMine));
                return;
            }

            var unit = args.Entity as Unit;
            if (unit != null && unit.IsValid && unit.IsRealUnit() && !(unit is Building))
            {
                Units.Add(unit);
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
                    MyHero.Abilities.Remove(ability);
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
                    MyHero.Items.Remove(item);
                }

                OnItemRemove?.Invoke(null, new ItemEventArgs(item, isMine));
                return;
            }

            var unit = args.Entity as Unit;
            if (unit != null && unit.IsValid && unit.IsRealUnit() && !(unit is Building))
            {
                Units.Remove(unit);
                OnUnitRemove?.Invoke(null, new UnitEventArgs(unit));
            }
        }
    }
}