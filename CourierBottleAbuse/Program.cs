using System;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;

namespace CourierBottleAbuse {
    internal class Program {

        private static bool enabled;
        private static bool following;

        private static readonly Menu Menu = new Menu("Courier Bottle Abuse", "bottleAbuse", true);

        private static void Main() {
            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;

            Menu.AddItem(new MenuItem("hotkey", "Change hotkey").SetValue(new KeyBind('Z', KeyBindType.Press)));

            var stashItems = new Menu("Take items from stash", "stash");

            stashItems.AddItem(new MenuItem("stashBefore", "Before").SetValue(true)
                .SetTooltip("Will bring items from stash before abusing bottle"));
            stashItems.AddItem(new MenuItem("stashLimitBefore", "Limit before").SetValue(true)
                .SetTooltip("Will take items only if couier in base, when \"Before\" active"));
            stashItems.AddItem(new MenuItem("stashAfter", "After").SetValue(true)
                .SetTooltip("Will bring items from stash with refilled bottle"));

            Menu.AddSubMenu(stashItems);
            Menu.AddToMainMenu();
        }

        private static void Game_OnWndProc(WndEventArgs args) {
            if (args.WParam == Menu.Item("hotkey").GetValue<KeyBind>().Key &&
                args.Msg == (uint) Utils.WindowsMessages.WM_KEYUP &&
                !Game.IsChatOpen) {
                enabled = true;
                following = false;
            }
        }

        private static void Game_OnUpdate(EventArgs args) {
            if (!Utils.SleepCheck("CourierAbuseDelay"))
                return;

            if (Game.IsPaused || !Game.IsInGame || !enabled) {
                Utils.Sleep(1000, "CourierAbuseDelay");
                return;
            }

            var hero = ObjectManager.LocalHero;

            if (hero == null || !hero.IsAlive) {
                enabled = false;
                return;
            }

            var courier = ObjectManager.GetEntities<Courier>().FirstOrDefault(x => x.IsAlive && x.Team == hero.Team);

            if (courier == null) {
                enabled = false;
                return;
            }

            var bottle = hero.Inventory.Items.FirstOrDefault(x => x.Name == "item_bottle");
            var courBottle = courier.Inventory.Items.FirstOrDefault(x => x.Name == "item_bottle");

            if (bottle == null && courBottle == null) {
                enabled = false;
                return;
            }

            var distance = hero.Distance2D(courier);

            if (distance > 200 && !following) {
                if (Menu.Item("stashBefore").GetValue<bool>() && hero.Inventory.StashItems.Any()) {
                    if (courier.HasModifier("modifier_fountain_aura_buff") || !Menu.Item("stashLimitBefore").GetValue<bool>()) {
                        courier.Spellbook.SpellD.UseAbility();
                        courier.Spellbook.SpellF.UseAbility(true);
                        courier.Follow(hero, true);
                    } else {
                        courier.Follow(hero);
                    }
                } else {
                    courier.Follow(hero);
                }
                following = true;
            } else if (distance <= 200 && bottle != null && bottle.CurrentCharges == 0) {
                hero.Stop();
                hero.GiveItem(bottle, courier);
            } else if (courBottle != null) {
                courier.Spellbook.SpellQ.UseAbility();
                var burst = courier.Spellbook.SpellR;
                if (courier.IsFlying && burst.CanBeCasted())
                    burst.UseAbility();
                if (Menu.Item("stashAfter").GetValue<bool>())
                    courier.Spellbook.SpellD.UseAbility(true);
                courier.Spellbook.SpellF.UseAbility(true);
                enabled = false;
            }

            Utils.Sleep(333, "CourierAbuseDelay");

        }
    }
}