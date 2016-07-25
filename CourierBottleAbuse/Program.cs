namespace CourierBottleAbuse
{
    using System;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using Ensage.Common.Objects.UtilityObjects;

    internal class Program
    {
        #region Static Fields

        private static readonly Menu Menu = new Menu("Courier Bottle Abuse", "bottleAbuse", true);

        private static bool enabled;

        private static bool following;

        private static Hero hero;

        private static Sleeper sleeper;

        #endregion

        #region Methods

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

            if (Game.IsPaused || !Game.IsInGame || !enabled)
            {
                sleeper.Sleep(1000);
                return;
            }

            if (!hero.IsAlive)
            {
                enabled = false;
                return;
            }

            var courier = ObjectManager.GetEntities<Courier>().FirstOrDefault(x => x.IsAlive && x.Team == hero.Team);

            if (courier == null)
            {
                enabled = false;
                return;
            }

            var bottle = hero.Inventory.Items.FirstOrDefault(x => x.Name == "item_bottle");
            var courBottle = courier.Inventory.Items.FirstOrDefault(x => x.Name == "item_bottle");

            if (bottle == null && courBottle == null)
            {
                enabled = false;
                return;
            }

            var distance = hero.Distance2D(courier);

            if (distance > 200 && !following)
            {
                if (Menu.Item("stashBefore").GetValue<bool>() && hero.Inventory.StashItems.Any())
                {
                    if (courier.HasModifier("modifier_fountain_aura_buff")
                        || !Menu.Item("stashLimitBefore").GetValue<bool>())
                    {
                        courier.Spellbook.SpellD.UseAbility();
                        courier.Spellbook.SpellF.UseAbility(true);
                        courier.Follow(hero, true);
                    }
                    else
                    {
                        courier.Follow(hero);
                    }
                }
                else
                {
                    courier.Follow(hero);
                }
                following = true;
            }
            else if (distance <= 200 && bottle != null && bottle.CurrentCharges == 0)
            {
                hero.Stop();
                hero.GiveItem(bottle, courier);
            }
            else if (courBottle != null)
            {
                courier.Spellbook.SpellQ.UseAbility();
                var burst = courier.Spellbook.SpellR;
                if (courier.IsFlying && burst.CanBeCasted())
                {
                    burst.UseAbility();
                }
                if (Menu.Item("stashAfter").GetValue<bool>() && hero.Inventory.StashItems.Any())
                {
                    courier.Spellbook.SpellD.UseAbility(true);
                }
                courier.Spellbook.SpellF.UseAbility(true);
                enabled = false;
            }

            sleeper.Sleep(333);
        }

        private static void Main()
        {
            Menu.AddItem(new MenuItem("hotkey", "Hotkey").SetValue(new KeyBind('Z', KeyBindType.Press)))
                .ValueChanged += (sender, args) =>
                    {
                        if (args.GetNewValue<KeyBind>().Active)
                        {
                            enabled = true;
                            following = false;
                        }
                    };

            var stashItems = new Menu("Take items from stash", "stash");
            stashItems.AddItem(
                new MenuItem("stashBefore", "Before").SetValue(true)
                    .SetTooltip("Will bring items from stash before abusing bottle"));
            stashItems.AddItem(
                new MenuItem("stashLimitBefore", "Limit before").SetValue(true)
                    .SetTooltip("Will take items only if couier in base, when \"Before\" active"));
            stashItems.AddItem(
                new MenuItem("stashAfter", "After").SetValue(true)
                    .SetTooltip("Will bring items from stash with refilled bottle"));

            Menu.AddSubMenu(stashItems);
            Menu.AddToMainMenu();

            Events.OnLoad += Events_OnLoad;
            Events.OnClose += Events_OnClose;
        }

        #endregion
    }
}