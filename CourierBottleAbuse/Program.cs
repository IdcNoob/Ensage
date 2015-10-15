using System;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using SharpDX;

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

			if (bottle != null) {
				var courier = ObjectMgr.GetEntities<Courier>().FirstOrDefault();

				if (courier == null) {
					_enabled = false;
					return;
				}

				if (!_following) {
					courier.Follow(hero);
					_following = true;
				}

				if (courier.Distance2D(hero) < 200 && _following) {
					hero.GiveItem(bottle, courier);
					_following = false;
				}

				if (courier.Distance2D(hero) < 200 && !_following && courier.FindItem("item_bottle").Purchaser.Equals(hero)) {
					courier.FindSpell("courier_return_to_base").UseAbility();
					if (courier.IsFlying) courier.FindSpell("courier_burst").UseAbility();
					courier.FindSpell("courier_take_stash_items").UseAbility(true);
					courier.FindSpell("courier_transfer_items").UseAbility(true);

					_enabled = false;
				}

			}


			Utils.Sleep(250, "delay");
		}

	}
}