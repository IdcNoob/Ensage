namespace AdvancedRangeDisplay
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;

    using SharpDX;

    internal class AbilityDraw
    {
        public AbilityDraw(Hero hero, string abilityName)
        {
            Hero = hero;
            Name = abilityName;
            FindAbility();

            switch (Name)
            {
                case "attackRange":
                    CustomRange = Ranges.CustomRange.Attack;
                    break;
                case "expRange":
                    CustomRange = Ranges.CustomRange.Experience;
                    break;
                case "aggroRange":
                    CustomRange = Ranges.CustomRange.Aggro;
                    break;
            }
        }

        public Ability Ability { get; set; }

        public int Blue { get; set; }

        public Ranges.CustomRange CustomRange { get; }

        public bool Disabled { get; set; }

        public int Green { get; set; }

        public Hero Hero { get; set; }

        public bool IsValid => Ability != null && Ability.IsValid;

        public uint Level => IsValid ? Ability.Level : 1;

        public string Name { get; }

        public ParticleEffect ParticleEffect { get; set; }

        public float Radius { get; set; }

        public bool RadiusOnly { get; set; }

        public float Range { get; set; }

        public float RealCastRange { get; set; }

        public int Red { get; set; }

        public void FindAbility()
        {
            Ability = Hero.Spellbook.Spells.Concat(Hero.Inventory.Items)
                .FirstOrDefault(x => x.StoredName().StartsWith(Name));
        }

        public void SaveSettings(int? red = null, int? green = null, int? blue = null, bool? radiusOnly = null)
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

            if (radiusOnly != null)
            {
                RadiusOnly = radiusOnly.Value;
                ParticleEffect?.Dispose();
                ParticleEffect = null;
            }

            ParticleEffect?.SetControlPoint(1, new Vector3(Red, Green, Blue));
        }

        public void UpdateCastRange(bool radiusOnly = false)
        {
            switch (CustomRange)
            {
                case Ranges.CustomRange.Attack:
                    var attackRange = Hero.GetAttackRange();
                    RealCastRange = Hero.GetRealAttackRange();
                    Range = attackRange;
                    return;
                case Ranges.CustomRange.Experience:
                    RealCastRange = 1650;
                    Range = 1650;
                    return;
                case Ranges.CustomRange.Aggro:
                    RealCastRange = 525;
                    Range = 525;
                    return;
            }

            RealCastRange = Ability.GetRealCastRange();
            if (Ability.Name == "kunkka_tidebringer")
            {
                Radius = Range = Ability.GetRadius() + 35;
            }
            else
            {
                Range = Ability.GetCastRange();
                Radius = Ability.GetRadius() + 25;
            }
        }
    }
}