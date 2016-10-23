namespace Evader.UsableAbilities.Abilities
{
    using System.Linq;

    using Base;

    using Data;

    using Ensage;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class DarkRift : Targetable
    {
        #region Fields

        private readonly Unit fountain =
            ObjectManager.GetEntitiesFast<Unit>()
                .First(x => x.ClassID == ClassID.CDOTA_Unit_Fountain && x.Team == HeroTeam);

        #endregion

        #region Constructors and Destructors

        public DarkRift(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override void Use(EvadableAbility ability, Unit target)
        {
            Ability.UseAbility(fountain);
            Sleep();
        }

        #endregion
    }
}