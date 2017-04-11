namespace Evader.EvadableAbilities.Base.Interfaces
{
    using Ensage;

    internal interface IUnit
    {
        //todo remove name after tests
        string Name { get; }

        void AddUnit(Unit unit);
    }
}