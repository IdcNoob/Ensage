using System;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;

namespace CourierBottleAbuse {
	class Program {

		private const int WM_KEYUP = 0x0101;

		private static bool _enabled = false;
		private static bool _following = false;

		private static void Main() {
			Game.OnUpdate += Game_OnUpdate;
			Game.OnWndProc += Game_OnWndProc;
		}
		private static void Game_OnWndProc(WndEventArgs args) {
			if (args.WParam == 'Z' && !Game.IsChatOpen && args.Msg == WM_KEYUP) {
				_enabled = true;
				_following = false;
			}
		}

		private static void Game_OnUpdate(EventArgs args) {

			if (!Game.IsInGame || !_enabled || Game.IsPaused || !Utils.SleepCheck("delay"))
				return;

			var player = ObjectMgr.LocalPlayer;
			if (player == null || player.Team == Team.Observer)
				return;

			var hero = ObjectMgr.LocalHero;

			var bottle = hero.FindItem("item_bottle");
			var courier = ObjectMgr.GetEntities<Courier>().FirstOrDefault();
			var courBottle = courier.FindItem("item_bottle");

			if ((bottle == null && courBottle == null) || courier == null) {
				_enabled = false;
				return;
			}
			var distance = hero.Distance2D(courier);

			if (distance > 200 && !_following) {
				courier.Follow(hero);
				_following = true;
			}

			if (distance <= 200 && _following) {
				hero.Stop();
				hero.GiveItem(bottle, courier);
				_following = false;
			}

			if (distance <= 200 && !_following && courBottle != null) {
				courier.FindSpell("courier_return_to_base").UseAbility();
				if (courier.IsFlying) courier.FindSpell("courier_burst").UseAbility();
				courier.FindSpell("courier_take_stash_items").UseAbility(true);
				courier.FindSpell("courier_transfer_items").UseAbility(true);

				_enabled = false;
			}

			Utils.Sleep(250, "delay");
		}

	}
}