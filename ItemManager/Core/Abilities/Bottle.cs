namespace ItemManager.Core.Abilities
{
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;
    using Ensage.Common.Extensions;

    using Interfaces;

    using Menus.Modules.Recovery;

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
            RestoredStats = RestoredStats.All;
        }

        public float HealthRestore { get; }

        public float ManaRestore { get; }

        public Attribute PowerTreadsAttribute { get; }

        public RestoredStats RestoredStats { get; }

        public bool TakenFromStash { get; private set; }

        public bool CanBeAutoCasted()
        {
            return !IsSleeping && Ability.IsValid && Ability.CanBeCasted() && IsInInventory() && bottle.CurrentCharges > 0
                   && bottle.StoredRune == RuneType.None;
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
                return true;
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
            TakenFromStash = took;
            SetSleep(200);
        }

        public bool ShouldBeUsed(MyHero hero, RecoveryMenu menu, float missingHealth, float missingMana)
        {
            if (menu.ItemSettingsMenu.BottleSettings.OverhealEnabled)
            {
                return missingMana >= menu.ItemSettingsMenu.BottleSettings.MpThreshold
                       || missingHealth >= menu.ItemSettingsMenu.BottleSettings.HpThreshold;
            }

            return missingMana >= menu.ItemSettingsMenu.BottleSettings.MpThreshold
                   && missingHealth >= menu.ItemSettingsMenu.BottleSettings.HpThreshold;
        }

        public override void Use(Unit target = null, bool queue = false)
        {
            var regeneration = target?.FindModifier(ModifierUtils.BottleRegeneration)
                               ?? Manager.MyHero.Hero.FindModifier(ModifierUtils.BottleRegeneration);
            if (IsInInventory() && !Manager.MyHero.IsAtBase() && regeneration?.RemainingTime > 0.15 + (Game.Ping / 1000))
            {
                return;
            }

            if (IsInStash())
            {
                Manager.MyHero.SaveItemSlot(bottle, ItemStoredPlace.Stash);
                MoveItem(ItemSlot.InventorySlot_1, true);
                return;
            }

            base.Use(target, queue);
            SetSleep(500);
        }

        public void Use(Hero hero, bool queue = false)
        {
            SetSleep(500);
            Ability.UseAbility(hero, queue);
        }

        private bool IsInInventory()
        {
            return Manager.MyHero.GetItems(ItemStoredPlace.Inventory).Any(x => x.Handle == Handle);
        }

        private bool IsInStash()
        {
            return Manager.MyHero.GetItems(ItemStoredPlace.Stash).Any(x => x.Handle == Handle);
        }
    }
}