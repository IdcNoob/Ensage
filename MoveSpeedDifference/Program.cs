using System;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using SharpDX;

namespace MoveSpeedDifference {
    internal class Program {

        private static readonly Menu Menu = new Menu("Move Speed Difference", "moveSpeed", true);

        private static void Main() {
            Drawing.OnDraw += Drawing_OnDraw;

            Menu.AddItem(new MenuItem("hotkey", "Change hotkey").SetValue(new KeyBind(18, KeyBindType.Press)));
            Menu.AddItem(new MenuItem("ally", "Show on allies").SetValue(false));
            Menu.AddItem(new MenuItem("size", "Size").SetValue(new Slider(0, -2, 10)));
            Menu.AddItem(new MenuItem("posX", "Position X").SetValue(new Slider(0, -100, 100)));
            Menu.AddItem(new MenuItem("posY", "Position Y").SetValue(new Slider(0, -100, 100)));

            Menu.AddToMainMenu();
        }

        private static void Drawing_OnDraw(EventArgs args) {
            if (!Game.IsInGame || !Menu.Item("hotkey").GetValue<KeyBind>().Active)
                return;

            var hero = ObjectMgr.LocalHero;
            if (hero == null)
                return;

            var heroes =
                ObjectMgr.GetEntities<Hero>()
                    .Where(x => !x.IsIllusion && x.IsVisible && x.IsAlive && !x.Equals(hero))
                    .ToList();

            if (!Menu.Item("ally").GetValue<bool>())
                heroes = heroes.Where(x => x.Team == hero.GetEnemyTeam()).ToList();

            var ratio = HUDInfo.RatioPercentage() +
                        (HUDInfo.RatioPercentage() * ((float) Menu.Item("size").GetValue<Slider>().Value / 10));

            var posX = Menu.Item("posX").GetValue<Slider>().Value;
            var posY = Menu.Item("posY").GetValue<Slider>().Value;

            foreach (var unit in heroes) {
                var screenPosition = HUDInfo.GetHPbarPosition(unit);
                if (screenPosition.IsZero)
                    continue;

                var diff = unit.MovementSpeed - hero.MovementSpeed;

                Drawing.DrawRect(screenPosition + new Vector2(posX, posY), new Vector2(45 * ratio, 25 * ratio),
                    new Color(0, 100, 100, 255));

                Drawing.DrawText(diff > 0 ? "+" + diff : diff.ToString(), "Arial",
                    screenPosition + new Vector2(posX + 5 * ratio, posY + 4 * ratio),
                    new Vector2(20 * ratio, 0), diff > 0 ? Color.Orange : Color.Yellow,
                    FontFlags.AntiAlias | FontFlags.DropShadow);
            }

        }
    }
}