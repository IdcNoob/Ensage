namespace Evader.UsableAbilities.Abilities
{
    using System.Linq;

    using Base;

    using Common;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class TreeDance : BlinkAbility
    {
        private readonly Unit fountain;

        private Tree jumpTree;

        public TreeDance(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
            fountain = ObjectManager.GetEntities<Unit>()
                .First(x => x.NetworkName == "CDOTA_Unit_Fountain" && x.Team == HeroTeam);
        }

        public override bool CanBeCasted(EvadableAbility ability, Unit unit)
        {
            return !Sleeper.Sleeping && Ability.CanBeCasted() && Hero.CanCast() && !Hero.IsRuptured();
        }

        public override float GetRequiredTime(EvadableAbility ability, Unit unit, float remainingTime)
        {
            jumpTree = ObjectManager.GetEntitiesFast<Tree>()
                .Where(x => x.IsValid && x.IsAlive && x.Distance2D(Hero) < GetCastRange())
                .OrderBy(x => x.Distance2D(fountain))
                .FirstOrDefault(x => Hero.GetTurnTime(x) + CastPoint + 0.2f < remainingTime);

            if (jumpTree == null)
            {
                return float.MaxValue;
            }

            return CastPoint + (float)Hero.GetTurnTime(jumpTree) + 0.15f;
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            Ability.UseAbility(jumpTree, false, true);
            Sleep();
        }
    }
}