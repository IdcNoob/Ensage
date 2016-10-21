namespace Timbersaw
{
    using System.Collections.Generic;
    using System.Linq;

    using Abilities;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;
    using Ensage.Common.Objects.UtilityObjects;

    using SharpDX;

    internal class Timbersaw
    {
        #region Fields

        private readonly List<Chakram> chakrams = new List<Chakram>();

        private readonly Target target = new Target();

        private bool aghsAdded;

        private bool cameraCentered;

        private Hero hero;

        private MenuManager menuManager;

        private Orbwalker orbwalker;

        //private ParticleEffect debug;

        private MultiSleeper sleeper;

        private ParticleEffect targetParticle;

        private TimberChain timberChain;

        private TreeFactory treeFactory;

        private WhirlingDeath whirlingDeath;

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            menuManager.OnClose();
            chakrams.Clear();
            TimberPrediction.OnClose();
        }

        public void OnDraw()
        {
            if (!menuManager.IsEnabled)
            {
                return;
            }

            if (!target.Locked)
            {
                target.NewTarget(TargetSelector.ClosestToMouse(hero, 600));
            }

            if (!target.IsValid() || hero.Distance2D(target.GetPosition()) > 1400 && !target.Locked || !hero.IsAlive)
            {
                if (targetParticle != null)
                {
                    targetParticle.Dispose();
                    targetParticle = null;
                }
                return;
            }

            //if (debug == null)
            //{
            //    debug = new ParticleEffect(
            //        @"materials\ensage_ui\particles\drag_selected_ring_mod.vpcf",
            //        target.GetPosition());
            //    debug.SetControlPoint(1, new Vector3(0, 255, 0));
            //    debug.SetControlPoint(2, new Vector3(50, 255, 0));
            //}
            //debug.SetControlPoint(0, TimberPrediction.PredictedXYZ(target, 700));

            if (targetParticle == null)
            {
                targetParticle = new ParticleEffect(
                    @"particles\ui_mouseactions\range_finder_tower_aoe.vpcf",
                    target.GetPosition(true));
            }

            targetParticle.SetControlPoint(2, hero.Position);
            targetParticle.SetControlPoint(6, new Vector3(1, 0, 0));
            targetParticle.SetControlPoint(7, target.GetPosition(true));
        }

        public void OnExecuteAbilitiy(Player sender, ExecuteOrderEventArgs args)
        {
            if (!menuManager.IsEnabled)
            {
                return;
            }

            if (menuManager.ChaseEnabled && args.Order == Order.MoveLocation)
            {
                sleeper.Sleep(500, orbwalker);
            }

            var ability = args.Ability;

            if (ability == null || args.Order != Order.AbilityLocation)
            {
                return;
            }

            var chakram = chakrams.FirstOrDefault(x => x.Ability.Equals(ability));

            if (chakram != null)
            {
                chakram.Position = args.TargetPosition;
                return;
            }

            if (!menuManager.IsSafeChainEnabled && ability.Equals(timberChain.Ability)
                && !treeFactory.CheckTree(hero, args.TargetPosition, timberChain))
            {
                args.Process = false;
            }
        }

        public void OnLoad()
        {
            hero = ObjectManager.LocalHero;
            menuManager = new MenuManager(hero.Name);
            treeFactory = new TreeFactory();
            orbwalker = new Orbwalker(hero);
            sleeper = new MultiSleeper();

            TimberPrediction.OnLoad();

            whirlingDeath = new WhirlingDeath(hero.Spellbook.SpellQ);
            timberChain = new TimberChain(hero.Spellbook.SpellW);

            chakrams.Add(
                new Chakram(
                    hero.Spellbook.Spells.First(x => x.Name == "shredder_chakram"),
                    hero.Spellbook.Spells.First(x => x.Name == "shredder_return_chakram")));
        }

        public void OnUpdate()
        {
            if (sleeper.Sleeping(this))
            {
                return;
            }

            if (Game.IsPaused || !hero.IsAlive || hero.IsChanneling() || !menuManager.IsEnabled)
            {
                return;
            }

            var heroPosition = hero.Position;

            if (!aghsAdded && !sleeper.Sleeping(chakrams))
            {
                if (hero.AghanimState())
                {
                    chakrams.Add(
                        new Chakram(
                            hero.Spellbook.Spells.First(x => x.Name == "shredder_chakram_2"),
                            hero.Spellbook.Spells.First(x => x.Name == "shredder_return_chakram_2")));
                    aghsAdded = true;
                }
                sleeper.Sleep(5000, chakrams);
            }

            if (menuManager.MoveEnabled)
            {
                var blink = hero.FindItem("item_blink", true);
                var mousePosition = Game.MousePosition;

                if (blink != null && !sleeper.Sleeping(blink) && menuManager.IsItemEnabled(blink.StoredName())
                    && blink.CanBeCasted() && mousePosition.Distance2D(hero) > 500)
                {
                    var castRange = blink.GetCastRange();

                    var blinkPosition = hero.Distance2D(mousePosition) > castRange
                                            ? (mousePosition - heroPosition) * castRange
                                              / mousePosition.Distance2D(hero) + heroPosition
                                            : mousePosition;

                    blink.UseAbility(blinkPosition);
                    sleeper.Sleep(1000, blink);
                }
                else if (timberChain.CanBeCasted())
                {
                    var moveTree = treeFactory.GetMoveTree(hero, mousePosition, timberChain.GetCastRange(), 800);

                    if (moveTree != null)
                    {
                        timberChain.UseAbility(moveTree.Position);
                        sleeper.Sleep(
                            timberChain.GetSleepTime + moveTree.Distance2D(hero) / timberChain.Speed * 1000 * 2
                            + Game.Ping,
                            this);
                        return;
                    }
                }

                if (!sleeper.Sleeping(hero))
                {
                    hero.Move(mousePosition);
                    sleeper.Sleep(200, hero);
                }
            }
            else if (menuManager.ChaseEnabled)
            {
                if (!target.IsValid())
                {
                    return;
                }

                if (!target.Locked)
                {
                    target.Locked = true;
                }

                if (!cameraCentered && menuManager.IsCenterCameraEnabled)
                {
                    Game.ExecuteCommand("+dota_camera_center_on_hero");
                    cameraCentered = true;
                }

                var distanceToEnemy = target.GetDistance(heroPosition);
                var targetPosition = target.GetPosition();
                var ping = Game.Ping;

                var phaseChakram = chakrams.FirstOrDefault(x => x.IsInPhase);

                if (phaseChakram != null)
                {
                    var predictedPosition = TimberPrediction.PredictedXYZ(
                        target,
                        distanceToEnemy / phaseChakram.Speed * 1000);

                    if (phaseChakram.Position.Distance2D(predictedPosition) > phaseChakram.Radius)
                    {
                        phaseChakram.Stop(hero);
                        treeFactory.ClearUnavailableTrees(true);
                        sleeper.Sleep(ping + 100, this);
                        return;
                    }
                }

                if (timberChain.IsInPhase && timberChain.CastedOnEnemy)
                {
                    var predictedPosition = TimberPrediction.PredictedXYZ(
                        target,
                        distanceToEnemy / timberChain.Speed * 1000);

                    if (timberChain.Position.Distance2D(predictedPosition) > timberChain.Radius)
                    {
                        timberChain.Stop(hero);
                        timberChain.ChakramCombo = false;
                        whirlingDeath.Combo = false;
                        sleeper.Sleep(ping + 100, this);
                        return;
                    }
                }

                var doubleChakramDamage = chakrams.Count(x => x.Damaging(target)) == 2;
                var returnChakrams = chakrams.Where(x => x.ShouldReturn(hero, target, doubleChakramDamage));

                foreach (var chakram in returnChakrams)
                {
                    chakram.Return();
                    treeFactory.ClearUnavailableTrees(true);
                }

                var blink = hero.FindItem("item_blink", true);
                var shivasGuard = hero.FindItem("item_shivas_guard", true);
                var soulRing = hero.FindItem("item_soul_ring");

                var usableChakram = chakrams.FirstOrDefault(x => x.CanBeCasted());

                if (blink != null && !sleeper.Sleeping(blink) && menuManager.IsItemEnabled(blink.StoredName())
                    && blink.CanBeCasted() && distanceToEnemy > 500
                    && (target.FindAngle(heroPosition) > 0.6 || distanceToEnemy > 700)
                    && (timberChain.Cooldown > 2 || distanceToEnemy > 800)
                    && (whirlingDeath.CanBeCasted() || usableChakram != null)
                    && hero.Modifiers.All(x => x.Name != timberChain.ModifierName) && !timberChain.IsSleeping)
                {
                    var castRange = blink.GetCastRange();

                    var sleep = ping + 300;

                    if (distanceToEnemy <= castRange)
                    {
                        var positionNearTree = treeFactory.GetBlinkPosition(
                            target,
                            heroPosition,
                            castRange,
                            whirlingDeath.Radius,
                            whirlingDeath.CanBeCasted());

                        sleep += (float)hero.GetTurnTime(positionNearTree);
                        blink.UseAbility(positionNearTree);
                    }
                    else
                    {
                        var maxDistancePosition = (targetPosition - heroPosition) * castRange
                                                  / targetPosition.Distance2D(hero) + heroPosition;

                        sleep += (float)hero.GetTurnTime(maxDistancePosition);
                        blink.UseAbility(maxDistancePosition);
                    }
                    sleeper.Sleep(1000, blink);
                    sleeper.Sleep(sleep, this);
                    return;
                }

                if (soulRing != null && !sleeper.Sleeping(soulRing) && menuManager.IsItemEnabled(soulRing.StoredName())
                    && soulRing.CanBeCasted() && hero.Health > 600)
                {
                    soulRing.UseAbility();
                    sleeper.Sleep(1000, soulRing);
                }

                if (shivasGuard != null && !sleeper.Sleeping(shivasGuard)
                    && menuManager.IsItemEnabled(shivasGuard.StoredName()) && shivasGuard.CanBeCasted()
                    && distanceToEnemy < 500)
                {
                    shivasGuard.UseAbility();
                    sleeper.Sleep(1000, shivasGuard);
                }

                if (whirlingDeath.CanBeCasted()
                    && (distanceToEnemy <= whirlingDeath.Radius
                        || (whirlingDeath.Combo && whirlingDeath.ComboDelayPassed)))
                {
                    whirlingDeath.UseAbility();
                    whirlingDeath.Combo = false;
                    timberChain.ChakramCombo = false;
                }

                if (timberChain.CanBeCasted() && (usableChakram == null || distanceToEnemy > 300))
                {
                    if ((blink == null || !blink.CanBeCasted() || !menuManager.IsItemEnabled(blink.Name))
                        && usableChakram != null && !chakrams.Any(x => x.IsSleeping || x.Casted)
                        && distanceToEnemy < chakrams.First().GetCastRange()
                        && treeFactory.TreesInPath(hero, targetPosition, 100) >= 5)
                    {
                        var predictedPosition = TimberPrediction.PredictedXYZ(
                            target,
                            usableChakram.GetSleepTime + ping
                            + target.GetDistance(heroPosition) / usableChakram.Speed * 1000
                            + usableChakram.Radius / 2 / target.Hero.MovementSpeed * 1000);

                        usableChakram.UseAbility(predictedPosition, target.Hero, hero);
                        treeFactory.SetUnavailableTrees(hero.Position, predictedPosition, usableChakram);

                        sleeper.Sleep(usableChakram.GetSleepTime + ping, this);
                        return;
                    }

                    var possibleDamageTree = treeFactory.GetDamageTree(hero, targetPosition, timberChain);

                    if (target.IsVsisible && possibleDamageTree != null)
                    {
                        var predictedPosition = TimberPrediction.PredictedXYZ(
                            target,
                            timberChain.CastPoint * 1000
                            + (distanceToEnemy + hero.Distance2D(possibleDamageTree.Position)) / timberChain.Speed
                            * 1000 + ping);

                        var damageTreeWithPrediction = treeFactory.GetDamageTree(
                            hero,
                            predictedPosition,
                            timberChain,
                            blink != null && !sleeper.Sleeping(blink) && menuManager.IsItemEnabled(blink.Name)
                            && blink.CanBeCasted());

                        if (damageTreeWithPrediction != null)
                        {
                            timberChain.UseAbility(damageTreeWithPrediction.Position, true);
                            timberChain.Position = predictedPosition;
                            timberChain.ChakramCombo = distanceToEnemy > 600 && usableChakram != null;
                            whirlingDeath.Combo = (distanceToEnemy < 400 || distanceToEnemy > 600)
                                                  && whirlingDeath.CanBeCasted();

                            var sleep = timberChain.GetSleepTime
                                        + (float)hero.GetTurnTime(damageTreeWithPrediction.Position) * 1000;

                            if (whirlingDeath.Combo)
                            {
                                whirlingDeath.SetComboDelay(
                                    sleep - ping
                                    + heroPosition.Distance2D(damageTreeWithPrediction) / timberChain.Speed * 1000
                                    + distanceToEnemy / timberChain.Speed * 1000);
                            }

                            sleeper.Sleep(sleep + ping, this);
                            return;
                        }
                    }
                    else if (timberChain.Level >= 4 && hero.Mana > 500)
                    {
                        if (target.IsVsisible || distanceToEnemy > 400)
                        {
                            var chaseTree = treeFactory.GetChaseTree(hero, target, timberChain, 400, 600);

                            if (chaseTree != null)
                            {
                                timberChain.UseAbility(chaseTree.Position);

                                sleeper.Sleep(
                                    timberChain.GetSleepTime + (float)hero.GetTurnTime(chaseTree.Position) * 1000 + ping,
                                    this);
                                return;
                            }
                        }
                    }
                }

                if (usableChakram != null
                    && (distanceToEnemy < 600 || target.FindAngle(heroPosition) <= 0.6 || timberChain.ChakramCombo)
                    && (!whirlingDeath.Combo || !whirlingDeath.ComboDelayPassed)
                    /*&& (TimberPrediction.StraightTime(target.Hero) > 500 || distanceToEnemy < 300)*/)
                {
                    var predictedPosition = TimberPrediction.PredictedXYZ(
                        target,
                        usableChakram.GetSleepTime + ping
                        + target.GetDistance(heroPosition) / usableChakram.Speed * 1000
                        + usableChakram.Radius / 2 / target.Hero.MovementSpeed * 1000);

                    usableChakram.UseAbility(predictedPosition, target.Hero, hero);
                    timberChain.ChakramCombo = false;

                    treeFactory.SetUnavailableTrees(hero.Position, predictedPosition, usableChakram);

                    sleeper.Sleep(
                        usableChakram.GetSleepTime + (float)hero.GetTurnTime(predictedPosition) * 1000 + ping,
                        this);
                    return;
                }

                if (!sleeper.Sleeping(orbwalker))
                {
                    orbwalker.OrbwalkOn(target.Hero, target.GetPosition());
                }
            }
            else if (target.Locked)
            {
                if (cameraCentered)
                {
                    Game.ExecuteCommand("-dota_camera_center_on_hero");
                    cameraCentered = false;
                }

                if (timberChain.IsInPhase)
                {
                    timberChain.Stop(hero);
                }

                treeFactory.ClearUnavailableTrees(true);
                timberChain.ChakramCombo = false;
                whirlingDeath.Combo = false;

                var phaseChakrams = chakrams.Where(x => x.IsInPhase).ToList();
                var castedChakrams = chakrams.Where(x => x.Casted).ToList();

                if (phaseChakrams.Any())
                {
                    phaseChakrams.ForEach(x => x.Stop(hero));
                }
                else if (castedChakrams.Any())
                {
                    castedChakrams.ForEach(x => x.Return());
                }
                else
                {
                    target.Locked = false;
                }

                sleeper.Sleep(500, this);
            }

            treeFactory.ClearUnavailableTrees();
        }

        #endregion
    }
}