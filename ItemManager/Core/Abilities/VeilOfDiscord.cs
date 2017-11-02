namespace ItemManager.Core.Abilities
{
    using System.Collections.Generic;
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.SDK.Helpers;

    [Ability(AbilityId.item_veil_of_discord)]
    internal class VeilOfDiscord : OffensiveAbility
    {
        private readonly float radius;

        public VeilOfDiscord(Ability ability, Manager manager)
            : base(ability, manager)
        {
            radius = ability.AbilitySpecialData.First(x => x.Name == "debuff_radius").Value;
        }

        public override bool CanBeCasted(Unit target)
        {
            return base.CanBeCasted(target) && !target.HasModifier("modifier_item_veil_of_discord_debuff");
        }

        public override void Use(Unit target = null, bool queue = false)
        {
            if (target == null)
            {
                return;
            }

            SetSleep(200);

            var position = target.Position;
            var tempPosition = target.Position;
            var inRange = new List<Hero>();

            foreach (var enemy in EntityManager<Hero>.Entities.Where(
                x => x.IsValid && x.IsVisible && !x.Equals(target) && !x.IsIllusion && x.IsAlive && x.Team != Manager.MyHero.Team
                     && !x.IsInvul() && x.Distance2D(target) < radius * 2))
            {
                tempPosition = (tempPosition + enemy.Position) / 2;

                if (tempPosition.Distance2D(Manager.MyHero.Position) > GetCastRange()
                    || inRange.Any(x => x.Distance2D(tempPosition) > radius))
                {
                    tempPosition = position;
                    continue;
                }

                position = tempPosition;
                inRange.Add(enemy);
            }

            Ability.UseAbility(position, queue);
        }
    }
}