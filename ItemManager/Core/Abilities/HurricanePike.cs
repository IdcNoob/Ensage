namespace ItemManager.Core.Abilities
{
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Helpers;

    [Ability(AbilityId.item_hurricane_pike)]
    internal class HurricanePike : DefensiveAbility
    {
        private readonly float castRange;

        public HurricanePike(Ability ability, Manager manager)
            : base(ability, manager)
        {
            castRange = ability.AbilitySpecialData.First(x => x.Name == "cast_range_enemy").Value + 100;
        }

        public override void Use(Unit target = null, bool queue = false)
        {
            var closestTarget = EntityManager<Hero>.Entities
                .Where(
                    x => x.IsValid && x.IsVisible && x.IsAlive && !x.IsMagicImmune() && !x.IsIllusion && x.Team != Manager.MyHero.Team
                         && x.Distance2D(Manager.MyHero.Position) <= castRange)
                .OrderBy(x => x.Distance2D(Manager.MyHero.Position))
                .FirstOrDefault();

            if (closestTarget != null)
            {
                Ability.UseAbility(closestTarget);
                SetSleep(500);
            }
        }
    }
}