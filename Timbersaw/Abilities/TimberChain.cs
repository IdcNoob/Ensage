namespace Timbersaw.Abilities
{
    using Ensage;

    using SharpDX;

    internal class TimberChain : TimberAbility
    {
        #region Fields

        private readonly int[] speed = { 1600, 2000, 2400, 2800 };

        #endregion

        #region Constructors and Destructors

        public TimberChain(Ability ability)
            : base(ability)
        {
            Radius = 250;
            ModifierName = "modifier_shredder_timber_chain";
        }

        #endregion

        #region Public Properties

        public bool CastedOnEnemy { get; set; }

        public bool ChakramCombo { get; set; }

        public float Cooldown => Ability.Cooldown;

        public bool IsInPhase => Ability.IsInAbilityPhase;

        public uint Level => Ability.Level;

        public string ModifierName { get; }

        public Vector3 Position { get; set; }

        public float Radius { get; }

        public float Speed => speed[Ability.Level - 1];

        #endregion

        #region Public Methods and Operators

        public void Stop(Hero hero)
        {
            hero.Stop();
            Sleeper.Sleep(Game.Ping);
        }

        public void UseAbility(Vector3 position, bool enemy = false)
        {
            Ability.UseAbility(position);
            CastedOnEnemy = enemy;
            Sleeper.Sleep(GetSleepTime + 1000);
        }

        #endregion
    }
}