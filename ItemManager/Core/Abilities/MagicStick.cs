namespace ItemManager.Core.Abilities
{
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;

    using Interfaces;

    using Utils;

    [Ability(AbilityId.item_magic_stick)]
    [Ability(AbilityId.item_magic_wand)]
    internal class MagicStick : UsableAbility, IRecoveryAbility
    {
        private readonly Item magicStick;

        private readonly float restorePerCharge;

        public MagicStick(Ability ability, Manager manager)
            : base(ability, manager)
        {
            magicStick = ability as Item;

            restorePerCharge = ability.AbilitySpecialData.First(x => x.Name == "restore_per_charge").Value;

            PowerTreadsAttribute = Attribute.Agility;
            ItemRestoredStats = ItemUtils.Stats.All;
        }

        public float HealthRestore => restorePerCharge * magicStick.CurrentCharges;

        public ItemUtils.Stats ItemRestoredStats { get; }

        public float ManaRestore => restorePerCharge * magicStick.CurrentCharges;

        public Attribute PowerTreadsAttribute { get; }

        public override bool CanBeCasted()
        {
            return base.CanBeCasted() && magicStick.CurrentCharges > 0;
        }
    }
}