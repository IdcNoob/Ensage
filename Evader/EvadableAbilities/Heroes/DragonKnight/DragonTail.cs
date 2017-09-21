namespace Evader.EvadableAbilities.Heroes.DragonKnight
{
    using System.Linq;

    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class DragonTail : Projectile, IModifier
    {
        private readonly float speed;

        public DragonTail(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.ModifierSource);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Add(Lotus);
            CounterAbilities.Add(HurricanePike);
            CounterAbilities.Remove("abaddon_aphotic_shield");

            Modifier.AllyCounterAbilities.Add(Enrage);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(Invul);
            Modifier.AllyCounterAbilities.AddRange(VsPhys);

            speed = ability.AbilitySpecialData.First(x => x.Name == "projectile_speed").Value;
        }

        public EvadableModifier Modifier { get; }

        public override float GetProjectileSpeed()
        {
            return speed;
        }
    }
}