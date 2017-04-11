namespace AbilityLastHitMarker.Classes
{
    using Ensage;
    using Ensage.Common.Extensions;

    internal class Ability
    {
        public Ability(Ensage.Ability ability)
        {
            Source = ability;
            Name = ability.Name;
            Texture = Drawing.GetTexture("materials/ensage_ui/spellicons/" + Name);
            Handle = ability.Handle;

            var towerDamageReduction = 0;
            if (AbilityAdjustments.TowerDamageAbilities.TryGetValue(Name, out towerDamageReduction))
            {
                DoesTowerDamage = true;
                TowerDamageReduction = towerDamageReduction;
            }
        }

        public bool CanBeCasted => Level > 0 && Source.CanBeCasted();

        public bool DoesTowerDamage { get; }

        public uint Handle { get; }

        public uint Level => Source.Level;

        public string Name { get; }

        public Ensage.Ability Source { get; }

        public DotaTexture Texture { get; }

        public int TowerDamageReduction { get; }
    }
}