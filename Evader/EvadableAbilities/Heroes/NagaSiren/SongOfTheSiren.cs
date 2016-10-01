namespace Evader.EvadableAbilities.Heroes
{
    using Base;

    using Ensage;

    using static Core.Abilities;

    internal class SongOfTheSiren : AOE
    {
        #region Constructors and Destructors

        public SongOfTheSiren(Ability ability)
            : base(ability)
        {
            //todo ignore remaining time ?
            BlinkAbilities.AddRange(BlinkAbilityNames);
            DisableAbilities.AddRange(DisableAbilityNames);
        }

        #endregion
    }
}