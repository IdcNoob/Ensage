using System;
using System.Linq;
using Ensage;
using Ensage.Common.Extensions;
using SharpDX;

namespace MoveSpeedDifference {
    class Program {

	    private static bool _enabled;

		private static void Main() {
			Game.OnWndProc += Game_OnWndProc;
			Drawing.OnDraw += Drawing_OnDraw;
		}
		private static void Game_OnWndProc(WndEventArgs args) {
			if (args.WParam == 18 && !Game.IsChatOpen) {
				_enabled = args.Msg == 260;
			}
		}
		private static void Drawing_OnDraw(EventArgs args) {
			if (!Game.IsInGame || !_enabled)
				return;

			var hero = ObjectMgr.LocalHero;
			if (hero == null)
				return;
			
			var enemies = ObjectMgr.GetEntities<Hero>().Where(x => !x.IsIllusion && x.IsVisible && x.IsAlive && x.Team == hero.GetEnemyTeam()).ToList();

			foreach (var enemy in enemies) {
				Vector2 screenPos;
				var enemyPos = enemy.Position + new Vector3(0, 0, enemy.HealthBarOffset);
				if (!Drawing.WorldToScreen(enemyPos, out screenPos)) continue;

				var dif = enemy.MovementSpeed -hero.MovementSpeed;
				var text = (dif > 0 ? "+" + dif : dif.ToString());

				Drawing.DrawRect(screenPos + new Vector2(-20, -65),  new Vector2(45, 25), new Color(0, 100, 100, 200));
				Drawing.DrawText(text, "Arial", screenPos + new Vector2(-15, -65), new Vector2(22, 0), dif > 0 ? Color.Orange : Color.Yellow, FontFlags.AntiAlias | FontFlags.DropShadow);
			}

		}
	}
}
