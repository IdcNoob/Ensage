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
        private static MenuItem aggro;

        private static MenuItem aggroMove;

        private static Hero currentTarget;

        private static MenuItem enabledHarras;

        private static MenuItem enabledTowerUnnagro;

        private static Hero hero;

        private static Team heroTeam;

        private static Vector3 lastAttackPosition;

        private static Vector3 lastMovePosition;

        private static Unit lastTarget;

        private static Menu menu;

        private static Orbwalker orbwalker;

        private static bool restorePreviousAction;

        private static MenuItem showText;

        private static Sleeper sleeper;

        private static MenuItem textX;

        private static MenuItem textY;

        private static MenuItem towerUnnagroHp;

        private static MenuItem unaggro;

        private static MenuItem unaggroMove;

        private static void Aggro()
        {
            var enemy = ObjectManager.GetEntitiesParallel<Hero>()
                .Where(x => x.IsValid && x.IsAlive && !x.IsInvul() && x.Team != heroTeam)
                .OrderBy(x => hero.FindRelativeAngle(x.Position))
                .FirstOrDefault();

            if (enemy == null)
            {
                return;
            }

            hero.Attack(enemy);

            if (aggroMove.IsActive())
            {
                hero.Move(Game.MousePosition);
            }
            else
            {
                hero.Stop();
            }
        }

        private static void DrawingOnDraw(EventArgs args)
        {
            if (!showText.IsActive())
            {
                return;
            }

            if (enabledHarras.IsActive())
            {
                Drawing.DrawText(
                    "Harass helper",
                    "Arial",
                    new Vector2(textX.GetValue<Slider>().Value, textY.GetValue<Slider>().Value),
                    new Vector2(21),
                    Color.White,
                    FontFlags.None);
            }

            if (enabledTowerUnnagro.IsActive())
            {
                Drawing.DrawText(
                    "Tower unnagro",
                    "Arial",
                    new Vector2(textX.GetValue<Slider>().Value, textY.GetValue<Slider>().Value + 21),
                    new Vector2(21),
                    Color.White,
                    FontFlags.None);
            }
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
                Aggro();
                return;
            }

            if (unaggro.IsActive())
            {
                UnAggro();
                return;
            }

            if (enabledTowerUnnagro.IsActive() && hero.Health < towerUnnagroHp.GetValue<Slider>().Value)
            {
                var tower = ObjectManager.GetEntitiesParallel<Tower>()
                    .FirstOrDefault(
                        x => x.IsValid && x.IsAlive && x.Team != heroTeam && x.AttackTarget is Hero
                             && x.AttackTarget.Equals(hero));

                if (tower != null && !(lastTarget is Hero))
                {
                    var otherUnits = ObjectManager.GetEntitiesParallel<Unit>()
                        .Any(
                            x => x.IsValid && !x.Equals(hero) && x.IsAlive && x.IsSpawned && x.Team == heroTeam
                                 && x.ClassId != ClassId.CDOTA_BaseNPC_Creep_Siege
                                 && x.Distance2D(tower) < tower.AttackRange);

                    if (otherUnits)
                    {
                        UnAggro(true);
                        restorePreviousAction = true;
                        sleeper.Sleep(1000);
                        return;
                    }
                }

                if (tower == null && restorePreviousAction)
                {
                    restorePreviousAction = false;

                    if (!lastMovePosition.IsZero)
                    {
                        hero.Move(lastMovePosition);
                    }
                    else if (!lastAttackPosition.IsZero)
                    {
                        hero.Attack(lastMovePosition);
                    }
                    else if (lastTarget != null && lastTarget.IsValid && lastTarget.IsVisible && lastTarget.IsAlive)
                    {
                        hero.Attack(lastTarget);
                    }
                    else
                    {
                        hero.Stop();
                    }
                }
            }

            if (!enabledHarras.IsActive() || currentTarget == null || !currentTarget.IsAlive)
            {
                return;
            }

            if (hero.Distance2D(currentTarget) > hero.GetAttackRange() && !hero.IsAttacking())
            {
                hero.Move(currentTarget.Position);
            }
            else
            {
                orbwalker.OrbwalkOn(currentTarget);
            }
        }

        private static void Main()
        {
            menu = new Menu("Harass Helper", "harassHelper", true);

            menu.AddItem(
                enabledHarras = new MenuItem("enabled", "Enabled").SetValue(new KeyBind('Z', KeyBindType.Toggle)));
            menu.AddItem(showText = new MenuItem("showText", "Show active text").SetValue(true));
            menu.AddItem(
                textX =
                    new MenuItem("textX", "Text position X").SetValue(new Slider(10, 0, (int)HUDInfo.ScreenSizeX())));
            menu.AddItem(
                textY = new MenuItem("textY", "Text position Y").SetValue(
                    new Slider((int)(HUDInfo.ScreenSizeY() * 0.70), 0, (int)HUDInfo.ScreenSizeY())));

            menu.AddItem(aggro = new MenuItem("aggro", "Aggro key").SetValue(new KeyBind(107, KeyBindType.Press)));
            menu.AddItem(
                unaggro = new MenuItem("unaggro", "Unaggro key").SetValue(new KeyBind(109, KeyBindType.Press)));

            menu.AddItem(
                aggroMove = new MenuItem("aggroMove", "Aggro move").SetValue(false)
                    .SetTooltip("Move to mouse position when using aggro"));
            menu.AddItem(
                unaggroMove = new MenuItem("unaggroMove", "Unaggro move").SetValue(false)
                    .SetTooltip("Move to mouse position when using unaggro"));

            menu.AddItem(
                enabledTowerUnnagro =
                    new MenuItem("enabledTowerUnnagro", "Auto tower unnagro").SetValue(
                        new KeyBind('X', KeyBindType.Toggle)));
            menu.AddItem(
                towerUnnagroHp =
                    new MenuItem("towerUnnagro", "Auto tower unnagro when hp lower than").SetValue(
                        new Slider(600, 0, 2000)));

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
            if (!args.Entities.Contains(hero))
            {
                return;
            }

            currentTarget = null;
            lastTarget = null;
            lastAttackPosition = new Vector3();
            lastMovePosition = new Vector3();

            switch (args.OrderId)
            {
                case OrderId.AttackTarget:
                    lastTarget = args.Target as Unit;

                    if (!enabledHarras.IsActive())
                    {
                        return;
                    }

                    currentTarget = args.Target as Hero;

                    if (currentTarget == null)
                    {
                        return;
                    }
                    if (currentTarget.Team == heroTeam)
                    {
                        currentTarget = null;
                        return;
                    }

                    if (hero.Distance2D(currentTarget) > hero.GetAttackRange())
                    {
                        args.Process = false;
                        sleeper.Sleep(0);
                    }
                    break;
                case OrderId.AttackLocation:
                    lastAttackPosition = args.TargetPosition;
                    break;
                case OrderId.MoveLocation:
                    lastMovePosition = args.TargetPosition;
                    break;
            }
        }

        private static void UnAggro(bool towerUnnagro = false)
        {
            var ally = ObjectManager.GetEntitiesParallel<Creep>()
                .Where(x => x.IsValid && x.IsAlive && x.IsSpawned && x.Team == heroTeam)
                .OrderBy(x => hero.FindRelativeAngle(x.Position))
                .FirstOrDefault();

            if (ally == null)
            {
                return;
            }

            hero.Attack(ally);

            if (towerUnnagro)
            {
                DelayAction.Add(200, () => hero.Stop());
                return;
            }

            if (unaggroMove.IsActive())
            {
                hero.Move(Game.MousePosition);
            }
            else
            {
                hero.Stop();
            }
        }
    }
}