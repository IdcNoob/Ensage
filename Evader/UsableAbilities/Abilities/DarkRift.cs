namespace Evader.UsableAbilities.Abilities
{
    using System.Linq;

    using Base;

    using Core;

    using Ensage;

    using EvadableAbilities.Base;

    using AbilityType = Core.AbilityType;

    internal class DarkRift : Targetable
    {
        #region Fields

        private readonly Unit fountain =
            ObjectManager.GetEntities<Unit>().First(x => x.ClassID == ClassID.CDOTA_Unit_Fountain && x.Team == HeroTeam);

        #endregion

        #region Constructors and Destructors

        public DarkRift(Ability ability, AbilityType type, AbilityFlags flags = AbilityFlags.None)
            : base(ability, type, flags)
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