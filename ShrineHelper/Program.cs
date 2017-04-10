namespace ShrineHelper
{
    using System;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;

    internal class Program
    {
        #region Static Fields

        private static MenuItem blockShrineHp;

        private static MenuItem blockShrineMp;

        private static Hero hero;

        private static Menu menu;

        #endregion

        #region Methods

        private static void ActivateShrine(object sender, OnValueChangeEventArgs onValueChangeEventArgs)
        {
            if (!onValueChangeEventArgs.GetNewValue<KeyBind>().Active)
            {
                return;
            }

            var shrine =
                ObjectManager.GetEntities<Unit>()
                    .Where(
                        x =>
                            x.IsValid && x.ClassID == ClassID.CDOTA_BaseNPC_Healer && x.Distance2D(hero) <= 555
                            && x.IsAlive && x.Team == hero.Team
                            && x.Spellbook.Spells.FirstOrDefault(z => z.Name == "filler_ability")?.AbilityState
                            == AbilityState.Ready)
                    .OrderBy(x => x.Distance2D(hero))
                    .FirstOrDefault();

            if (shrine != null)
            {
                hero.Follow(shrine);
            }
        }

        private static void EventsOnClose(object sender, EventArgs eventArgs)
        {
            Events.OnClose -= EventsOnClose;
            Player.OnExecuteOrder -= PlayerOnExecuteOrder;

            menu.RemoveFromMainMenu();
        }

        private static void EventsOnLoad(object sender, EventArgs eventArgs)
        {
            hero = ObjectManager.LocalHero;

            menu = new Menu("Shrine Helper", "shrineHelper", true, "filler_ability", true);

            menu.AddItem(blockShrineHp = new MenuItem("blockShrineHp", "Block shrine HP% activation"))
                .SetValue(new Slider(70))
                .SetTooltip("Block shrine activation when your hero have more HP%");
            menu.AddItem(blockShrineMp = new MenuItem("blockShrineMp", "Block shrine MP% activation"))
                .SetValue(new Slider(70))
                .SetTooltip("Block shrine activation when your hero have more MP%");
            menu.AddItem(new MenuItem("activateShrine", "Force activate shrine"))
                .SetValue(new KeyBind('M', KeyBindType.Press))
                .ValueChanged += ActivateShrine;

            menu.AddToMainMenu();

            Events.OnClose += EventsOnClose;
            Player.OnExecuteOrder += PlayerOnExecuteOrder;
        }

        private static void Main()
        {
            Events.OnLoad += EventsOnLoad;
            
            int.TryParse("1337", out var yolo);
        }

        private static void PlayerOnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!args.Entities.Contains(hero))
            {
                return;
            }

            if (args.Order == Order.MoveTarget && args.Target?.ClassID == ClassID.CDOTA_BaseNPC_Healer
                && (float)hero.Health / hero.MaximumHealth * 100 > blockShrineHp.GetValue<Slider>().Value
                && hero.Mana / hero.MaximumMana * 100 > blockShrineMp.GetValue<Slider>().Value)
            {
                args.Process = false;
                hero.Move(args.Target.Position);
            }
        }

        #endregion
    }
}
