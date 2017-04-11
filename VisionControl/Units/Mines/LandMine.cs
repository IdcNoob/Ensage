namespace VisionControl.Units.Mines
{
    using System.Linq;

    using Ensage;

    using SharpDX;

    internal class LandMine : Mine
    {
        private const string AbilityName = "techies_land_mines";

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

        public override bool ShowTimer { get; } = false;
    }
}