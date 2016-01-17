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
            var items = new Dictionary<string, bool> {
                {"item_gem", true},
                {"item_cheese", true},
                {"item_rapier", true},
                {"item_aegis", true},
                {"rune_doubledamage", true}
            };

            Menu.AddItem(new MenuItem("holdSnatchKey", "Hold Key").SetValue(new KeyBind('O', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("pressSnatchKey", "Press Key").SetValue(new KeyBind('P', KeyBindType.Toggle)));
            Menu.AddItem(new MenuItem("enabledSteal", "Steal:").SetValue(new AbilityToggler(items)));
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

            if (!hero.IsAlive || Game.IsPaused ||
                (!Menu.Item("pressSnatchKey").GetValue<KeyBind>().Active &&
                 !Menu.Item("holdSnatchKey").GetValue<KeyBind>().Active)) {
                Utils.Sleep(Menu.Item("sleep").GetValue<Slider>().Value, "anotherSnatcher");
                return;
            }

            if (Menu.Item("enabledSteal").GetValue<AbilityToggler>().IsEnabled("item_aegis")) {
                var rune = ObjectMgr.GetEntities<Rune>().FirstOrDefault(x => x.IsVisible && x.Distance2D(hero) < 400);
                if (rune != null) {
                    hero.PickUpRune(rune);
                }
            }

            if (hero.Inventory.FreeSlots.Any()) {
                var aegis = Menu.Item("enabledSteal").GetValue<AbilityToggler>().IsEnabled("rune_doubledamage");
                var cheese = Menu.Item("enabledSteal").GetValue<AbilityToggler>().IsEnabled("item_cheese");
                var rapier = Menu.Item("enabledSteal").GetValue<AbilityToggler>().IsEnabled("item_rapier");
                var gem = Menu.Item("enabledSteal").GetValue<AbilityToggler>().IsEnabled("item_gem");

                var items =
                    ObjectMgr.GetEntities<PhysicalItem>()
                        .FirstOrDefault(
                            x =>
                                x.IsVisible && x.Distance2D(hero) < 400 &&
                                ((aegis && x.Item.Name == "item_aegis") || (rapier && x.Item.Name == "item_rapier") ||
                                 (cheese && x.Item.Name == "item_cheese") || (gem && x.Item.Name == "item_gem")));

                if (items != null)
                    hero.PickUpItem(items);
            }

            Utils.Sleep(Menu.Item("sleep").GetValue<Slider>().Value, "anotherSnatcher");
        }
    }
}