namespace Evader.UsableAbilities.Base
{
    using System.Linq;

    using Common;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class TargetBlink : UsableAbility
    {
        private Unit blinkUnit;

        public TargetBlink(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
        }

        public override bool CanBeCasted(EvadableAbility ability, Unit unit)
        {
            if (Hero.IsRuptured())
            {
                return false;
            }

            blinkUnit = ObjectManager.GetEntitiesFast<Unit>()
                .FirstOrDefault(
                    x => x.IsValid && x.IsAlive && (x is Creep || x is Hero) && !x.Equals(Hero) && x.Team == HeroTeam
                         && x.Distance2D(Hero) < GetCastRange() && x.Distance2D(Hero) > 200);

            return !Sleeper.Sleeping && blinkUnit != null && Ability.CanBeCasted() && Hero.CanCast();
        }

        public override float GetRequiredTime(EvadableAbility ability, Unit unit, float remainingTime)
        {
            if (blinkUnit == null)
            {
                return float.MaxValue;
            }

            return CastPoint + (float)Hero.GetTurnTime(blinkUnit);
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            if (blinkUnit == null)
            {
                return;
            }

            Ability.UseAbility(blinkUnit, false, true);
            Sleep();
        }
    }
}