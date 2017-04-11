namespace Evader.EvadableAbilities.Heroes.DarkSeer
{
    using System.Linq;

    using Base;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    internal class Vacuum : LinearAOE
    {
        private readonly Ability talent;

        private readonly float talentAOE;

        public Vacuum(Ability ability)
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

            talent = AbilityOwner.FindSpell("special_bonus_unique_dark_seer_2");
            talentAOE = talent?.AbilitySpecialData.First(x => x.Name == "value").Value ?? 0;
        }

        protected override float GetRadius()
        {
            return Ability.GetRadius() + (talent?.Level > 0 ? talentAOE : 0);
        }
    }
}