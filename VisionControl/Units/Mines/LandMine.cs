namespace VisionControl.Units.Mines
{
    using System.Linq;

    using Ensage;

    using SharpDX;

    internal class LandMine : Mine
    {
        #region Constants

        private const string AbilityName = "techies_land_mines";

        #endregion

        #region Constructors and Destructors

        public LandMine(Unit unit)
            : base(unit, AbilityName)
        {
            PositionCorrection = new Vector2(25);
            Radius = Ability.GetAbilityDataByName(AbilityName).AbilitySpecialData.First(x => x.Name == "radius").Value;

            if (ParticleEffect != null)
            {
                ParticleEffect.SetControlPoint(1, new Vector3(255, 0, 0));
                ParticleEffect.SetControlPoint(2, new Vector3(Radius, 255, 0));
            }
        }

        #endregion

        #region Public Properties

        public override bool ShowTimer { get; } = false;

        #endregion
    }
}