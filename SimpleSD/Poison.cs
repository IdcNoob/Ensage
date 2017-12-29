namespace SimpleSD
{
    using Ensage;
    using Ensage.SDK.Abilities;
    using Ensage.SDK.Extensions;

    internal class Poison : LineAbility
    {
        public Poison(Ability ability)
            : base(ability)
        {
        }

        public override float Radius
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("radius");
            }
        }

        public override float Speed
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("speed");
            }
        }
    }
}