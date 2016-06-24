namespace MoveSpeedDifference
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using Ensage.Common.Objects;

    using SharpDX;

    internal class Program
    {
        #region Static Fields

        private static readonly Menu Menu = new Menu("Move Speed Difference", "moveSpeed", true);

        private static Team enemyTeam;

        private static Hero hero;

        private static bool pause;

        private static Dictionary<Unit, int> unitsSpeed = new Dictionary<Unit, int>();

        #endregion

        #region Methods

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (pause)
            {
                return;
            }

            var size = new Vector2(Menu.Item("size").GetValue<Slider>().Value);
            var boxSize = new Vector2(3 * size.X, 2 * size.Y);
            var menuPosition = new Vector2(
                Menu.Item("posX").GetValue<Slider>().Value,
                Menu.Item("posY").GetValue<Slider>().Value);

            foreach (var pair in unitsSpeed)
            {
                var unit = pair.Key;
                var speed = pair.Value;

                var screenPosition = HUDInfo.GetHPbarPosition(unit);
                if (screenPosition.IsZero)
                {
                    continue;
                }

                Drawing.DrawRect(screenPosition + menuPosition, boxSize, new Color(0, 100, 100, 255));

                var text = speed.ToString("+#;-#;0");
                var textSize = Drawing.MeasureText(text, "Arial", size, FontFlags.DropShadow);

                Drawing.DrawText(
                    text,
                    "Arial",
                    screenPosition + menuPosition
                    + new Vector2((boxSize.X - textSize.X) / 2, (boxSize.Y - textSize.Y) / 2),
                    size,
                    speed > 0 ? Color.Orange : Color.Yellow,
                    FontFlags.DropShadow);
            }
        }

        private static void Events_OnClose(object sender, EventArgs e)
        {
            Drawing.OnDraw -= Drawing_OnDraw;
            Game.OnIngameUpdate -= Game_OnUpdate;
            unitsSpeed.Clear();
        }

        private static void Events_OnLoad(object sender, EventArgs e)
        {
            hero = ObjectManager.LocalHero;
            enemyTeam = hero.GetEnemyTeam();

            Game.OnIngameUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            pause = !Menu.Item("hotkey").GetValue<KeyBind>().Active;

            if (!Utils.SleepCheck("MoveSpeed.UpdateSpeed"))
            {
                return;
            }

            var heroSpeed = hero.MovementSpeed;

            unitsSpeed =
                Heroes.All.Where(
                    x =>
                    (x.Team == enemyTeam || Menu.Item("ally").GetValue<bool>()) && !x.IsIllusion && x.IsVisible
                    && x.IsAlive && !x.Equals(hero)).ToDictionary(x => (Unit)x, x => x.MovementSpeed - heroSpeed);

            var courier =
                ObjectManager.GetEntities<Courier>()
                    .FirstOrDefault(x => x.Team == enemyTeam && x.IsAlive && x.IsVisible);

            if (courier != null)
            {
                unitsSpeed.Add(courier, courier.MovementSpeed - heroSpeed);
            }

            Utils.Sleep(500, "MoveSpeed.UpdateSpeed");
        }

        private static void Main()
        {
            Menu.AddItem(new MenuItem("hotkey", "Change hotkey").SetValue(new KeyBind(18, KeyBindType.Press)));
            Menu.AddItem(new MenuItem("ally", "Show on allies").SetValue(false));
            Menu.AddItem(new MenuItem("size", "Size").SetValue(new Slider(20, 10, 30)));
            Menu.AddItem(new MenuItem("posX", "Position X").SetValue(new Slider(0, -100)));
            Menu.AddItem(new MenuItem("posY", "Position Y").SetValue(new Slider(0, -100)));

            Menu.AddToMainMenu();

            Events.OnLoad += Events_OnLoad;
            Events.OnClose += Events_OnClose;
        }

        #endregion
    }
}