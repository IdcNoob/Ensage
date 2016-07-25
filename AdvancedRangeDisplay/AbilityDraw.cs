namespace AdvancedRangeDisplay
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Objects;

    using SharpDX;

    internal class AbilityDraw
    {
        #region Constructors and Destructors

        public AbilityDraw(Hero hero, string abilityName)
        {
            Hero = hero;
            Name = abilityName;
            FindAbility();
        }

        #endregion

        #region Public Properties

        public Ability Ability { get; set; }

        public int Blue { get; set; }

        public float CastRange { get; set; }

        public bool Disabled { get; set; }

        public int Green { get; set; }

        public Hero Hero { get; set; }

        public bool IsValid => Ability != null && Ability.IsValid;

        public string Name { get; }

        public ParticleEffect ParticleEffect { get; set; }

        public int Red { get; set; }

        #endregion

        #region Public Methods and Operators

        public void FindAbility()
        {
            Ability =
                Hero.Spellbook.Spells.Concat(Hero.Inventory.Items).FirstOrDefault(x => x.StoredName().StartsWith(Name));
        }

        public void SetColor(int? red = null, int? green = null, int? blue = null)
        {
            if (red != null)
            {
                Red = red.Value;
            }

            if (green != null)
            {
                Green = green.Value;
            }

            if (blue != null)
            {
                Blue = blue.Value;
            }

            ParticleEffect?.SetControlPoint(1, new Vector3(Red, Green, Blue));
        }

        public void UpdateCastRange()
        {
            CastRange = Ability.ClassID == ClassID.CDOTA_Ability_AttributeBonus ? 1450 : Ability.GetRealCastRange();
        }

        #endregion
    }
}