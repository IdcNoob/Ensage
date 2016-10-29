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

        private static float delay;

        private static Hero hero;

        private static AbilityToggler holdAbilities;

        private static bool holdKey;

        private static Menu menu;

        private static Sleeper sleeper;

        private static AbilityToggler toggleAbilities;

        private static bool toggleKey;

        #endregion

        #region Methods

        private static void Game_OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            if (!hero.IsAlive || Game.IsPaused || (!holdKey && !toggleKey))
            {
                sleeper.Sleep(delay);
                return;
            }

            if ((toggleKey && toggleAbilities.IsEnabled("rune_doubledamage"))
                || (holdKey && holdAbilities.IsEnabled("rune_doubledamage")))
            {
                var rune =
                    ObjectManager.GetEntitiesParallel<Rune>()
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
                var aegis = (toggleKey && toggleAbilities.IsEnabled("item_aegis"))
                            || (holdKey && holdAbilities.IsEnabled("item_aegis"));

                var cheese = (toggleKey && toggleAbilities.IsEnabled("item_cheese"))
                             || (holdKey && holdAbilities.IsEnabled("item_cheese"));

                var rapier = (toggleKey && toggleAbilities.IsEnabled("item_rapier"))
                             || (holdKey && holdAbilities.IsEnabled("item_rapier"));

                var gem = (toggleKey && toggleAbilities.IsEnabled("item_gem"))
                          || (holdKey && holdAbilities.IsEnabled("item_gem"));

                var item =
                    ObjectManager.GetEntitiesParallel<PhysicalItem>()
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

            sleeper.Sleep(delay);
        }

        private static void Main()
        {
            menu = new Menu("Another Snatcher", "anotherSnatcher", true);

            var items = new List<string>
                {
                    "item_gem",
                    "item_cheese",
                    "item_rapier",
                    "item_aegis",
                    "rune_doubledamage"
                };

            menu.AddItem(new MenuItem("holdSnatchKey", "Hold key").SetValue(new KeyBind('O', KeyBindType.Press)))
                .ValueChanged += (sender, args) => holdKey = args.GetNewValue<KeyBind>().Active;
            menu.AddItem(
                new MenuItem("enabledStealHold", "Hold steal:").SetValue(
                    holdAbilities = new AbilityToggler(items.ToDictionary(x => x, x => true))));
            menu.AddItem(new MenuItem("pressSnatchKey", "Toggle key").SetValue(new KeyBind('P', KeyBindType.Toggle)))
                .ValueChanged += (sender, args) => toggleKey = args.GetNewValue<KeyBind>().Active;
            menu.AddItem(
                new MenuItem("enabledStealToggle", "Toggle steal:").SetValue(
                    toggleAbilities = new AbilityToggler(items.ToDictionary(x => x, x => true))));
            menu.AddItem(new MenuItem("sleep", "Check delay").SetValue(new Slider(200, 0, 500))).ValueChanged +=
                (sender, args) => delay = args.GetNewValue<Slider>().Value;

            toggleKey = menu.Item("pressSnatchKey").GetValue<KeyBind>().Active;
            delay = menu.Item("sleep").GetValue<Slider>().Value;
            Console.WriteLine(delay);
            menu.AddToMainMenu();

            Events.OnLoad += OnLoad;
            Events.OnClose += OnClose;
        }

        private static void OnClose(object sender, EventArgs e)
        {
            Game.OnUpdate -= Game_OnUpdate;
            Player.OnExecuteOrder -= PlayerOnExecuteOrder;

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