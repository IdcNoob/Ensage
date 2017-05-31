namespace ItemManager.Core.Abilities.Interfaces
{
    using Ensage;

    using Menus.Modules.OffensiveAbilities.AbilitySettings;

    internal interface IOffensiveAbility
    {
        uint Handle { get; }

        bool IsItem { get; }

        OffensiveAbilitySettings Menu { get; set; }

        string Name { get; }

        bool CanBeCasted(Unit target);

        bool CanBeCasted();

        void ChangeName(string name);

        void Use(Unit target, bool queue = false);
    }
}