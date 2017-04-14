namespace ItemManager.Core.Abilities
{
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;
    using Ensage.Common.Extensions;

    using Interfaces;

    using Utils;

    [Ability(AbilityId.item_bottle)]
    internal class Bottle : UsableAbility, IRecoveryAbility
    {
        private readonly Ensage.Items.Bottle bottle;

        public Bottle(Ability ability, Manager manager)
            : base(ability, manager)
        {
            bottle = ability as Ensage.Items.Bottle;

            ManaRestore = ability.AbilitySpecialData.First(x => x.Name == "mana_restore").Value;
            HealthRestore = ability.AbilitySpecialData.First(x => x.Name == "health_restore").Value;

            PowerTreadsAttribute = Attribute.Agility;
            ItemRestoredStats = ItemUtils.Stats.All;
        }

        public float HealthRestore { get; }

        public ItemUtils.Stats ItemRestoredStats { get; }

        public float ManaRestore { get; }

        public Attribute PowerTreadsAttribute { get; }

        public bool TookFromStash { get; private set; }

        public bool CanBeAutoCasted()
        {
            return !IsSleeping && Ability.IsValid && Ability.CanBeCasted() && IsInInventory()
                   && bottle.CurrentCharges > 0 && bottle.StoredRune == RuneType.None;
        }

        public override bool CanBeCasted()
        {
            if (IsSleeping || !Ability.IsValid || !Ability.CanBeCasted() || bottle.CurrentCharges <= 0
                || bottle.StoredRune != RuneType.None)
            {
                return false;
            }

            if (IsInInventory())
            {
                var regeneration = Manager.MyHero.FindModifier(ModifierUtils.BottleRegeneration);
                return regeneration == null || regeneration.RemainingTime < 0.15 + Game.Ping / 1000;
            }

            if (IsInStash())
            {
                return Manager.MyHero.IsAtBase();
            }

            return false;
        }

        public void MoveItem(ItemSlot slot, bool took)
        {
            bottle.MoveItem(slot);
            TookFromStash = took;
            SetSleep(200);
        }

        public override void Use(bool queue = false)
        {
            if (IsInStash())
            {
                Manager.SaveItemSlot(bottle, ItemUtils.StoredPlace.Stash);
                MoveItem(ItemSlot.InventorySlot_1, true);
                return;
            }

            base.Use(queue);
            SetSleep(500);
        }

        public void Use(Hero hero, bool queue = false)
        {
            if (!Ability.UseAbility(hero, queue))
            {
                return;
            }

            SetSleep(500);
        }

        private bool IsInInventory()
        {
            return Manager.GetMyItems(ItemUtils.StoredPlace.Inventory).Any(x => x.Handle == Handle);
        }

        private bool IsInStash()
        {
            return Manager.GetMyItems(ItemUtils.StoredPlace.Stash).Any(x => x.Handle == Handle);
        }
    }
}