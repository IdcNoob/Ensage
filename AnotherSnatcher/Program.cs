using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;

namespace AnotherSnatcher {
    internal class Program {
        private static Hero hero;
        private static bool inGame;

        private static readonly Menu Menu = new Menu("Another Snatcher", "anotherSnatcher", true);

        private static void Main() {
            var itemsToggle = new Dictionary<string, bool> {
                {"item_gem", true},
                {"item_cheese", true},
                {"item_rapier", true},
                {"item_aegis", true},
                {"rune_doubledamage", true}
            };
            var itemsHold = itemsToggle.ToDictionary(x => x.Key, x => x.Value);

            Menu.AddItem(new MenuItem("holdSnatchKey", "Hold key").SetValue(new KeyBind('O', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("enabledStealHold", "Hold steal:").SetValue(new AbilityToggler(itemsToggle)));
            Menu.AddItem(new MenuItem("pressSnatchKey", "Toggle key").SetValue(new KeyBind('P', KeyBindType.Toggle)));
            Menu.AddItem(new MenuItem("enabledStealToggle", "Toggle steal:").SetValue(new AbilityToggler(itemsHold)));
            Menu.AddItem(new MenuItem("sleep", "Check delay").SetValue(new Slider(200, 50, 500)));

            Menu.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args) {
            if (!Utils.SleepCheck("anotherSnatcher"))
                return;

            if (!inGame) {
                hero = ObjectMgr.LocalHero;

                if (!Game.IsInGame || hero == null) {
                    Utils.Sleep(1000, "anotherSnatcher");
                    return;
                }

                inGame = true;
            }

            if (!Game.IsInGame) {
                inGame = false;
                return;
            }

            var holdKey = Menu.Item("holdSnatchKey").GetValue<KeyBind>().Active;
            var toggleKey = Menu.Item("pressSnatchKey").GetValue<KeyBind>().Active;

            if (!hero.IsAlive || Game.IsPaused || (!holdKey && !toggleKey)) {
                Utils.Sleep(Menu.Item("sleep").GetValue<Slider>().Value, "anotherSnatcher");
                return;
            }

            if ((toggleKey && Menu.Item("enabledStealToggle").GetValue<AbilityToggler>().IsEnabled("rune_doubledamage")) || 
                (holdKey && Menu.Item("enabledStealHold").GetValue<AbilityToggler>().IsEnabled("rune_doubledamage"))) {
                var rune = ObjectMgr.GetEntities<Rune>().FirstOrDefault(x => x.IsVisible && x.Distance2D(hero) < 400);
                if (rune != null) {
                    hero.PickUpRune(rune);
                    Utils.Sleep(500, "anotherSnatcher");
                    return;
                }
            }

            if (hero.Inventory.FreeSlots.Any()) {
                var aegis = (toggleKey && Menu.Item("enabledStealToggle").GetValue<AbilityToggler>().IsEnabled("item_aegis")) ||
                            (holdKey && Menu.Item("enabledStealHold").GetValue<AbilityToggler>().IsEnabled("item_aegis"));

                var cheese = (toggleKey && Menu.Item("enabledStealToggle").GetValue<AbilityToggler>().IsEnabled("item_cheese")) ||
                             (holdKey && Menu.Item("enabledStealHold").GetValue<AbilityToggler>().IsEnabled("item_cheese"));

                var rapier = (toggleKey && Menu.Item("enabledStealToggle").GetValue<AbilityToggler>().IsEnabled("item_rapier")) ||
                             (holdKey && Menu.Item("enabledStealHold").GetValue<AbilityToggler>().IsEnabled("item_rapier"));

                var gem = (toggleKey && Menu.Item("enabledStealToggle").GetValue<AbilityToggler>().IsEnabled("item_gem")) ||
                          (holdKey && Menu.Item("enabledStealHold").GetValue<AbilityToggler>().IsEnabled("item_gem"));

                var item =
                    ObjectMgr.GetEntities<PhysicalItem>()
                        .FirstOrDefault(
                            x =>
                                x.IsVisible && x.Distance2D(hero) < 400 &&
                                ((aegis && x.Item.Name == "item_aegis") || (rapier && x.Item.Name == "item_rapier") ||
                                 (cheese && x.Item.Name == "item_cheese") || (gem && x.Item.Name == "item_gem")));

                if (item != null) {
                    hero.PickUpItem(item);
                    Utils.Sleep(500, "anotherSnatcher");
                    return;
                }
            }

            Utils.Sleep(Menu.Item("sleep").GetValue<Slider>().Value, "anotherSnatcher");
        }
    }
}