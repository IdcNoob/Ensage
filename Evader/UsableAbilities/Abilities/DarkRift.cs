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
        private readonly Unit fountain;

        public DarkRift(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
            fountain = ObjectManager.GetEntities<Unit>()
                .First(x => x.NetworkName == "CDOTA_Unit_Fountain" && x.Team == HeroTeam);
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            Ability.UseAbility(fountain, false, true);
            Sleep();
        }
    }
}