namespace Kunkka
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;
    using Ensage.Common.Objects;

    using global::Kunkka.Abilities;

    using SharpDX;

    internal class Kunkka
    {
        #region Fields

        private readonly List<IAbility> allSpells = new List<IAbility>();

        private bool arrowCasted;

        private double arrowHitTime;

        private bool comboStarted;

        private GhostShip ghostShip;

        private Hero hero;

        private Team heroTeam;

        private bool hookCasted;

        private double hookHitTime;

        private MenuManager menuManager;

        private Hero target;

        private bool targetLocked;

        private ParticleEffect targetParticle;

        private Vector3 targetPosition;

        private TideBringer tideBringer;

        private Torrent torrent;

        private Xmark xMark;

        private Xreturn xReturn;

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            menuManager.OnClose();
            allSpells.Clear();
            targetParticle?.Dispose();
        }

        public void OnDraw()
        {
            if (!menuManager.IsEnabled)
            {
                return;
            }

            if (!targetLocked)
            {
                target = TargetSelector.ClosestToMouse(hero, 600);
            }

            if (target == null || xMark.CastRange < hero.Distance2D(target) || !hero.IsAlive
                || target.IsLinkensProtected() || target.IsMagicImmune())
            {
                if (targetParticle != null)
                {
                    targetParticle.Dispose();
                    targetParticle = null;
                }
                target = null;
                return;
            }

            targetPosition = target.Position;

            if (targetParticle == null)
            {
                targetParticle = new ParticleEffect(@"particles\ui_mouseactions\range_finder_tower_aoe.vpcf", target);
            }

            targetParticle.SetControlPoint(2, hero.Position);
            targetParticle.SetControlPoint(6, new Vector3(1, 0, 0));
            targetParticle.SetControlPoint(7, targetPosition);
        }

        public void OnExecuteAbilitiy(Player sender, ExecuteOrderEventArgs args)
        {
            if (!sender.Equals(hero.Player) || !menuManager.IsEnabled)
            {
                return;
            }

            var ability = args.Ability;

            if (ability == null || !ability.Equals(xMark.Ability))
            {
                return;
            }

            var newTarget = args.Target as Hero;

            if (newTarget == null || newTarget.Team == heroTeam)
            {
                return;
            }

            targetLocked = true;
            target = newTarget;
            xMark.Position = target.Position;
        }

        public void OnLoad()
        {
            hero = ObjectManager.LocalHero;
            heroTeam = hero.Team;

            menuManager = new MenuManager(hero.Name);

            allSpells.Add(torrent = new Torrent(hero.Spellbook.SpellQ));
            allSpells.Add(tideBringer = new TideBringer(hero.Spellbook.SpellW));
            allSpells.Add(xMark = new Xmark(hero.Spellbook.Spells.First(x => x.Name == "kunkka_x_marks_the_spot")));
            allSpells.Add(xReturn = new Xreturn(hero.Spellbook.Spells.First(x => x.Name == "kunkka_return")));
            allSpells.Add(ghostShip = new GhostShip(hero.Spellbook.SpellR));
        }

        public void OnUpdate()
        {
            if (!Utils.SleepCheck("Kunkka.Sleep"))
            {
                return;
            }

            if (Game.IsPaused || !hero.IsAlive || !hero.CanCast() || hero.IsChanneling() || !menuManager.IsEnabled)
            {
                Utils.Sleep(333, "Kunkka.Sleep");
                return;
            }

            if (menuManager.TpHomeEanbled)
            {
                var teleport = hero.FindItem("item_travel_boots")
                               ?? hero.FindItem("item_travel_boots_2") ?? hero.FindItem("item_tpscroll");

                if (teleport != null && teleport.CanBeCasted() && xMark.CanBeCasted)
                {
                    var fountain =
                        ObjectManager.GetEntities<Unit>()
                            .FirstOrDefault(
                                x =>
                                x.Team == heroTeam && x.ClassID == ClassID.CDOTA_Unit_Fountain
                                && x.Distance2D(hero) > 2000);

                    if (fountain == null)
                    {
                        return;
                    }

                    xMark.UseAbility(hero);
                    teleport.UseAbility(fountain, true);
                    Utils.Sleep(1000, "Kunkka.Sleep");
                    return;
                }
            }

            if (menuManager.HitAndRunEnabled)
            {
                var blink = hero.FindItem("item_blink");

                if (blink == null)
                {
                    return;
                }

                var creep =
                    Creeps.All.OrderBy(x => x.Distance2D(Game.MousePosition))
                        .FirstOrDefault(
                            x => x.Team != heroTeam && x.IsSpawned && x.IsVisible && x.Distance2D(hero) < 2000);

                if (xReturn.CanBeCasted && !blink.CanBeCasted()
                    && (creep == null || !creep.IsAlive || tideBringer.Casted))
                {
                    xReturn.UseAbility();
                    Utils.Sleep(500, "Kunkka.Sleep");
                    return;
                }

                if (creep == null || !creep.IsAlive)
                {
                    return;
                }

                if (creep.Distance2D(hero) > 1200)
                {
                    hero.Move(creep.Position);
                    Utils.Sleep(500, "Kunkka.Sleep");
                    return;
                }

                if (xMark.CanBeCasted)
                {
                    xMark.UseAbility(hero);
                    Utils.Sleep(xMark.GetSleepTime, "Kunkka.Sleep");
                    return;
                }

                if (blink.CanBeCasted() && hero.HasModifier("modifier_kunkka_x_marks_the_spot"))
                {
                    blink.UseAbility(creep.Position.Extend(hero.Position, hero.AttackRange));
                    hero.Attack(creep, true);
                    Utils.Sleep(400, "Kunkka.Sleep");
                    return;
                }
            }

            if (menuManager.ComboEnabled)
            {
                var fullCombo = menuManager.FullComboEnabled;

                if (!comboStarted)
                {
                    if (target == null || xMark.CastRange < hero.Distance2D(target))
                    {
                        return;
                    }

                    var manaRequired = allSpells.Where(x => x != ghostShip || fullCombo)
                        .Aggregate(0u, (current, spell) => current + spell.ManaCost);

                    if (manaRequired > hero.Mana)
                    {
                        return;
                    }

                    if (
                        allSpells.Any(
                            x => !x.CanBeCasted && x != xReturn && x != tideBringer && (x != ghostShip || fullCombo)))
                    {
                        return;
                    }

                    targetLocked = true;
                    comboStarted = true;
                }

                if (target == null || !target.IsValid || target.IsMagicImmune())
                {
                    return;
                }

                if (xMark.CanBeCasted)
                {
                    xMark.UseAbility(target);
                    Utils.Sleep(xMark.GetSleepTime, "Kunkka.Sleep");
                    return;
                }

                if (ghostShip.CanBeCasted && fullCombo)
                {
                    if (!hero.AghanimState())
                    {
                        ghostShip.UseAbility(targetPosition);
                        Utils.Sleep(ghostShip.GetSleepTime, "Kunkka.Sleep");
                        return;
                    }

                    if (torrent.Casted
                        && Game.RawGameTime
                        >= torrent.HitTime - ghostShip.CastPoint - xReturn.CastPoint - Game.Ping / 1000)
                    {
                        ghostShip.UseAbility(GetTorrentThinker()?.Position ?? targetPosition);
                    }
                }

                if (torrent.CanBeCasted)
                {
                    torrent.UseAbility(targetPosition);
                    Utils.Sleep(torrent.GetSleepTime, "Kunkka.Sleep");
                    return;
                }

                if (xReturn.CanBeCasted && Game.RawGameTime >= torrent.HitTime - xReturn.CastPoint - Game.Ping / 1000)
                {
                    xReturn.UseAbility();
                    Utils.Sleep(xReturn.GetSleepTime, "Kunkka.Sleep");
                    return;
                }
            }
            else if (comboStarted)
            {
                comboStarted = false;
                targetLocked = false;
            }

            if (xMark.Casted && xReturn.CanBeCasted && menuManager.AutoReturnEnabled && !comboStarted)
            {
                var gameTime = Game.RawGameTime;

                var pudge =
                    Heroes.GetByTeam(heroTeam)
                        .FirstOrDefault(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Pudge && x.IsAlive && !x.IsIllusion);

                if (pudge != null)
                {
                    var hook = pudge.Spellbook.SpellQ;

                    if (hook.IsInAbilityPhase)
                    {
                        if (hookCasted)
                        {
                            return;
                        }

                        hookHitTime = CalculateHitTime(pudge, hook, gameTime, 0);

                        if (hookHitTime > 0)
                        {
                            hookCasted = true;
                        }
                    }
                    else if (hookCasted && hook.AbilityState != AbilityState.OnCooldown)
                    {
                        hookCasted = false;
                    }
                }

                var mirana =
                    Heroes.GetByTeam(heroTeam)
                        .FirstOrDefault(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Mirana && x.IsAlive && !x.IsIllusion);

                if (mirana != null)
                {
                    var arrow = mirana.Spellbook.SpellW;

                    if (arrow.IsInAbilityPhase)
                    {
                        if (arrowCasted)
                        {
                            return;
                        }

                        arrowHitTime = CalculateHitTime(mirana, arrow, gameTime);

                        if (arrowHitTime > 0)
                        {
                            arrowCasted = true;
                        }
                    }
                    else if (arrowCasted && arrow.AbilityState != AbilityState.OnCooldown)
                    {
                        arrowCasted = false;
                    }
                }

                var delay = xReturn.CastPoint + Game.Ping / 1000;

                if (torrent.Casted)
                {
                    var torrentThinker = GetTorrentThinker();

                    if (torrentThinker != null)
                    {
                        if (xMark.Position.Distance2D(torrentThinker) > torrent.Radius)
                        {
                            return;
                        }

                        var modifier = torrentThinker.FindModifier("modifier_kunkka_torrent_thinker");
                        var hitTime = torrent.AdditionalDelay - modifier.ElapsedTime - 0.15;

                        if (hitTime <= delay)
                        {
                            xReturn.UseAbility();
                            targetLocked = false;
                        }
                    }
                }

                if (arrowCasted && gameTime >= arrowHitTime - delay)
                {
                    xReturn.UseAbility();
                    targetLocked = false;
                    arrowCasted = false;
                }

                if (hookCasted && gameTime >= hookHitTime - delay)
                {
                    xReturn.UseAbility();
                    targetLocked = false;
                    hookCasted = false;
                }
            }

            if (targetLocked && xMark.Casted && xReturn.Casted)
            {
                targetLocked = false;
            }

            Utils.Sleep(50, "Kunkka.Sleep");
        }

        #endregion

        #region Methods

        private double CalculateHitTime(Unit unit, Ability ability, float gameTime, float adjustCastPoint = 1)
        {
            var abilityEndPosition = unit.InFront(ability.GetCastRange() + 150);

            var unitAbilityEnd = unit.Distance2D(abilityEndPosition);
            var unitTarget = unit.Distance2D(xMark.Position);
            var targetAbilityEnd = xMark.Position.Distance2D(abilityEndPosition);

            if (Math.Abs(unitTarget + targetAbilityEnd - unitAbilityEnd) < 10)
            {
                return gameTime + ability.FindCastPoint() * adjustCastPoint
                       + (unitTarget - ability.GetRadius()) / ability.GetProjectileSpeed();
            }

            return 0;
        }

        private Unit GetTorrentThinker()
        {
            return
                ObjectManager.GetEntities<Unit>()
                    .FirstOrDefault(
                        x =>
                        x.ClassID == ClassID.CDOTA_BaseNPC && x.HasModifier("modifier_kunkka_torrent_thinker")
                        && x.Team == heroTeam);
        }

        #endregion
    }
}