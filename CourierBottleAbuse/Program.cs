using System;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;

namespace CourierBottleAbuse {
	class Program {

		private static bool _enabled = false;
		private static bool _following = false;

		private static void Main() {
			Game.OnIngameUpdate += Game_OnIngameUpdate;
			Game.OnWndProc += Game_OnWndProc;
		}

		private static void Game_OnWndProc(WndEventArgs args) {
			if (args.WParam == 'Z' && !Game.IsChatOpen && args.Msg == (uint) Utils.WindowsMessages.WM_KEYUP) {
				_enabled = true;
				_following = false;
			}
		}

		private static void Game_OnIngameUpdate(EventArgs args) {
			if (Game.IsPaused || !_enabled || !Utils.SleepCheck("delay"))
				return;

			var hero = ObjectMgr.LocalHero;

			if (hero == null || !hero.IsAlive) {
				_enabled = false;
				return;
			}

			var bottle = hero.FindItem("item_bottle");
			var courier = ObjectMgr.GetEntities<Courier>().FirstOrDefault(x => x.IsAlive && x.Team == hero.Team);
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

			if (distance <= 200 && _following/* && bottle != null && bottle.CurrentCharges == 0*/) {
				hero.Stop();
				hero.GiveItem(bottle, courier);
				_following = false;
			}

			if (distance <= 200 && !_following && courBottle != null) {
				courier.Spellbook.SpellQ.UseAbility();
				if (courier.IsFlying) courier.Spellbook.SpellR.UseAbility();
				courier.Spellbook.SpellD.UseAbility(true);
				courier.Spellbook.SpellF.UseAbility(true);
				_enabled = false;
			}

			Utils.Sleep(250, "delay");
		}

	}
}