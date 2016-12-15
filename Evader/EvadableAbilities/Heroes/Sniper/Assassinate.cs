namespace Evader.EvadableAbilities.Heroes.Sniper
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class Assassinate : Projectile
    {
        #region Fields

        private readonly float aghanimRadius;

        #endregion

        #region Constructors and Destructors

        public Assassinate(Ability ability)
            : base(ability)
        {
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

        #endregion

        #region Methods

        protected override float GetRadius()
        {
            return AbilityOwner.AghanimState() ? aghanimRadius : base.GetRadius();
        }

        #endregion
    }
}