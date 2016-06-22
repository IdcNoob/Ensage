namespace TranquilAbuse
{
    using System;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using Ensage.Common.Objects;

    internal class Program
    {
        #region Static Fields

        private static readonly Menu Menu = new Menu("Abuse", "tranquilAbuse", true, "item_tranquil_boots", true);

        private static Hero hero;

        #endregion

        #region Methods

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!Utils.SleepCheck("Tranquil.Sleep"))
            {
                return;
            }

            if (Game.IsPaused || !hero.IsAlive || !Menu.Item("enabled").IsActive())
            {
                Utils.Sleep(1000, "Tranquil.Sleep");
                return;
            }

            Utils.Sleep(333, "Tranquil.Sleep");

            var tranquil = hero.FindItem("item_tranquil_boots");

            if (tranquil != null && tranquil.AssembledTime + 10 < Game.RawGameTime)
            {
                return;
            }

            var regen = hero.FindItem("item_ring_of_regen");

            if (Menu.Item("abuseKey").GetValue<KeyBind>().Active)
            {
                if (tranquil != null)
                {
                    if (tranquil.AssembledTime + 8 < Game.RawGameTime)
                    {
                        tranquil.DisassembleItem();
                    }
                }
                else if (regen != null)
                {
                    if (regen.IsCombineLocked)
                    {
                        regen.UnlockCombining();
                    }
                }
                else
                {
                    PickUpItems();
                }
            }
            else
            {
                if (tranquil != null)
                {
                    tranquil.DisassembleItem();
                    return;
                }

                if (regen != null && !regen.IsCombineLocked)
                {
                    regen.LockCombining();
                }
                else if (regen == null)
                {
                    var droppedRegen =
                        ObjectManager.GetEntities<PhysicalItem>()
                            .FirstOrDefault(
                                x => x.Distance2D(hero) < 500 && x.Item.StoredName() == "item_ring_of_regen");

                    if (droppedRegen != null)
                    {
                        hero.PickUpItem(droppedRegen);
                    }
                }
                else
                {
                    PickUpItems();
                }
            }
        }

        private static void Main()
        {
            Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
            Menu.AddItem(new MenuItem("abuseKey", "Key").SetValue(new KeyBind('D', KeyBindType.Toggle)));

            Menu.AddToMainMenu();

            Events.OnLoad += OnLoad;
        }

        private static void OnClose(object sender, EventArgs e)
        {
            Events.OnClose -= OnClose;
            Game.OnIngameUpdate -= Game_OnUpdate;
        }

        private static void OnLoad(object sender, EventArgs e)
        {
            hero = ObjectManager.LocalHero;

            Events.OnClose += OnClose;
            Game.OnIngameUpdate += Game_OnUpdate;
        }

        private static void PickUpItems()
        {
            var droppedItems =
                ObjectManager.GetEntities<PhysicalItem>()
                    .Where(
                        x =>
                        x.Distance2D(hero) < 500
                        && (x.Item.StoredName() == "item_boots" || x.Item.StoredName() == "item_ring_of_regen"
                            || x.Item.StoredName() == "item_ring_of_protection"));

            foreach (var item in droppedItems)
            {
                hero.PickUpItem(item);
            }
        }

        #endregion
    }
}