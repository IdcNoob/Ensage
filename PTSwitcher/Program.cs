using System;
using Ensage;
using Ensage.Common;

namespace PTSwitcher {
	internal class Program {

		private static Hero hero;
		private static bool inGame;
		private static bool showInfo = true;

		private static void Main() {
			Game.OnUpdate += Game_OnUpdate;
		}

		private static void Game_OnUpdate(EventArgs args) {

			if (!Utils.SleepCheck("PTSwitcherDelay"))
				return;

			if (!inGame) {
				hero = ObjectMgr.LocalHero;

				if (!Game.IsInGame || hero == null) {
					Utils.Sleep(10000, "PTSwitcherDelay");
					return;
				}

				inGame = true;
				showInfo = true;
			}

			if (!Game.IsInGame) {
				inGame = false;
				return;
			}

			if (inGame && showInfo) {
				Game.PrintMessage("<font color='#ff0000'>PT Switcher is no longer supported!!!<br/>Use merged Smart HP/MP Abuse instead and delete this assembly</font>", MessageType.LogMessage);
				showInfo = false;
			}

			Utils.Sleep(10000, "PTSwitcherDelay");
		}
	}
}