namespace Evader.UsableAbilities.Base
{
    using System.Linq;

    using Core;

    using Ensage;
    using Ensage.Common.Extensions;

    using EvadableAbilities.Base;

    using AbilityType = Core.AbilityType;

    internal class TargetBlink : UsableAbility
    {
        #region Fields

        private Unit blinkUnit;

        #endregion

        #region Constructors and Destructors

        public TargetBlink(Ability ability, AbilityType type, AbilityFlags flags = AbilityFlags.None)
            : base(ability, type, flags)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted(Unit unit)
        {
            blinkUnit =
                ObjectManager.GetEntitiesFast<Unit>()
                    .FirstOrDefault(
                        x =>
                        x.IsValid && x.IsAlive && (x is Creep || x is Hero) && !x.Equals(Hero) && x.Team == HeroTeam
                        && x.Distance2D(Hero) < GetCastRange() && x.Distance2D(Hero) > 200);
            return !Sleeper.Sleeping && blinkUnit != null && Ability.CanBeCasted();
        }

        public override float GetRequiredTime(EvadableAbility ability, Unit unit)
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

            Ability.UseAbility(blinkUnit);
            Sleep();
        }

        #endregion
    }
}