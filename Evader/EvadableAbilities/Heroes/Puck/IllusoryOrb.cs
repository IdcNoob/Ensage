namespace Evader.EvadableAbilities.Heroes.Puck
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class IllusoryOrb : LinearProjectile
    {
        private readonly float bonus;

        private readonly Ability talent;

        public IllusoryOrb(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);

            talent = AbilityOwner.FindSpell("special_bonus_unique_puck");

            if (talent != null)
            {
                bonus = talent.AbilitySpecialData.First(x => x.Name == "value").Value / 100 + 1;
            }
        }

        public override float GetProjectileSpeed()
        {
            return base.GetProjectileSpeed() * (talent?.Level > 0 ? bonus : 1);
        }

        protected override float GetCastRange()
        {
            return base.GetCastRange() * (talent?.Level > 0 ? bonus : 1);
        }
    }
}