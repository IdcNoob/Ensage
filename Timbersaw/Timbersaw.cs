namespace Timbersaw
{
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using global::Timbersaw.Abilities;

    using SharpDX;

    internal class Timbersaw
    {
        #region Fields

        private readonly List<Chakram> chakrams = new List<Chakram>();

        private readonly Target target = new Target();

        private bool aghsAdded;

        private bool cameraCentered;

        private Chakram chakram;

        private Chakram chakram2;

        private Hero hero;

        private Vector3 heroPosition;

        private MenuManager menuManager;

        private Orbwalker orbwalker;

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

            if (targetParticle == null)
            {
                targetParticle = new ParticleEffect(
                    @"particles\ui_mouseactions\range_finder_tower_aoe.vpcf",
                    target.GetPosition(true));
            }

            targetParticle.SetControlPoint(2, heroPosition = hero.Position);
            targetParticle.SetControlPoint(6, new Vector3(1, 0, 0));
            targetParticle.SetControlPoint(7, target.GetPosition(true));
        }

        public void OnExecuteAbilitiy(Player sender, ExecuteOrderEventArgs args)
        {
            if (!menuManager.IsEnabled || !menuManager.IsSafeChainEnabled)
            {
                return;
            }

            var ability = args.Ability;

            if (ability == null || args.Order != Order.AbilityLocation || !ability.Equals(timberChain.Ability))
            {
                return;
            }

            if (!treeFactory.CheckTree(hero.Position, args.TargetPosition, timberChain.GetCastRange()))
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

            TimberPrediction.OnLoad();

            whirlingDeath = new WhirlingDeath(hero.Spellbook.SpellQ);
            timberChain = new TimberChain(hero.Spellbook.SpellW);
            chakram = new Chakram(
                hero.Spellbook.Spells.First(x => x.Name == "shredder_chakram"),
                hero.Spellbook.Spells.First(x => x.Name == "shredder_return_chakram"));
            chakram2 = new Chakram(
                hero.Spellbook.Spells.First(x => x.Name == "shredder_chakram_2"),
                hero.Spellbook.Spells.First(x => x.Name == "shredder_return_chakram_2"));

            chakrams.Add(chakram);
            //chakrams.Add(chakram2);
        }

        public void OnUpdate()
        {
            if (!Utils.SleepCheck("Timbersaw.Sleep"))
            {
                return;
            }

            if (Game.IsPaused || !hero.IsAlive || !hero.CanCast() || hero.IsChanneling() || !menuManager.IsEnabled)
            {
                return;
            }

            if (!aghsAdded && Utils.SleepCheck("Timbersaw.Aghanim"))
            {
                if (hero.AghanimState())
                {
                    chakrams.Add(chakram2);
                    aghsAdded = true;
                }
                Utils.Sleep(5000, "Timbersaw.Aghanim");
            }

            if (menuManager.EscapeEnabled)
            {
                var blink = hero.FindItem("item_blink", true);

                if (Utils.SleepCheck("Timbersaw.Blink") && blink != null && blink.CanBeCasted())
                {
                    var mouse = Game.MousePosition;
                    var castRange = blink.GetCastRange();

                    var blinkPosition = hero.Distance2D(Game.MousePosition) > castRange
                                            ? (mouse - heroPosition) * castRange / mouse.Distance2D(hero) + heroPosition
                                            : Game.MousePosition;

                    blink.UseAbility(blinkPosition);
                    Utils.Sleep(1000, "Timbersaw.Blink");
                }
                else if (timberChain.CanBeCasted())
                {
                    var chaseTree = treeFactory.GetEscapeTree(hero, Game.MousePosition, timberChain.GetCastRange(), 800);

                    if (chaseTree != null)
                    {
                        timberChain.UseAbility(chaseTree.Position);
                        Utils.Sleep(
                            timberChain.GetSleepTime + chaseTree.Distance2D(hero) / timberChain.Speed * 1000 * 2
                            + Game.Ping,
                            "Timbersaw.Sleep");
                        return;
                    }
                }

                if (Utils.SleepCheck("Timbersaw.Move"))
                {
                    hero.Move(Game.MousePosition);
                    Utils.Sleep(100, "Timbersaw.Move");
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
                        chakram.Stop(hero);
                        Utils.Sleep(ping + 100, "Timbersaw.Sleep");
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
                        Utils.Sleep(ping + 100, "Timbersaw.Sleep");
                        return;
                    }
                }

                chakrams.FirstOrDefault(x => x.ShouldReturn(hero, target))?.Return();

                var blink = hero.FindItem("item_blink", true);
                var shiva = hero.FindItem("item_shivas_guard", true);
                var soulring = hero.FindItem("item_soul_ring");

                var usableChakram = chakrams.FirstOrDefault(x => x.CanBeCasted());

                if (Utils.SleepCheck("Timbersaw.Blink") && blink != null && blink.CanBeCasted() && distanceToEnemy > 400
                    && (target.GetTurnTime(heroPosition) > 0 || distanceToEnemy > 600)
                    && (timberChain.Cooldown > 2 || distanceToEnemy > 800)
                    && (whirlingDeath.CanBeCasted() || usableChakram != null)
                    && hero.Modifiers.All(x => x.Name != timberChain.ModifierName) && !timberChain.IsSleeping)
                {
                    var castRange = blink.GetCastRange();

                    var sleep = ping + 300d;

                    if (distanceToEnemy <= castRange)
                    {
                        var positionNearTree = treeFactory.GetBlinkPosition(
                            target,
                            heroPosition,
                            castRange,
                            whirlingDeath.Radius,
                            whirlingDeath.CanBeCasted());

                        sleep += hero.GetTurnTime(positionNearTree);
                        blink.UseAbility(positionNearTree);
                    }
                    else
                    {
                        var maxDistancePosition = (targetPosition - heroPosition) * castRange
                                                  / targetPosition.Distance2D(hero) + heroPosition;

                        sleep += hero.GetTurnTime(maxDistancePosition);
                        blink.UseAbility(maxDistancePosition);
                    }

                    Utils.Sleep(1000, "Timbersaw.Blink");
                    Utils.Sleep(sleep, "Timbersaw.Sleep");
                    return;
                }

                if (Utils.SleepCheck("Timbersaw.SoulRing") && soulring != null && soulring.CanBeCasted())
                {
                    soulring.UseAbility();
                    Utils.Sleep(1000, "Timbersaw.SoulRing");
                }

                if (Utils.SleepCheck("Timbersaw.Shiva") && shiva != null && shiva.CanBeCasted() && distanceToEnemy < 500)
                {
                    shiva.UseAbility();
                    Utils.Sleep(1000, "Timbersaw.Shiva");
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
                    var possibleDamageTree = treeFactory.GetDamageTree(
                        heroPosition,
                        targetPosition,
                        timberChain.GetCastRange(),
                        timberChain.Radius);

                    if (target.IsVsisible && possibleDamageTree != null)
                    {
                        var predictedPosition = TimberPrediction.PredictedXYZ(
                            target,
                            timberChain.CastPoint * 1000
                            + (distanceToEnemy + hero.Distance2D(possibleDamageTree.Position)) / timberChain.Speed
                            * 1000 + ping);

                        var damageTreeWithPrediction = treeFactory.GetDamageTree(
                            heroPosition,
                            predictedPosition,
                            timberChain.GetCastRange(),
                            timberChain.Radius);

                        if (damageTreeWithPrediction != null)
                        {
                            timberChain.UseAbility(damageTreeWithPrediction.Position, true);
                            timberChain.Position = predictedPosition;
                            timberChain.ChakramCombo = distanceToEnemy > 600 && usableChakram != null;
                            whirlingDeath.Combo = (distanceToEnemy < 400 || distanceToEnemy > 600)
                                                  && whirlingDeath.CanBeCasted();

                            var sleep = timberChain.GetSleepTime
                                        + hero.GetTurnTime(damageTreeWithPrediction.Position) * 1000;

                            if (whirlingDeath.Combo)
                            {
                                whirlingDeath.SetComboDelay(
                                    sleep - ping
                                    + heroPosition.Distance2D(damageTreeWithPrediction) / timberChain.Speed * 1000
                                    + distanceToEnemy / timberChain.Speed * 1000);
                            }

                            Utils.Sleep(sleep + ping, "Timbersaw.Sleep");
                            return;
                        }
                    }
                    else if (timberChain.Level >= 4)
                    {
                        if (target.IsVsisible || distanceToEnemy > 300)
                        {
                            var chaseTree = treeFactory.GetChaseTree(heroPosition, target, timberChain, 300, 500);

                            if (chaseTree != null)
                            {
                                timberChain.UseAbility(chaseTree.Position);
                                Utils.Sleep(
                                    timberChain.GetSleepTime + hero.GetTurnTime(chaseTree.Position) * 1000 + ping,
                                    "Timbersaw.Sleep");
                                return;
                            }
                        }
                    }
                }

                if (usableChakram != null
                    && (distanceToEnemy < 600 || target.GetTurnTime(heroPosition) <= 0 || timberChain.ChakramCombo)
                    && (!whirlingDeath.Combo || !whirlingDeath.ComboDelayPassed)
                    && TimberPrediction.StraightTime(target.Hero) > 500)
                {
                    var predictedPosition = TimberPrediction.PredictedXYZ(
                        target,
                        usableChakram.GetSleepTime + ping
                        + target.GetDistance(heroPosition) / usableChakram.Speed * 1000
                        + usableChakram.Radius / 2 / target.Hero.MovementSpeed * 1000);

                    usableChakram.UseAbility(predictedPosition, target.Hero);

                    timberChain.ChakramCombo = false;

                    Utils.Sleep(
                        usableChakram.GetSleepTime + hero.GetTurnTime(predictedPosition) * 1000 + ping,
                        "Timbersaw.Sleep");
                    return;
                }

                orbwalker.OrbwalkOn(target.Hero, target.GetPosition());
            }
            else if (target.Locked)
            {
                if (cameraCentered)
                {
                    Game.ExecuteCommand("-dota_camera_center_on_hero");
                    cameraCentered = false;
                }

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

                Utils.Sleep(500, "Timbersaw.Sleep");
            }
        }

        #endregion
    }
}