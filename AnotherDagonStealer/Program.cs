using System;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;

namespace AnotherDagonStealer {
    internal class Program {
        private static Hero hero;
        private static Hero heroClone;

        private static Item dagon;
        private static Item dagonClone;

        private static bool inGame;

        private static readonly Menu Menu = new Menu("Another Dagon Stealer", "dagonStealer", true);

        private static readonly int[] DagonDamage = {400, 500, 600, 700, 800};

        private static readonly string[] IgnoreModifiers = {
            "modifier_templar_assassin_refraction_absorb",
            "modifier_item_blade_mail_reflect",
            "modifier_item_lotus_orb_active",
            "modifier_nyx_assassin_spiked_carapace"
        };

        private static void Main() {
            Menu.AddItem(new MenuItem("key", "Enabled").SetValue(new KeyBind('K', KeyBindType.Toggle, true)));
            Menu.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args) {
            if (!Utils.SleepCheck("dagonStealDelay"))
                return;

            if (!inGame) {
                hero = ObjectMgr.LocalHero;

                if (!Game.IsInGame || hero == null) {
                    Utils.Sleep(1000, "dagonStealDelay");
                    return;
                }

                inGame = true;
            }

            if (!Game.IsInGame) {
                inGame = false;
                return;
            }

            dagon = hero.GetDagon();

            if (dagon == null || Game.IsPaused || !Menu.Item("key").GetValue<KeyBind>().Active) {
                Utils.Sleep(200, "dagonStealDelay");
                return;
            }

            if (hero.IsChanneling() || hero.IsInvisible()) {
                Utils.Sleep(200, "dagonStealDelay");
                return;
            }

            if (hero.ClassID == ClassID.CDOTA_Unit_Hero_ArcWarden) {
                heroClone =
                    ObjectMgr.GetEntities<Hero>()
                        .FirstOrDefault(
                            x =>
                                x.IsAlive && x.Team == hero.Team &&
                                x.Modifiers.Any(mod => mod.Name == "modifier_arc_warden_tempest_double"));

                if (heroClone != null) {
                    dagonClone = heroClone.GetDagon();
                }
            }

            var target = ObjectMgr.GetEntities<Hero>().FirstOrDefault(x => CheckTarget(x));

            if (target != null) {
                if (hero.CanUseItems() && dagon.CanBeCasted())
                    dagon.UseAbility(target);
                else if (heroClone != null && dagonClone.CanBeCasted() && heroClone.CanUseItems())
                    dagonClone.UseAbility(target);
            } else if (heroClone != null) {
                if (dagonClone.CanBeCasted() && heroClone.CanUseItems() && dagon.CanBeCasted() && hero.CanUseItems()) {
                    target = ObjectMgr.GetEntities<Hero>().FirstOrDefault(x => CheckTarget(x, true));
                    if (target != null) {
                        dagon.UseAbility(target);
                        dagonClone.UseAbility(target);
                    }
                }
            }

            Utils.Sleep(200, "dagonStealDelay");
        }

        private static bool CheckTarget(Unit enemy, bool doubleOwnage = false) {
            if (enemy.IsIllusion || !enemy.IsValidTarget(dagon.GetCastRange(), true, hero.NetworkPosition))
                return false;

            if (enemy.IsLinkensProtected() || enemy.IsMagicImmune())
                return false;

            if (!enemy.CanDie() || enemy.Modifiers.Any(x => IgnoreModifiers.Any(x.Name.Equals)))
                return false;

            return enemy.Health <
                   enemy.DamageTaken(DagonDamage[dagon.Level - 1] * (doubleOwnage ? 2 : 1), DamageType.Magical, hero);
        }
    }
}
