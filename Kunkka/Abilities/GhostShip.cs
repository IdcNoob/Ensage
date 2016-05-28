namespace Kunkka.Abilities
{
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    using SharpDX;

    internal class GhostShip : IAbility
    {
        #region Constructors and Destructors

        //private readonly double additionalDelay;

        public GhostShip(Ability ability)
        {
            Ability = ability;
            CastPoint = ability.FindCastPoint();
            // additionalDelay = 2.7; //AbilityDatabase.Find(ability.Name).AdditionalDelay;
        }

        #endregion

        #region Public Properties

        public Ability Ability { get; }

        public bool CanBeCasted => Utils.SleepCheck("Kunkka.GhostShip") && Ability.CanBeCasted();

        public bool Casted => Ability.AbilityState == AbilityState.OnCooldown;

        public double CastPoint { get; }

        public uint ManaCost => Ability.ManaCost;

        #endregion

        #region Public Methods and Operators

        public void UseAbility(Vector3 targetPosition)
        {
            Ability.UseAbility(targetPosition);
            Utils.Sleep(CastPoint * 1000 + 200, "Kunkka.GhostShip");
        }

        #endregion
    }
}