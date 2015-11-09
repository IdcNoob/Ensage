using System;
using System.Linq;
using Ensage;
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
				Vector2 screenPos;
				var enemyPos = enemy.Position + new Vector3(0, 0, enemy.HealthBarOffset);
				if (!Drawing.WorldToScreen(enemyPos, out screenPos))
					continue;

				var diff = enemy.MovementSpeed - hero.MovementSpeed;

				Drawing.DrawRect(screenPos + new Vector2(-20, -65), new Vector2(45, 25), new Color(0, 100, 100, 200));
				Drawing.DrawText(diff > 0 ? "+" + diff : diff.ToString(), "Arial", screenPos + new Vector2(-15, -65),
					new Vector2(22, 0), diff > 0 ? Color.Orange : Color.Yellow, FontFlags.AntiAlias | FontFlags.DropShadow);
			}

		}
	}
}
