namespace Kunkka.Abilities
{
    using Ensage;
    using Ensage.Common.AbilityInfo;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using SharpDX;

    internal class Torrent : IAbility
    {
        #region Fields

        private readonly Sleeper sleeper = new Sleeper();

        #endregion

        #region Constructors and Destructors

        public Torrent(Ability ability)
        {
            Ability = ability;
            CastPoint = (float)ability.FindCastPoint();
            AdditionalDelay = AbilityDatabase.Find(ability.Name).AdditionalDelay;
            Radius = ability.GetRadius() + 25;
        }

        #endregion

        #region Public Properties

        public Ability Ability { get; }

        public double AdditionalDelay { get; }

        public bool CanBeCasted => !sleeper.Sleeping && Ability.CanBeCasted();

        public bool Casted => Ability.Cooldown > 5;

        public float CastPoint { get; }

        public float CastRange => Ability.GetCastRange() + 100;

        public float Cooldown => Ability.Cooldown;

        public float GetSleepTime => CastPoint * 1000 + Game.Ping;

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
            sleeper.Sleep(GetSleepTime + 300);
        }

        #endregion
    }
}