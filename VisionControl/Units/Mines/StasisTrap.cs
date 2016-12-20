namespace VisionControl.Units.Mines
{
    using System.Linq;

    using Ensage;

    using SharpDX;

    internal class StasisTrap : Mine
    {
        #region Constants

        private const string AbilityName = "techies_stasis_trap";

        #endregion

        #region Constructors and Destructors

        public StasisTrap(Unit unit)
            : base(unit, AbilityName)
        {
            PositionCorrection = new Vector2(25);
            Radius =
                Ability.GetAbilityDataByName(AbilityName)
                    .AbilitySpecialData.First(x => x.Name == "activation_radius")
                    .Value;

            if (ParticleEffect != null)
            {
                ParticleEffect.SetControlPoint(1, new Vector3(65, 105, 225));
                ParticleEffect.SetControlPoint(2, new Vector3(Radius, 255, 0));
            }
        }

        #endregion

        #region Public Properties

        public override bool ShowTimer { get; } = false;

        #endregion
    }
}