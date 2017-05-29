namespace ItemManager.Core.Abilities.Interfaces
{
    using Ensage;

    using Menus.Modules.DefensiveAbilities.AbilitySettings;

    internal interface IDefensiveAbility
    {
        uint Handle { get; }

        bool IsItem { get; }

        DefensiveAbilitySettings Menu { get; set; }

        string Name { get; }

        bool CanBeCasted();

        void Use(Unit target = null, bool queue = false);
    }
}