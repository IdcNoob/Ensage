namespace Kunkka.Abilities
{
    using Ensage;
    using Ensage.Common.Extensions;

    internal class TideBringer : IAbility
    {
        public TideBringer(Ability ability)
        {
            Ability = ability;
        }

        public Ability Ability { get; }

        public bool CanBeCasted => Ability.CanBeCasted();

        public bool Casted => Ability.AbilityState == AbilityState.OnCooldown;

        public float CastPoint { get; } = 0;

        public uint ManaCost { get; } = 0;

        public void UseAbility(Unit target, bool queue)
        {
            Ability.UseAbility(target, queue);
        }
    }
}