namespace Kunkka.Abilities
{
    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    internal class Xreturn : IAbility
    {
        private readonly Sleeper sleeper = new Sleeper();

        public Xreturn(Ability ability)
        {
            Ability = ability;
            CastPoint = (float)ability.FindCastPoint();
        }

        public Ability Ability { get; }

        public bool CanBeCasted => !sleeper.Sleeping && !Ability.IsHidden && Ability.CanBeCasted();

        public bool Casted => Ability.IsHidden;

        public float CastPoint { get; }

        public float GetSleepTime => CastPoint * 1000 + Game.Ping;

        public uint ManaCost { get; } = 0;

        public void UseAbility()
        {
            Ability.UseAbility();
            sleeper.Sleep(GetSleepTime + 300);
        }
    }
}