namespace Evader.EvadableAbilities.Heroes.Bloodseeker
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class Rupture : LinearTarget
    {
        public Rupture(Ability ability)
            : base(ability)
        {
            DisableAbilities.AddRange(DisableAbilityNames);
            BlinkAbilities.AddRange(BlinkAbilityNames);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Lotus);
            CounterAbilities.AddRange(Invis);

            BlinkAbilities.Remove("sandking_burrowstrike");
            BlinkAbilities.Remove("riki_blink_strike");
            BlinkAbilities.Remove("magnataur_skewer");
            BlinkAbilities.Remove("slark_pounce");
            BlinkAbilities.Remove("mirana_leap");
            BlinkAbilities.Remove("earth_spirit_rolling_boulder");
            BlinkAbilities.Remove("item_force_staff");
            BlinkAbilities.Remove("item_hurricane_pike");
        }
    }
}