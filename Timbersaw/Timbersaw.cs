namespace Timbersaw
{
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    using global::Timbersaw.Abilities;

    using SharpDX;

    internal class Timbersaw
    {
        #region Fields

        private readonly List<Chakram> chakrams = new List<Chakram>();

        private readonly Target target = new Target();

        private bool aghsAdded;

        private Chakram chakram;

        private Chakram chakram2;

        private Hero hero;

        private MenuManager menuManager;

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
                target.RemoveTarget();
                return;
            }

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

        public void OnLoad()
        {
            hero = ObjectManager.LocalHero;
            menuManager = new MenuManager(hero.Name);
            treeFactory = new TreeFactory();

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

            if (menuManager.ComboEnabled)
            {
                if (!target.IsValid())
                {
                    target.Locked = false;
                    return;
                }

                if (!target.Locked)
                {
                    target.Locked = true;
                }

                var distanceToEnemy = target.GetDistance(hero.NetworkPosition);
                var targetPosition = target.GetPosition();

                var phaseChakram = chakram.IsInPhase ? chakram : chakram2.IsInPhase ? chakram2 : null;

                if (phaseChakram != null)
                {
                    var predictedPosition = TimberPrediction.PredictedXYZ(
                        target,
                        distanceToEnemy / phaseChakram.Speed * 1000);

                    if (phaseChakram.Position.Distance2D(predictedPosition) > phaseChakram.Radius)
                    {
                        chakram.Stop(hero);
                        Utils.Sleep(Game.Ping + 100, "Timbersaw.Sleep");
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
                        Utils.Sleep(Game.Ping + 100, "Timbersaw.Sleep");
                        return;
                    }
                }

                var blink = hero.FindItem("item_blink", true);
                var shiva = hero.FindItem("item_shivas_guard", true);
                var soulring = hero.FindItem("item_soul_ring");

                if (Utils.SleepCheck("Timbersaw.Blink") && blink != null && blink.CanBeCasted() && distanceToEnemy > 400
                    && (timberChain.Cooldown > 2 || distanceToEnemy > 800)
                    && hero.Modifiers.All(x => x.Name != timberChain.ModifierName) && !timberChain.IsSleeping)
                {
                    var castRange = blink.GetCastRange();

                    if (distanceToEnemy <= castRange)
                    {
                        var positionNearTree = treeFactory.GetBlinkPosition(
                            targetPosition,
                            hero.Position,
                            distanceToEnemy,
                            whirlingDeath.Radius);

                        blink.UseAbility(positionNearTree);

                        if (whirlingDeath.CanBeCasted())
                        {
                            whirlingDeath.UseAbility(true);
                            Utils.Sleep(Game.Ping + 150, "Timbersaw.Sleep");
                        }
                    }
                    else
                    {
                        var maxDistancePosition = (targetPosition - hero.Position) * castRange
                                                  / targetPosition.Distance2D(hero) + hero.Position;
                        blink.UseAbility(maxDistancePosition);
                    }

                    Utils.Sleep(1000, "Timbersaw.Blink");
                    return;
                }

                if (Utils.SleepCheck("Timbersaw.SoulRing") && soulring != null && soulring.CanBeCasted())
                {
                    soulring.UseAbility();
                    Utils.Sleep(1000, "Timbersaw.SoulRing");
                }

                if (Utils.SleepCheck("Timbersaw.Shiva") && shiva != null && shiva.CanBeCasted()
                    && distanceToEnemy < shiva.GetCastRange())
                {
                    shiva.UseAbility();
                    Utils.Sleep(1000, "Timbersaw.Shiva");
                }

                if (whirlingDeath.CanBeCasted() && distanceToEnemy <= whirlingDeath.Radius)
                {
                    whirlingDeath.UseAbility();
                }

                var usableChakram = chakrams.FirstOrDefault(x => x.CanBeCasted());

                if (timberChain.CanBeCasted() && (usableChakram == null || distanceToEnemy > 300))
                {
                    var possibleDamageTree = treeFactory.GetDamageTree(
                        hero.Position,
                        targetPosition,
                        timberChain.GetCastRange(),
                        timberChain.Radius);

                    if (target.IsVsisible && possibleDamageTree != null)
                    {
                        var pos = TimberPrediction.PredictedXYZ(
                            target,
                            timberChain.CastPoint * 1000
                            + (distanceToEnemy + hero.Distance2D(possibleDamageTree.Position)) / timberChain.Speed
                            * 1000 + Game.Ping);

                        var damageTreeWithPrediction = treeFactory.GetDamageTree(
                            hero.Position,
                            pos,
                            timberChain.GetCastRange(),
                            timberChain.Radius);

                        if (damageTreeWithPrediction != null)
                        {
                            timberChain.UseAbility(damageTreeWithPrediction.Position, true);
                            timberChain.Position = pos;
                            Utils.Sleep(timberChain.GetSleepTime, "Timbersaw.Sleep");
                            return;
                        }
                    }
                    else
                    {
                        if (distanceToEnemy > 600)
                        {
                            var chaseTree = treeFactory.GetChaseTree(
                                hero.Position,
                                target,
                                timberChain.GetCastRange(),
                                600);

                            if (chaseTree != null)
                            {
                                timberChain.UseAbility(chaseTree.Position);
                                Utils.Sleep(timberChain.GetSleepTime, "Timbersaw.Sleep");
                                return;
                            }
                        }
                    }
                }

                if (usableChakram != null && (distanceToEnemy < 500 || timberChain.CanBeCasted()))
                {
                    var pos = TimberPrediction.PredictedXYZ(
                        target,
                        usableChakram.GetSleepTime + Game.Ping
                        + target.GetDistance(hero.Position) / usableChakram.Speed * 1000
                        + usableChakram.Radius / 2 / target.Hero.MovementSpeed * 1000);

                    usableChakram.UseAbility(pos, target.Hero);
                    Utils.Sleep(usableChakram.GetSleepTime, "Timbersaw.Sleep");
                    return;
                }

                var usedChakram = chakrams.FirstOrDefault(x => x.ShouldReturn(targetPosition));
                usedChakram?.Return();

                if (Utils.SleepCheck("Timbersaw.Move"))
                {
                    hero.Move(targetPosition);
                    Utils.Sleep(100, "Timbersaw.Move");
                }
            }
            else if (target.Locked)
            {
                target.Locked = false;

                var usedChakrams = chakrams.Where(x => x.Casted);
                foreach (var usedChakram in usedChakrams)
                {
                    usedChakram.Return();
                }
            }
        }

        #endregion
    }
}