namespace Kunkka.Abilities
{
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    using SharpDX;

    internal class Torrent : IAbility
    {
        #region Fields

        #endregion

        #region Constructors and Destructors

        public Torrent(Ability ability)
        {
            Ability = ability;
            CastPoint = ability.FindCastPoint();
            AdditionalDelay = 1.6; //AbilityDatabase.Find(Abiltity.Name).AdditionalDelay;
            Radius = ability.GetRadius();
        }

        #endregion

        #region Public Properties

        public Ability Ability { get; }

        public bool CanBeCasted => Utils.SleepCheck("Kunkka.Torrent") && Ability.CanBeCasted();

        public bool Casted => Ability.AbilityState == AbilityState.OnCooldown;

        public double CastPoint { get; }

        public double HitTime { get; private set; }

        public uint ManaCost => Ability.ManaCost;

        public Vector3 Position { get; set; }

        public float Radius { get; }

        public double AdditionalDelay { get; }

        public float CastRange => Ability.GetCastRange() + 150;
        #endregion

        #region Public Methods and Operators

        public void CalculateHitTime()
        {
            HitTime = Game.GameTime + AdditionalDelay + CastPoint * 0.75;
        }

        public void UseAbility(Vector3 targetPosition)
        {
            Ability.UseAbility(targetPosition);
            CalculateHitTime();
            Utils.Sleep(CastPoint * 1000 + 200, "Kunkka.Torrent");
        }

        #endregion
    }
}