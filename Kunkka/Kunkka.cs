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

        private bool comboStarted;

        private GhostShip ghostShip;

        private Hero hero;

        private MenuManager menuManager;

        private ParticleEffect particleEffect;

        private Hero target;

        private bool targetLocked;

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
            particleEffect?.Dispose();
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

            if (target == null || xMark.CastRange < hero.Distance2D(target))
            {
                if (particleEffect != null)
                {
                    particleEffect.Dispose();
                    particleEffect = null;
                }
                return;
            }

            targetPosition = target.Position;

            if (particleEffect == null)
            {
                particleEffect = new ParticleEffect(@"particles\ui_mouseactions\range_finder_tower_aoe.vpcf", target);
            }

            particleEffect.SetControlPoint(2, hero.Position);
            particleEffect.SetControlPoint(6, new Vector3(1, 0, 0));
            particleEffect.SetControlPoint(7, targetPosition);
        }

        public void OnExecuteAbilitiy(Player sender, ExecuteOrderEventArgs args)
        {
            if (!sender.Equals(hero.Player) || !menuManager.IsEnabled)
            {
                return;
            }

            var ability = args.Ability;

            if (ability == null)
            {
                return;
            }

            if (ability == xMark.Ability)
            {
                var heroTarget = args.Target as Hero;
                if (heroTarget != null && heroTarget != hero)
                {
                    targetLocked = true;
                    target = heroTarget;
                    xMark.Position = Game.MousePosition;
                }
            }
            else if (ability == torrent.Ability)
            {
                torrent.Position = Game.MousePosition;
                torrent.CalculateHitTime();
            }
        }

        public void OnLoad()
        {
            hero = ObjectManager.LocalHero;
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
                               ?? hero.FindItem("item_travel_boots_2")
                               ?? hero.FindItem("item_tpscroll");

                if (teleport != null && teleport.CanBeCasted() && xMark.CanBeCasted)
                {
                    var fountain =
                        ObjectManager.GetEntities<Unit>()
                            .FirstOrDefault(
                                x =>
                                x.Team == hero.Team && x.ClassID == ClassID.CDOTA_Unit_Fountain
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
                            x => x.IsAlive && x.IsSpawned && x.IsValid && x.IsVisible && x.Distance2D(hero) < 2000);

                if (creep == null)
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
                    Console.WriteLine("xmark");
                    xMark.UseAbility(hero);
                    Utils.Sleep(xMark.CastPoint * 1000, "Kunkka.Sleep");
                    return;
                }

                if (blink.CanBeCasted())
                {
                    blink.UseAbility(creep.Position.Extend(hero.Position, hero.AttackRange));
                    hero.Attack(creep, true);
                    Utils.Sleep(400, "Kunkka.Sleep");
                    return;
                }

                if (tideBringer.Casted && xReturn.CanBeCasted)
                {
                    xReturn.UseAbility();
                    Utils.Sleep(500, "Kunkka.Sleep");
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

                    targetLocked = true;

                    var manaRequired = allSpells.Where(x => x != ghostShip || fullCombo)
                        .Aggregate(0u, (current, spell) => current + spell.ManaCost);

                    if (manaRequired > hero.Mana)
                    {
                        return;
                    }

                    if (
                        allSpells.Any(
                            x => !x.CanBeCasted && (x != xReturn && x != tideBringer || (x == ghostShip && !fullCombo))))
                    {
                        return;
                    }

                    targetPosition = target.NetworkPosition;
                    comboStarted = true;
                }

                if (target == null || !target.IsValid || target.IsMagicImmune())
                {
                    return;
                }

                if (xMark.CanBeCasted)
                {
                    xMark.UseAbility(target);
                    Utils.Sleep(xMark.CastPoint * 1000 + Game.Ping, "Kunkka.Sleep");
                    return;
                }

                if (ghostShip.CanBeCasted && fullCombo)
                {
                    ghostShip.UseAbility(targetPosition);
                    Utils.Sleep(ghostShip.CastPoint * 1000 + Game.Ping, "Kunkka.Sleep");
                    return;
                }

                if (torrent.CanBeCasted)
                {
                    torrent.UseAbility(targetPosition);
                    Utils.Sleep(torrent.CastPoint * 1000 + Game.Ping, "Kunkka.Sleep");
                    return;
                }

                if (xReturn.CanBeCasted && Game.GameTime >= torrent.HitTime - xReturn.CastPoint - Game.Ping / 1000)
                {
                    xReturn.UseAbility();
                    Utils.Sleep(xReturn.CastPoint * 1000 + Game.Ping, "Kunkka.Sleep");
                    return;
                }
            }
            else if (comboStarted)
            {
                comboStarted = false;
                targetLocked = false;
            }

            if (torrent.Casted && xMark.Casted && xReturn.CanBeCasted && menuManager.AutoReturnEnabled)
            {
                if (xMark.Position.Distance2D(torrent.Position) > torrent.Radius)
                {
                    targetLocked = false;
                    return;
                }

                if (Game.GameTime >= torrent.HitTime - xReturn.CastPoint - Game.Ping / 1000)
                {
                    xReturn.UseAbility();
                    targetLocked = false;
                }
            }

            if (targetLocked && xMark.Casted && xReturn.Casted)
            {
                targetLocked = false;
            }

            Utils.Sleep(100, "Kunkka.Sleep");
        }

        #endregion
    }
}