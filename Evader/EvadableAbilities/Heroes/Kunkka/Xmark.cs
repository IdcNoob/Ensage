namespace Evader.EvadableAbilities.Heroes.Kunkka
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class Xmark : LinearTarget
    {
        public Xmark(Ability ability)
            : base(ability)
        {
            IsDisable = false;

            BlinkAbilities.Clear();
            BlinkAbilities.Add("item_blink");

            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(Lotus);
        }
    }
}