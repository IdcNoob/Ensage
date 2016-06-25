namespace Kunkka.Abilities
{
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.AbilityInfo;
    using Ensage.Common.Extensions;

    using SharpDX;

    internal class Torrent : IAbility
    {
        #region Constructors and Destructors

        public Torrent(Ability ability)
        {
            Ability = ability;
            CastPoint = ability.FindCastPoint();
            AdditionalDelay = AbilityDatabase.Find(ability.Name).AdditionalDelay;
            Radius = ability.GetRadius();
        }

        #endregion

        #region Public Properties

        public Ability Ability { get; }

        public double AdditionalDelay { get; }

        public bool CanBeCasted => Utils.SleepCheck("Kunkka.Torrent") && Ability.CanBeCasted();

        public bool Casted => Ability.Cooldown > 5;

        public double CastPoint { get; }

        public float CastRange => Ability.GetCastRange() + 100;

        public float Cooldown => Ability.Cooldown;

        public double GetSleepTime => CastPoint * 1000 + Game.Ping;

        public double HitTime { get; private set; }

        public uint ManaCost => Ability.ManaCost;

        public float Radius { get; }

        #endregion

        #region Public Methods and Operators

        public void CalculateHitTime()
        {
            var gameTime = Game.RawGameTime;

            if (HitTime <= gameTime)
            {
                HitTime = gameTime + AdditionalDelay + CastPoint + Game.Ping / 1000 - 0.085;
            }
        }

        public void UseAbility(Vector3 targetPosition)
        {
            CalculateHitTime();
            Ability.UseAbility(targetPosition);
            Utils.Sleep(GetSleepTime + 300, "Kunkka.Torrent");
        }

        #endregion
    }
}