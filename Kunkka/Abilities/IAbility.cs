namespace Kunkka.Abilities
{
    using Ensage;

    internal interface IAbility
    {
        Ability Ability { get; }

        bool CanBeCasted { get; }

        bool Casted { get; }

        float CastPoint { get; }

        uint ManaCost { get; }
    }
}