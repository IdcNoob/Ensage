using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using Attribute = Ensage.Attribute;

namespace WinterIsComing {
    internal class Program {
        private static bool inGame;
        private static Hero hero;

        private static readonly Menu Menu = new Menu("Winter is Coming", "winterIsComing", true);
        private static readonly Dictionary<string, bool> EnemiesUlt = new Dictionary<string, bool>();
        private static readonly Dictionary<string, bool> AlliesLowHp = new Dictionary<string, bool>();
        private static readonly Dictionary<string, bool> AlliesDisabled = new Dictionary<string, bool>();

        private static void Main() {
            var autoHealMenu = new Menu("Auto heal", "autoHeal");

            autoHealMenu.AddItem(new MenuItem("autoHealWhenDisabled", "When disabled"));
            autoHealMenu.AddItem(new MenuItem("autoHealWhenLowHP", "When low HP"));
            autoHealMenu.AddItem(
                new MenuItem("autoHealWhenLowHPThreshold", "Low HP% threshold").SetValue(new Slider(30, 0, 99)));
            autoHealMenu.AddItem(
                new MenuItem("autoHealWhenLowHPRange", "Auto heal low HP if enemy in range").SetValue(new Slider(500, 0,
                    2000)).SetTooltip("If set to 0 range will be ignored"));

            var autoUltMenu = new Menu("Auto ultimate", "autoUltMenu");

            autoUltMenu.AddItem(new MenuItem("autoUlt", "Auto ultimate"))
                .SetValue(new HeroToggler(EnemiesUlt, true, false, false)).DontSave();
            autoUltMenu.AddItem(new MenuItem("autoUltEnemies", "When enemies near").SetValue(new Slider(2, 0, 4)));

            Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));

            Menu.AddSubMenu(autoHealMenu);
            Menu.AddSubMenu(autoUltMenu);
            Menu.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args) {
            if (!Utils.SleepCheck("WinterIsComingDelay"))
                return;

            if (!Menu.Item("enabled").GetValue<bool>()) {
                Utils.Sleep(1000, "WinterIsComingDelay");
                return;
            }

            if (!inGame) {
                hero = ObjectMgr.LocalHero;

                if (!Game.IsInGame || hero == null || hero.ClassID != ClassID.CDOTA_Unit_Hero_Winter_Wyvern) {
                    Utils.Sleep(1000, "WinterIsComingDelay");
                    return;
                }

                EnemiesUlt.Clear();
                AlliesLowHp.Clear();
                AlliesDisabled.Clear();

                inGame = true;
            }

            if (!Game.IsInGame) {
                inGame = false;
                return;
            }

            var sleep = 250;

            var reloadEnemyMenu = false;
            var reloadAllyMenu = false;

            var allHeroes = ObjectMgr.GetEntities<Hero>().Where(x => !x.IsIllusion).ToList();

            var allEnemies = allHeroes.Where(x => x.Team == hero.GetEnemyTeam()).ToList();
            var allAllies = allHeroes.Where(x => x.Team == hero.Team).ToList();

            foreach (var enemy in allEnemies.Where(enemy => !EnemiesUlt.ContainsKey(enemy.Name))) {
                EnemiesUlt.Add(enemy.Name, false);
                reloadEnemyMenu = true;
            }

            foreach (var ally in allAllies.Where(enemy => !AlliesLowHp.ContainsKey(enemy.Name))) {
                AlliesLowHp.Add(ally.Name, true);
                AlliesDisabled.Add(ally.Name, true);

                reloadAllyMenu = true;
            }

            if (reloadEnemyMenu)
                Menu.Item("autoUlt").SetValue(new HeroToggler(EnemiesUlt, true, false, false)).DontSave();

            if (reloadAllyMenu) {
                Menu.Item("autoHealWhenDisabled").SetValue(new HeroToggler(AlliesDisabled, false, true)).DontSave();
                Menu.Item("autoHealWhenLowHP").SetValue(new HeroToggler(AlliesLowHp, false, true)).DontSave();
            }

            if (!hero.IsAlive || Game.IsPaused || !hero.CanCast()) {
                Utils.Sleep(sleep, "WinterIsComingDelay");
                return;
            }

            var ult = hero.Spellbook.SpellR;
            var heal = hero.Spellbook.SpellE;

            var enemies = allEnemies.Where(x => x.IsVisible && x.IsAlive).ToList();

            if (ult.CanBeCasted()) {
                var ultTarget =
                    enemies.FirstOrDefault(
                        enemy =>
                            Menu.Item("autoUlt").GetValue<HeroToggler>().IsEnabled(enemy.Name) &&
                            enemy.IsValidTarget(ult.CastRange, false, hero.NetworkPosition) &&
                            enemy.Modifiers.All(mod => !mod.Name.StartsWith("modifier_winter_wyvern_winters_curse_aura")) &&
                            enemies.Count(x => x.Distance2D(enemy) <= 400) >
                            Menu.Item("autoUltEnemies").GetValue<Slider>().Value);

                if (ultTarget != null) {
                    if (ultTarget.IsLinkensProtected()) {
                        //break linkens ?
                        //ult.UseAbility(ultTarget, true);
                    } else {
                        ult.UseAbility(ultTarget);
                    }
                }
            }

            var allies =
                allAllies.Where(x => x.IsValidTarget(2000, false, hero.NetworkPosition))
                    .OrderBy(x => (float) x.Health / x.MaximumHealth)
                    .ToList();

            var lowHpAlly = allies.FirstOrDefault(x => IsLowHp(x) && IsEnemyNear(x, enemies));
            var disabledAlly = allies.FirstOrDefault(IsDisbled);

            if (heal.CanBeCasted()) {
                if (lowHpAlly != null && !lowHpAlly.IsMagicImmune() && !lowHpAlly.IsChanneling() &&
                    hero.Distance2D(lowHpAlly) <= heal.CastRange) {
                    heal.UseAbility(lowHpAlly);
                    sleep = 400;
                } else if (disabledAlly != null && hero.Distance2D(disabledAlly) <= heal.CastRange) {
                    heal.UseAbility(disabledAlly);
                    sleep = 400;
                }
            }

            Utils.Sleep(sleep, "WinterIsComingDelay");
        }

        private static bool IsLowHp(Entity unit) {
            return ((float) unit.Health / unit.MaximumHealth) * 100 <=
                   Menu.Item("autoHealWhenLowHPThreshold").GetValue<Slider>().Value &&
                   Menu.Item("autoHealWhenLowHP").GetValue<HeroToggler>().IsEnabled(unit.Name);
        }

        private static bool IsEnemyNear(Entity unit, IEnumerable<Hero> enemies) {
            var range = Menu.Item("autoHealWhenLowHPRange").GetValue<Slider>().Value;
            return range == 0 || enemies.Any(x => x.IsValidTarget(range, false, unit.NetworkPosition));
        }

        private static bool IsDisbled(Hero unit) {
            return (unit.IsHexed() || unit.IsStunned() ||
                    (unit.IsSilenced() && unit.PrimaryAttribute == Attribute.Intelligence) ||
                    unit.Modifiers.Any(
                        x =>
                            x.Name == "modifier_doom_bringer_doom" || x.Name == "modifier_axe_berserkers_call" ||
                            (x.Name == "modifier_legion_commander_duel" &&
                             unit.ClassID != ClassID.CDOTA_Unit_Hero_Legion_Commander)) &&
                    Menu.Item("autoHealWhenDisabled").GetValue<HeroToggler>().IsEnabled(unit.Name)) &&
                   unit.Modifiers.All(x => x.Name != "modifier_winter_wyvern_cold_embrace");
        }
    }
}