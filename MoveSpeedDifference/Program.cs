using System;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using SharpDX;

namespace MoveSpeedDifference {
	internal class Program {

		private static bool _enabled;

		private static void Main() {
			Game.OnWndProc += Game_OnWndProc;
			Drawing.OnDraw += Drawing_OnDraw;
		}

		private static void Game_OnWndProc(WndEventArgs args) {
			if (args.WParam == 18 && Game.IsInGame) {
				_enabled = args.Msg == 260;
			}
		}

		private static void Drawing_OnDraw(EventArgs args) {
			if (!Game.IsInGame || !_enabled)
				return;

			var hero = ObjectMgr.LocalHero;
			if (hero == null)
				return;

			var enemies =
				ObjectMgr.GetEntities<Hero>()
					.Where(x => !x.IsIllusion && x.IsVisible && x.IsAlive && x.Team == hero.GetEnemyTeam())
					.ToList();

			foreach (var enemy in enemies) {
				var screenPosition = HUDInfo.GetHPbarPosition(enemy);
				if (screenPosition.IsZero)
					continue;

				var ratio = HUDInfo.RatioPercentage();
				var diff = enemy.MovementSpeed - hero.MovementSpeed;

				Drawing.DrawRect(screenPosition + new Vector2(24 * ratio, -28 * ratio), new Vector2(45 * ratio, 25 * ratio), new Color(0, 100, 100, 255));

				Drawing.DrawText(diff > 0 ? "+" + diff : diff.ToString(), "Arial",
					screenPosition + new Vector2(30 * ratio, -25 * ratio),
					new Vector2(20 * ratio, 0), diff > 0 ? Color.Orange : Color.Yellow, FontFlags.AntiAlias | FontFlags.DropShadow);
			}

		}
	}
}