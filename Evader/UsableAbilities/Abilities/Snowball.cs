namespace Evader.UsableAbilities.Abilities
{
    using System.Linq;

    using Base;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class Snowball : Targetable
    {
        #region Fields

        private readonly float grabRadius;

        #endregion

        #region Constructors and Destructors

        public Snowball(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
            grabRadius = Ability.AbilitySpecialData.First(x => x.Name == "snowball_grab_radius").Value;
        }

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted(EvadableAbility ability, Unit unit)
        {
            return !Sleeper.Sleeping && Ability.CanBeCasted() && Hero.CanCast() && Hero.Distance2D(unit) <= grabRadius
                   && CheckEnemy(unit);
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            Ability.UseAbility(ability.AbilityOwner);
            if (!target.Equals(Hero))
            {
                Hero.Attack(target);
            }
            Sleep();
        }

        #endregion
    }
}