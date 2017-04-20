namespace CompleteLastHitMarker.Abilities.Active.Base
{
    using System;

    using Ensage;
    using Ensage.Common.AbilityInfo;
    using Ensage.Common.Extensions;

    using Interfaces;

    using Units.Base;

    using Utils;

    internal class DefaultActive : DefaultAbility, IActiveAbility
    {
        public DefaultActive(Ability ability)
            : base(ability)
        {
            Texture = Drawing.GetTexture("materials/ensage_ui/spellicons/" + Name);

            if (ability.IsAbilityBehavior(AbilityBehavior.Attack))
            {
                DealsAutoAttackDamage = true;
            }
        }

        public bool DealsAutoAttackDamage { get; protected set; }

        public float ManaCost => Ability.ManaCost;

        public DotaTexture Texture { get; }

        public virtual float CalculateDamage(Hero source, Unit target)
        {
            return (float)Math.Round(AbilityDamage.CalculateDamage(Ability, source, target));
        }

        public bool CanBeCasted()
        {
            return Ability.CanBeCasted() && !Ability.IsHidden;
        }

        public override bool IsValid(Hero hero, KillableUnit unit)
        {
            return base.IsValid(hero, unit) && (DealsDamageToTowers || unit.UnitType != UnitType.Tower);
        }
    }
}