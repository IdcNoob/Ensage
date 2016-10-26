namespace HarassHelper
{
    using System;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using Ensage.Common.Objects.UtilityObjects;

    using SharpDX;

    internal class Program
    {
        #region Static Fields

        private static MenuItem aggro;

        private static MenuItem aggroMove;

        private static MenuItem enabled;

        private static Hero hero;

        private static Team heroTeam;

        private static Menu menu;

        private static Orbwalker orbwalker;

        private static MenuItem showText;

        private static Sleeper sleeper;

        private static Hero target;

        private static MenuItem textX;

        private static MenuItem textY;

        private static MenuItem unaggro;

        private static MenuItem unaggroMove;

        #endregion

        #region Methods

        private static void DrawingOnDraw(EventArgs args)
        {
            if (!showText.IsActive() || !enabled.IsActive())
            {
                return;
            }

            Drawing.DrawText(
                "Harass helper",
                "Arial",
                new Vector2(textX.GetValue<Slider>().Value, textY.GetValue<Slider>().Value),
                new Vector2(21),
                Color.White,
                FontFlags.None);
        }

        private static void GameOnIngameUpdate(EventArgs args)
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            sleeper.Sleep(100);

            if (Game.IsPaused || !hero.IsAlive)
            {
                return;
            }

            if (aggro.IsActive())
            {
                var enemy =
                    ObjectManager.GetEntitiesParallel<Hero>()
                        .Where(x => x.IsValid && x.IsAlive && !x.IsInvul() && x.Team != heroTeam)
                        .OrderBy(x => hero.FindRelativeAngle(x.Position))
                        .FirstOrDefault();

                if (enemy != null)
                {
                    hero.Attack(enemy);
                    if (aggroMove.IsActive())
                    {
                        hero.Move(Game.MousePosition);
                    }
                    else
                    {
                        hero.Stop();
                    }
                    return;
                }
            }

            if (unaggro.IsActive())
            {
                var ally =
                    ObjectManager.GetEntitiesParallel<Creep>()
                        .Where(x => x.IsValid && x.IsAlive && x.IsSpawned && x.Team == heroTeam)
                        .OrderBy(x => hero.FindRelativeAngle(x.Position))
                        .FirstOrDefault();

                if (ally != null)
                {
                    hero.Attack(ally);
                    if (unaggroMove.IsActive())
                    {
                        hero.Move(Game.MousePosition);
                    }
                    else
                    {
                        hero.Stop();
                    }
                    return;
                }
            }

            if (!enabled.IsActive() || target == null || !target.IsAlive)
            {
                return;
            }

            if (hero.Distance2D(target) > hero.GetAttackRange() && !hero.IsAttacking())
            {
                hero.Move(target.Position);
            }
            else
            {
                orbwalker.OrbwalkOn(target);
            }
        }

        private static void Main()
        {
            menu = new Menu("Harass Helper", "harassHelper", true);

            menu.AddItem(enabled = new MenuItem("enabled", "Enabled").SetValue(new KeyBind('Z', KeyBindType.Toggle)));
            menu.AddItem(showText = new MenuItem("showText", "Show active text").SetValue(true));
            menu.AddItem(
                textX = new MenuItem("textX", "Text position X").SetValue(new Slider(10, 0, (int)HUDInfo.ScreenSizeX())));
            menu.AddItem(
                textY =
                new MenuItem("textY", "Text position Y").SetValue(
                    new Slider((int)(HUDInfo.ScreenSizeY() * 0.70), 0, (int)HUDInfo.ScreenSizeY())));

            menu.AddItem(aggro = new MenuItem("aggro", "Aggro key").SetValue(new KeyBind(107, KeyBindType.Press)));
            menu.AddItem(unaggro = new MenuItem("unaggro", "Unaggro key").SetValue(new KeyBind(109, KeyBindType.Press)));

            menu.AddItem(
                aggroMove =
                new MenuItem("aggroMove", "Aggro move").SetValue(false)
                    .SetTooltip("Move to mouse position when using aggro"));
            menu.AddItem(
                unaggroMove =
                new MenuItem("unaggroMove", "Unaggro move").SetValue(false)
                    .SetTooltip("Move to mouse position when using unaggro"));

            menu.AddToMainMenu();

            Events.OnLoad += OnLoad;
            Events.OnClose += OnClose;
        }

        private static void OnClose(object sender, EventArgs eventArgs)
        {
            Game.OnIngameUpdate -= GameOnIngameUpdate;
            Drawing.OnDraw -= DrawingOnDraw;
            Player.OnExecuteOrder -= PlayerOnExecuteOrder;
        }

        private static void OnLoad(object o, EventArgs e)
        {
            hero = ObjectManager.LocalHero;
            sleeper = new Sleeper();
            orbwalker = new Orbwalker(hero);
            heroTeam = hero.Team;

            Game.OnIngameUpdate += GameOnIngameUpdate;
            Drawing.OnDraw += DrawingOnDraw;
            Player.OnExecuteOrder += PlayerOnExecuteOrder;
        }

        private static void PlayerOnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!enabled.IsActive() || !args.Entities.Contains(hero))
            {
                return;
            }

            if (args.Order == Order.AttackTarget)
            {
                target = args.Target as Hero;

                if (target == null)
                {
                    return;
                }
                if (target.Team == heroTeam)
                {
                    target = null;
                    return;
                }

                if (hero.Distance2D(target) > hero.GetAttackRange())
                {
                    args.Process = false;
                    sleeper.Sleep(0);
                }
            }
            else
            {
                target = null;
            }
        }

        #endregion
    }
}