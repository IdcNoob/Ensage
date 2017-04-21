namespace ItemManager.Core.Abilities
{
    using Attributes;

    using Base;

    using Ensage;

    using SharpDX;

    [Ability(AbilityId.item_blink)]
    internal class BlinkDagger : UsableAbility
    {
        public BlinkDagger(Ability ability, Manager manager)
            : base(ability, manager)
        {
        }

        public void Use(Vector3 position, bool queue = false)
        {
            Ability.UseAbility(position, queue);
            SetSleep(500);
        }
    }
}