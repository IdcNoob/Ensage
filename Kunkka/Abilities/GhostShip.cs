namespace Kunkka.Abilities
{
    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using SharpDX;

    internal class GhostShip : IAbility
    {
        #region Fields

        private readonly Sleeper sleeper = new Sleeper();

        #endregion

        #region Constructors and Destructors

        public GhostShip(Ability ability)
        {
            Ability = ability;
            CastPoint = (float)ability.FindCastPoint();
            Radius = 560;
            Speed = ability.GetProjectileSpeed();
            AghanimSpeed = Speed * 4;
        }

        #endregion

        #region Public Properties

        public Ability Ability { get; }

        public float AghanimSpeed { get; private set; }

        public bool CanBeCasted => !sleeper.Sleeping && Ability.CanBeCasted();

        public bool Casted => Ability.AbilityState == AbilityState.OnCooldown;

        public float CastPoint { get; }

        public float CastRange => Ability.GetCastRange() + 150;

        public float Cooldown => Ability.Cooldown;

        public float GetSleepTime => CastPoint * 1000 + Game.Ping;

        public double HitTime { get; set; }

        public bool IsInPhase => Ability.IsInAbilityPhase;

        public bool JustCasted => Casted && Ability.Cooldown + 4 >= Ability.CooldownLength;

        public uint ManaCost => Ability.ManaCost;

        public Vector3 Position { get; set; }

        public float Radius { get; }

        public float Speed { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void UseAbility(Vector3 targetPosition)
        {
            Ability.UseAbility(targetPosition);
            sleeper.Sleep(GetSleepTime + 300);
        }

        #endregion
    }
}