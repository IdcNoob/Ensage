namespace AnotherDagonStealer
{
    using System;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.AbilityInfo;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using Ensage.Common.Objects.UtilityObjects;

    internal class Program
    {
        #region Static Fields

        private static readonly string[] IgnoreModifiers =
            {
                "modifier_templar_assassin_refraction_absorb",
                "modifier_item_blade_mail_reflect",
                "modifier_item_lotus_orb_active",
                "modifier_nyx_assassin_spiked_carapace"
            };

        private static readonly Menu Menu = new Menu("Another Dagon Stealer", "dagonStealer", true);

        private static Item dagon;

        private static Item dagonClone;

        private static Hero hero;

        private static Hero heroClone;

        private static Sleeper sleeper;

        #endregion

        #region Methods

        private static bool CheckTarget(Unit enemy, bool doubleOwnage = false)
        {
            if (enemy.IsIllusion || !enemy.IsValidTarget(dagon.GetCastRange(), true, hero.NetworkPosition))
            {
                return false;
            }

            if (enemy.IsLinkensProtected() || enemy.IsMagicImmune())
            {
                return false;
            }

            if (!enemy.CanDie() || enemy.Modifiers.Any(x => IgnoreModifiers.Any(x.Name.Equals)))
            {
                return false;
            }

            var damage = AbilityDamage.CalculateDamage(dagon, hero, enemy);

            if (doubleOwnage)
            {
                damage *= 2;
            }

            return enemy.Health < damage;
        }

        private static void Events_OnClose(object sender, EventArgs e)
        {
            Game.OnIngameUpdate -= Game_OnUpdate;
        }

        private static void Events_OnLoad(object sender, EventArgs e)
        {
            hero = ObjectManager.LocalHero;
            sleeper = new Sleeper();
            Game.OnIngameUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            dagon = hero.GetDagon();

            if (dagon == null || Game.IsPaused || !Menu.Item("key").GetValue<KeyBind>().Active || hero.IsChanneling()
                || (hero.IsInvisible() && !hero.IsVisibleToEnemies))
            {
                sleeper.Sleep(200);
                return;
            }

            if (hero.ClassID == ClassID.CDOTA_Unit_Hero_ArcWarden)
            {
                heroClone =
                    ObjectManager.GetEntities<Hero>()
                        .FirstOrDefault(
                            x =>
                            x.IsAlive && x.Team == hero.Team
                            && x.HasModifier("modifier_arc_warden_tempest_double"));

                if (heroClone != null)
                {
                    dagonClone = heroClone.GetDagon();
                }
            }

            var target = ObjectManager.GetEntities<Hero>().FirstOrDefault(x => CheckTarget(x));

            if (target != null)
            {
                if (hero.CanUseItems() && dagon.CanBeCasted())
                {
                    dagon.UseAbility(target);
                }
                else if (heroClone != null && dagonClone.CanBeCasted() && heroClone.CanUseItems())
                {
                    dagonClone.UseAbility(target);
                }
            }
            else if (heroClone != null)
            {
                if (dagonClone.CanBeCasted() && heroClone.CanUseItems() && dagon.CanBeCasted() && hero.CanUseItems())
                {
                    target = ObjectManager.GetEntities<Hero>().FirstOrDefault(x => CheckTarget(x, true));
                    if (target != null)
                    {
                        dagon.UseAbility(target);
                        dagonClone.UseAbility(target);
                    }
                }
            }

            sleeper.Sleep(200);
        }

        private static void Main()
        {
            Menu.AddItem(new MenuItem("key", "Enabled").SetValue(new KeyBind('K', KeyBindType.Toggle, true)));
            Menu.AddToMainMenu();

            Events.OnClose += Events_OnClose;
            Events.OnLoad += Events_OnLoad;
        }

        #endregion
    }
}