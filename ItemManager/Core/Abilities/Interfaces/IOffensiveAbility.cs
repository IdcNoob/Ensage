namespace ItemManager.Core.Abilities.Interfaces
{
    using Ensage;

    internal interface IOffensiveAbility
    {
        bool CanBeCasted(Unit target);

        float GetCastRange();

        void Use(Unit target, bool queue = false);
    }
}