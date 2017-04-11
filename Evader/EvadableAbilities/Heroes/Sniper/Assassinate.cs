namespace Evader.EvadableAbilities.Heroes.Sniper
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class Assassinate : Projectile
    {
        private readonly float aghanimRadius;

        public Assassinate(Ability ability)
            : base(ability)
        {
            BlinkAbilities.Add("item_blink");
            BlinkAbilities.Add("antimage_blink");
            BlinkAbilities.Add("queenofpain_blink");
            BlinkAbilities.Add("ember_spirit_activate_fire_remnant");

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Add(Lotus);

            aghanimRadius = Ability.AbilitySpecialData.First(x => x.Name == "scepter_radius").Value;
        }

        protected override float GetRadius()
        {
            return AbilityOwner.AghanimState() ? aghanimRadius : base.GetRadius();
        }
    }
}