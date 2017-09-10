namespace Kunkka.Abilities
{
    using Ensage;
    using Ensage.Common.AbilityInfo;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using SharpDX;

    internal class Torrent : IAbility
    {
        private readonly Sleeper sleeper = new Sleeper();

        public Torrent(Ability ability)
        {
            Ability = ability;
            CastPoint = (float)ability.FindCastPoint();
            AdditionalDelay = AbilityDatabase.Find(ability.Name).AdditionalDelay;
            Radius = ability.GetRadius() + 25;
        }

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

        public void CalculateHitTime(float adjustedTime)
        {
            var gameTime = Game.RawGameTime;

            if (HitTime <= gameTime)
            {
                HitTime = gameTime + AdditionalDelay + CastPoint + Game.Ping / 1000 - 0.085 + adjustedTime / 1000;
            }
        }

        public void UseAbility(Vector3 targetPosition, float adjustedTime = 0)
        {
            CalculateHitTime(adjustedTime);
            Ability.UseAbility(targetPosition);
            sleeper.Sleep(GetSleepTime + 300);
        }
    }
}