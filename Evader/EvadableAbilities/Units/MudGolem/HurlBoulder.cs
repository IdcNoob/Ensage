namespace Evader.EvadableAbilities.Units.MudGolem
{
    using Ensage;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class HurlBoulder : Projectile
    {
        public HurlBoulder(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.AddRange(VsLowDisable);
            CounterAbilities.AddRange(Invis);
        }
    }
}