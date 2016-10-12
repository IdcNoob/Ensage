namespace Evader.UsableAbilities.Abilities
{
    using Base;

    using Core;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using EvadableAbilities.Base;

    using AbilityType = Core.AbilityType;
    using Projectile = EvadableAbilities.Base.Projectile;

    internal class BallLightning : Targetable
    {
        #region Constructors and Destructors

        public BallLightning(Ability ability, AbilityType type, AbilityFlags flags = AbilityFlags.None)
            : base(ability, type, flags)
        {
            //todo improve
        }

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted(Unit unit)
        {
            return !Sleeper.Sleeping && Ability.CanBeCasted();
        }

        public override float GetRequiredTime(EvadableAbility ability, Unit unit)
        {
            var projectile = ability as Projectile;

            if (projectile != null && !projectile.IsDisjointable)
            {
                return CastPoint + (float)Hero.GetTurnTime(ability.AbilityOwner);
            }

            return CastPoint;
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            var projectile = ability as Projectile;

            if (projectile != null && !projectile.IsDisjointable)
            {
                if (!projectile.IsDisjointable)
                {
                    Ability.UseAbility(Hero.NetworkPosition.Extend(ability.AbilityOwner.Position, 250));
                }
                else
                {
                    Ability.UseAbility(Hero.InFront(50));
                }
            }
            else
            {
                Ability.UseAbility(Hero.InFront(150));
            }

            Sleep();
        }

        #endregion
    }
}