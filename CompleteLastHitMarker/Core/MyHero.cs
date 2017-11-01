namespace CompleteLastHitMarker.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Abilities;
    using Abilities.Active.Base;
    using Abilities.Interfaces;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.SDK.Helpers;

    using Menus;

    using SharpDX;

    using Units.Base;

    internal class MyHero
    {
        private readonly List<DefaultAbility> abilities = new List<DefaultAbility>();

        private readonly MenuManager menu;

        public MyHero(Hero hero, MenuManager menuManager, Type[] abilityTypes)
        {
            Hero = hero;
            Handle = hero.Handle;
            AttackDamageType = hero.AttackDamageType;
            Team = hero.Team;
            menu = menuManager;

            foreach (var ability in hero.Spellbook.Spells)
            {
                var type = abilityTypes.FirstOrDefault(x => x.GetCustomAttribute<AbilityAttribute>()?.AbilityId == ability.Id);

                if (type != null)
                {
                    AddNewAbility((DefaultAbility)Activator.CreateInstance(type, ability));
                }
                else if ((ability.IsNuke() || AbilityData.ForceInclude.Contains(ability.Id))
                         && !AbilityData.ForceExclude.Contains(ability.Id))
                {
                    AddNewAbility(new DefaultActive(ability));
                }
            }

            foreach (var ability in EntityManager<Item>.Entities.Where(
                x => x.IsValid && x.Purchaser?.Hero?.Handle == Handle && x.Id != AbilityId.ability_base))
            {
                var type = abilityTypes.FirstOrDefault(x => x.GetCustomAttribute<AbilityAttribute>()?.AbilityId == ability.Id);

                if (type != null)
                {
                    AddNewAbility((DefaultAbility)Activator.CreateInstance(type, ability));
                }
            }
        }

        public AttackDamageType AttackDamageType { get; }

        public int AutoAttackDamage => Hero.MinimumDamage + Hero.BonusDamage;

        public uint Handle { get; }

        public Hero Hero { get; }

        public bool IsAlive => Hero.IsAlive;

        public Vector3 Position => Hero.Position;

        public Team Team { get; }

        public void AddNewAbility(DefaultAbility ability)
        {
            if (abilities.Any(x => x.Handle == ability.Handle))
            {
                return;
            }

            abilities.Add(ability);

            if (ability is IActiveAbility)
            {
                menu.AbilitiesMenu.AddAbility(ability.Name);
            }
        }

        public Dictionary<DamageType, float> GetAutoAttackDamage(KillableUnit unit)
        {
            var damages = new Dictionary<DamageType, float>();
            var passiveAbilities = abilities.OfType<IPassiveAbility>().Where(x => x.IsValid(Hero, unit)).ToList();

            foreach (var ability in passiveAbilities)
            {
                var damage = ability.GetBonusDamage(Hero, unit, passiveAbilities);

                if (damages.ContainsKey(ability.DamageType))
                {
                    damages[ability.DamageType] += damage;
                }
                else
                {
                    damages.Add(ability.DamageType, damage);
                }
            }

            if (damages.ContainsKey(DamageType.Physical))
            {
                damages[DamageType.Physical] += AutoAttackDamage;
            }
            else
            {
                damages.Add(DamageType.Physical, AutoAttackDamage);
            }

            return damages;
        }

        public IEnumerable<IActiveAbility> GetValidAbilities(KillableUnit unit)
        {
            return abilities.OfType<IActiveAbility>().Where(x => x.IsValid(Hero, unit));
        }

        public void RemoveAbility(uint handle)
        {
            abilities.RemoveAll(x => x.Handle == handle);
        }
    }
}