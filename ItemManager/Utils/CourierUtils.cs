namespace ItemManager.Utils
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;

    internal static class CourierUtils
    {
        public static void DeliverItems(this Courier courier, bool queue = false)
        {
            var deliver = courier.Spellbook.Spells.FirstOrDefault(x => x.Id == AbilityId.courier_transfer_items);

            deliver?.UseAbility(queue);
        }

        public static void MoveToBase(this Courier courier, bool queue = false)
        {
            var goBase = courier.Spellbook.Spells.FirstOrDefault(x => x.Id == AbilityId.courier_return_to_base);

            goBase?.UseAbility(queue);
        }

        public static void MoveToSecretShop(this Courier courier, bool queue = false)
        {
            var goSecretShop = courier.Spellbook.Spells.FirstOrDefault(x => x.Id == AbilityId.courier_go_to_secretshop);

            goSecretShop?.UseAbility(queue);
        }

        public static void Resend(this Courier courier, Hero hero, bool queue = false)
        {
            var transfer = courier.Spellbook.Spells.FirstOrDefault(x => x.Id == AbilityId.courier_transfer_items_to_other_player);

            transfer?.UseAbility(hero, queue);
        }

        public static void Shield(this Courier courier, bool queue = false)
        {
            if (!courier.IsFlying)
            {
                return;
            }

            var shield = courier.Spellbook.Spells.FirstOrDefault(x => x.Id == AbilityId.courier_shield && x.CanBeCasted());

            shield?.UseAbility(queue);
        }

        public static void TakeAndDeliverItems(this Courier courier, bool queue = false)
        {
            var takeAndDeliver = courier.Spellbook.Spells.FirstOrDefault(x => x.Id == AbilityId.courier_take_stash_and_transfer_items);

            takeAndDeliver?.UseAbility(queue);
        }
    }
}