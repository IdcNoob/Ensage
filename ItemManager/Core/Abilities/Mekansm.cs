namespace ItemManager.Core.Abilities
{
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;

    using Interfaces;

    using Menus.Modules.Recovery;

    using Utils;

    [Ability(AbilityId.item_mekansm)]
    internal class Mekansm : UsableAbility, IRecoveryAbility
    {
        public Mekansm(Ability ability, Manager manager)
            : base(ability, manager)
        {
            ManaRestore = 0;
            HealthRestore = ability.AbilitySpecialData.First(x => x.Name == "heal_amount").Value;

            PowerTreadsAttribute = Attribute.Intelligence;
            RestoredStats = RestoredStats.Health;
        }

        public float HealthRestore { get; }

        public float ManaRestore { get; }

        public Attribute PowerTreadsAttribute { get; }

        public RestoredStats RestoredStats { get; }

        public bool ShouldBeUsed(MyHero hero, RecoveryMenu menu, float missingHealth, float missingMana)
        {
            return missingHealth >= menu.ItemSettingsMenu.Mekansm.HpThreshold;
        }
    }
}