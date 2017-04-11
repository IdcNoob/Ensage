namespace Evader.EvadableAbilities.Heroes.Lion
{
    using System.Linq;

    using Base;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    internal class FingerOfDeath : LinearTarget
    {
        private readonly float aghanimRadius;

        public FingerOfDeath(Ability ability)
            : base(ability)
        {
            BlinkAbilities.AddRange(BlinkAbilityNames);
            DisableAbilities.AddRange(DisableAbilityNames);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(TricksOfTheTrade);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invul);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Add(Lotus);
            CounterAbilities.Add(NetherWard);
            CounterAbilities.AddRange(Invis);

            AdditionalDelay = Ability.AbilitySpecialData.First(x => x.Name == "damage_delay").Value + 0.05f;
            aghanimRadius = Ability.AbilitySpecialData.First(x => x.Name == "splash_radius_scepter").Value + 60;
        }

        protected override float GetCastRange()
        {
            return base.GetCastRange() + 150;
        }

        protected override float GetRadius()
        {
            return AbilityOwner.AghanimState() ? aghanimRadius : base.GetRadius();
        }
    }
}