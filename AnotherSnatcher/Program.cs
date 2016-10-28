namespace AnotherSnatcher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using Ensage.Common.Objects.UtilityObjects;

    internal class Program
    {
        #region Static Fields

        private static readonly List<uint> DroppedItems = new List<uint>();

        private static Hero hero;

        private static Menu menu;

        private static Sleeper sleeper;

        #endregion

        #region Methods

        private static void Game_OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            var holdKey = menu.Item("holdSnatchKey").GetValue<KeyBind>().Active;
            var toggleKey = menu.Item("pressSnatchKey").GetValue<KeyBind>().Active;

            if (!hero.IsAlive || Game.IsPaused || (!holdKey && !toggleKey))
            {
                sleeper.Sleep(menu.Item("sleep").GetValue<Slider>().Value);
                return;
            }

            if ((toggleKey && menu.Item("enabledStealToggle").GetValue<AbilityToggler>().IsEnabled("rune_doubledamage"))
                || (holdKey && menu.Item("enabledStealHold").GetValue<AbilityToggler>().IsEnabled("rune_doubledamage")))
            {
                var rune = ObjectManager.GetEntities<Rune>()
                    .FirstOrDefault(x => x.IsVisible && x.Distance2D(hero) < 400);
                if (rune != null)
                {
                    hero.PickUpRune(rune);
                    sleeper.Sleep(500);
                    return;
                }
            }

            if (hero.Inventory.FreeSlots.Any())
            {
                var aegis = (toggleKey
                             && menu.Item("enabledStealToggle").GetValue<AbilityToggler>().IsEnabled("item_aegis"))
                            || (holdKey
                                && menu.Item("enabledStealHold").GetValue<AbilityToggler>().IsEnabled("item_aegis"));

                var cheese = (toggleKey
                              && menu.Item("enabledStealToggle").GetValue<AbilityToggler>().IsEnabled("item_cheese"))
                             || (holdKey
                                 && menu.Item("enabledStealHold").GetValue<AbilityToggler>().IsEnabled("item_cheese"));

                var rapier = (toggleKey
                              && menu.Item("enabledStealToggle").GetValue<AbilityToggler>().IsEnabled("item_rapier"))
                             || (holdKey
                                 && menu.Item("enabledStealHold").GetValue<AbilityToggler>().IsEnabled("item_rapier"));

                var gem = (toggleKey && menu.Item("enabledStealToggle").GetValue<AbilityToggler>().IsEnabled("item_gem"))
                          || (holdKey && menu.Item("enabledStealHold").GetValue<AbilityToggler>().IsEnabled("item_gem"));

                var item =
                    ObjectManager.GetEntities<PhysicalItem>()
                        .FirstOrDefault(
                            x =>
                            x.IsVisible && x.Distance2D(hero) < 400 && !DroppedItems.Contains(x.Item.Handle)
                            && ((aegis && x.Item.Name == "item_aegis") || (rapier && x.Item.Name == "item_rapier")
                                || (cheese && x.Item.Name == "item_cheese") || (gem && x.Item.Name == "item_gem")));

                if (item != null)
                {
                    hero.PickUpItem(item);
                    sleeper.Sleep(500);
                    return;
                }
            }
            sleeper.Sleep(menu.Item("sleep").GetValue<Slider>().Value);
        }

        private static void Main()
        {
            menu = new Menu("Another Snatcher", "anotherSnatcher", true);

            var itemsToggle = new Dictionary<string, bool>
                {
                    { "item_gem", true },
                    { "item_cheese", true },
                    { "item_rapier", true },
                    { "item_aegis", true },
                    { "rune_doubledamage", true }
                };
            var itemsHold = new Dictionary<string, bool>(itemsToggle);

            menu.AddItem(new MenuItem("holdSnatchKey", "Hold key").SetValue(new KeyBind('O', KeyBindType.Press)));
            menu.AddItem(new MenuItem("enabledStealHold", "Hold steal:").SetValue(new AbilityToggler(itemsToggle)));
            menu.AddItem(new MenuItem("pressSnatchKey", "Toggle key").SetValue(new KeyBind('P', KeyBindType.Toggle)));
            menu.AddItem(new MenuItem("enabledStealToggle", "Toggle steal:").SetValue(new AbilityToggler(itemsHold)));
            menu.AddItem(new MenuItem("sleep", "Check delay").SetValue(new Slider(200, 0, 500)));

            menu.AddToMainMenu();

            Events.OnLoad += OnLoad;
            Events.OnClose += OnClose;
        }

        private static void OnClose(object sender, EventArgs e)
        {
            Game.OnUpdate -= Game_OnUpdate;
            DroppedItems.Clear();
        }

        private static void OnLoad(object sender, EventArgs e)
        {
            hero = ObjectManager.LocalHero;
            sleeper = new Sleeper();

            Game.OnUpdate += Game_OnUpdate;
            Player.OnExecuteOrder += PlayerOnExecuteOrder;
        }

        private static void PlayerOnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!args.Entities.Contains(hero))
            {
                return;
            }

            if (args.Order == Order.DropItem)
            {
                DroppedItems.Add(args.Ability.Handle);
            }
            else if (args.Order == Order.PickItem)
            {
                var physicalItem = args.Target as PhysicalItem;
                if (physicalItem != null)
                {
                    DroppedItems.RemoveAll(x => x == physicalItem.Item.Handle);
                }
            }
        }

        #endregion
    }
}