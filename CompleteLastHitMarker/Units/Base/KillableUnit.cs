namespace CompleteLastHitMarker.Units.Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abilities.Interfaces;

    using Core;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    using Menus.Abilities;

    using SharpDX;

    using Utils;

    internal abstract class KillableUnit
    {
        protected KillableUnit(Unit unit)
        {
            Unit = unit;
            Handle = unit.Handle;
            Team = unit.Team;
            ArmorType = unit.ArmorType;
        }

        public bool AbilityDamageCalculated { get; protected set; }

        public Dictionary<IActiveAbility, float> ActiveAbilities { get; } = new Dictionary<IActiveAbility, float>();

        public ArmorType ArmorType { get; }

        public bool DamageCalculated { get; protected set; }

        public float DefaultTextureY { get; protected set; }

        public uint Handle { get; }

        public uint Health => Unit.Health;

        public float HealthPercentage => (float)Unit.Health / Unit.MaximumHealth;

        public abstract Vector2 HpBarPositionFix { get; }

        public Vector2 HpBarPosition
        {
            get
            {
                var position = HUDInfo.GetHPbarPosition(Unit);
                if (position.IsZero)
                {
                    return Vector2.Zero;
                }

                return position + HpBarPositionFix;
            }
        }

        public Vector2 HpBarSize { get; protected set; }

        public float Mana => Unit.Mana;

        public float MaxHealth => Unit.MaximumHealth;

        public IEnumerable<Modifier> Modifiers => Unit.Modifiers;

        public float MyAutoAttackDamageDone { get; protected set; }

        public Team Team { get; }

        public int TowerHelperHits { get; set; }

        public Unit Unit { get; }

        public UnitType UnitType { get; protected set; }

        public virtual void CalculateAbilityDamageTaken(MyHero hero, AbilitiesMenu menu)
        {
            if (hero.Team == Team)
            {
                return;
            }

            ActiveAbilities.Clear();

            foreach (var ability in hero.GetValidAbilities(this)
                .Where(x => menu.IsAbilityEnabled(x.Name))
                .OrderByDescending(x => menu.GetAbilityPriority(x.Name)))
            {
                ActiveAbilities[ability] = ability.CalculateDamage(hero.Hero, Unit);
            }

            if (ActiveAbilities.Any())
            {
                AbilityDamageCalculated = true;
            }
        }

        public void CalculateAutoAttackDamageTaken(MyHero hero)
        {
            var tempDamage = 0f;
            var multiplier = Damage.Multiplier(hero.AttackDamageType, ArmorType);

            foreach (var damage in hero.GetAutoAttackDamage(this))
            {
                if (damage.Key == DamageType.Physical)
                {
                    tempDamage += Unit.DamageTaken(damage.Value, damage.Key, hero.Hero) * multiplier;
                }
                else
                {
                    tempDamage += Unit.SpellDamageTaken(damage.Value, damage.Key, hero.Hero, string.Empty);
                }
            }

            MyAutoAttackDamageDone = (float)Math.Round(tempDamage);
            DamageCalculated = true;
        }

        public float Distance(MyHero hero)
        {
            return hero.Position.Distance2D(Unit);
        }

        public float Distance(Vector3 position)
        {
            return position.Distance2D(Unit);
        }

        public bool IsValid()
        {
            return Unit != null && Unit.IsValid && Unit.IsSpawned && Unit.IsAlive && Unit.IsVisible;
        }
    }
}