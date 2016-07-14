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

        private static readonly Menu Menu = new Menu("Another Snatcher", "anotherSnatcher", true);

        private static Hero hero;

        private static Sleeper sleeper;

        #endregion

        #region Methods

        private static void Game_OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            var holdKey = Menu.Item("holdSnatchKey").GetValue<KeyBind>().Active;
            var toggleKey = Menu.Item("pressSnatchKey").GetValue<KeyBind>().Active;

            if (!hero.IsAlive || Game.IsPaused || (!holdKey && !toggleKey))
            {
                sleeper.Sleep(Menu.Item("sleep").GetValue<Slider>().Value);
                return;
            }

            if ((toggleKey && Menu.Item("enabledStealToggle").GetValue<AbilityToggler>().IsEnabled("rune_doubledamage"))
                || (holdKey && Menu.Item("enabledStealHold").GetValue<AbilityToggler>().IsEnabled("rune_doubledamage")))
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
                             && Menu.Item("enabledStealToggle").GetValue<AbilityToggler>().IsEnabled("item_aegis"))
                            || (holdKey
                                && Menu.Item("enabledStealHold").GetValue<AbilityToggler>().IsEnabled("item_aegis"));

                var cheese = (toggleKey
                              && Menu.Item("enabledStealToggle").GetValue<AbilityToggler>().IsEnabled("item_cheese"))
                             || (holdKey
                                 && Menu.Item("enabledStealHold").GetValue<AbilityToggler>().IsEnabled("item_cheese"));

                var rapier = (toggleKey
                              && Menu.Item("enabledStealToggle").GetValue<AbilityToggler>().IsEnabled("item_rapier"))
                             || (holdKey
                                 && Menu.Item("enabledStealHold").GetValue<AbilityToggler>().IsEnabled("item_rapier"));

                var gem = (toggleKey && Menu.Item("enabledStealToggle").GetValue<AbilityToggler>().IsEnabled("item_gem"))
                          || (holdKey && Menu.Item("enabledStealHold").GetValue<AbilityToggler>().IsEnabled("item_gem"));

                var item =
                    ObjectManager.GetEntities<PhysicalItem>()
                        .FirstOrDefault(
                            x =>
                            x.IsVisible && x.Distance2D(hero) < 400
                            && ((aegis && x.Item.Name == "item_aegis") || (rapier && x.Item.Name == "item_rapier")
                                || (cheese && x.Item.Name == "item_cheese") || (gem && x.Item.Name == "item_gem")));

                if (item != null)
                {
                    hero.PickUpItem(item);
                    sleeper.Sleep(500);
                    return;
                }
            }
            sleeper.Sleep(Menu.Item("sleep").GetValue<Slider>().Value);
        }

        private static void Main()
        {
            var itemsToggle = new Dictionary<string, bool>
                                  {
                                      { "item_gem", true }, { "item_cheese", true }, { "item_rapier", true },
                                      { "item_aegis", true }, { "rune_doubledamage", true }
                                  };
            var itemsHold = new Dictionary<string, bool>(itemsToggle);

            Menu.AddItem(new MenuItem("holdSnatchKey", "Hold key").SetValue(new KeyBind('O', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("enabledStealHold", "Hold steal:").SetValue(new AbilityToggler(itemsToggle)));
            Menu.AddItem(new MenuItem("pressSnatchKey", "Toggle key").SetValue(new KeyBind('P', KeyBindType.Toggle)));
            Menu.AddItem(new MenuItem("enabledStealToggle", "Toggle steal:").SetValue(new AbilityToggler(itemsHold)));
            Menu.AddItem(new MenuItem("sleep", "Check delay").SetValue(new Slider(200, 0, 500)));

            Menu.AddToMainMenu();

            Events.OnLoad += OnLoad;
            Events.OnClose += OnClose;
        }

        private static void OnClose(object sender, EventArgs e)
        {
            Game.OnUpdate -= Game_OnUpdate;
        }

        private static void OnLoad(object sender, EventArgs e)
        {
            hero = ObjectManager.LocalHero;
            sleeper = new Sleeper();

            Game.OnUpdate += Game_OnUpdate;
        }

        #endregion
    }
}