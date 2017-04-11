namespace ItemManager.Core.Modules.HpMpAbuse
{
    using System;
    using System.Linq;

    using Ensage;
    using Ensage.Common.AbilityInfo;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;
    using Ensage.Common.Objects.UtilityObjects;

    using Helpers;

    using Items;

    using Menus;

    using Utils;

    using Attribute = Ensage.Attribute;

    internal class HpMpAbuse
    {
        private readonly AbilityUpdater abilityUpdater;

        private readonly Team enemyTeam;

        private readonly Fountain fountain;

        private readonly Hero hero;

        //  private bool heroAttacking;

        private readonly Team heroTeam;

        private readonly ItemManager itemManager;

        //   private int manaLeft;

        public HpMpAbuse(Hero myHero, MenuManager menuManager)
        {
            hero = myHero;
            Menu = menuManager;

            enemyTeam = hero.GetEnemyTeam();
            heroTeam = hero.Team;
            fountain = new Fountain(heroTeam);
            itemManager = new ItemManager(menuManager);
            abilityUpdater = new AbilityUpdater();

            Game.OnUpdate += OnUpdate;
            Player.OnExecuteOrder += OnExecuteOrder;
        }

        public static MenuManager Menu { get; private set; }

        //public void OnDraw()
        //{
        //    if (!menu.ManaChecker.Enabled)
        //    {
        //        return;
        //    }

        //    var text = manaLeft >= 0 ? "Yes" : "No";

        //    if (menu.ManaChecker.ShowManaInfo)
        //    {
        //        text += " (" + manaLeft + ")";
        //    }

        //    Drawing.DrawText(
        //        text,
        //        "Arial",
        //        new Vector2(menu.ManaChecker.TextPositionX, menu.ManaChecker.TextPositionY),
        //        new Vector2(menu.ManaChecker.TextSize),
        //        manaLeft >= 0 ? Color.Yellow : Color.DarkOrange,
        //        FontFlags.None);
        //}

        public static MultiSleeper Sleeper { get; } = new MultiSleeper();

        public void OnClose()
        {
            Game.OnUpdate -= OnUpdate;
            Player.OnExecuteOrder -= OnExecuteOrder;

            itemManager.OnClose();
            abilityUpdater.OnClose();
        }

        public void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (hero.IsInvisible() || !args.Entities.Contains(hero) || !args.IsPlayerInput)
            {
                return;
            }

            switch (args.OrderId)
            {
                case OrderId.TransferItem:
                case OrderId.MoveItem:
                case OrderId.DropItem:
                case OrderId.PickItem:
                case OrderId.BuyItem:
                    Sleeper.Sleep(200 + Game.Ping, itemManager);
                    break;
                case OrderId.AttackTarget:
                case OrderId.AttackLocation:
                    //if (itemManager.PowerTreads.IsValid())
                    //{
                    //    var attribute = itemManager.PowerTreads.GetAttackAttribute();
                    //    if (attribute != itemManager.PowerTreads.ActiveAttribute)
                    //    {
                    //        itemManager.PowerTreads.SwitchTo(attribute);
                    //        heroAttacking = true;
                    //    }
                    //}
                    break;
                case OrderId.AbilityTarget:
                case OrderId.AbilityLocation:
                case OrderId.Ability:
                case OrderId.ToggleAbility:
                    if (!args.IsQueued)
                    {
                        if (args.Ability.IsAbilityBehavior(AbilityBehavior.Channeled))
                        {
                            itemManager.Bottle.SetSleep(args.Ability.ChannelTime() * 1000);
                        }

                        if (abilityUpdater.IgnoredAbilities.Contains(args.Ability.ClassId))
                        {
                            Sleeper.Sleep(1000, "Main");
                            return;
                        }

                        UseAbility(args);
                    }
                    break;
                case OrderId.MoveLocation:
                case OrderId.MoveTarget:
                    if (Menu.RecoveryMenu.IsActive)
                    {
                        itemManager.PickUpItemsOnMove(args);
                    }
                    //if (itemManager.PowerTreads.IsValid() && !itemManager.PowerTreads.DelaySwitch()
                    //    && !Sleeper.Sleeping("SwitchBackPT"))
                    //{
                    //    var attribute = itemManager.PowerTreads.GetMoveAttribute();
                    //    if (attribute != itemManager.PowerTreads.ActiveAttribute)
                    //    {
                    //        itemManager.PowerTreads.SwitchTo(attribute);
                    //        itemManager.PowerTreads.DefaultAttribute = attribute;
                    //        heroAttacking = false;
                    //    }
                    //}
                    break;
                default:
                    //      heroAttacking = false;
                    break;
            }
        }

        public void OnUpdate(EventArgs args)
        {
            if (Sleeper.Sleeping("Main"))
            {
                return;
            }

            Sleeper.Sleep(100, "Main");

            if (!hero.IsAlive || !hero.CanUseItems() || Game.IsPaused)
            {
                return;
            }

            //if (!Sleeper.Sleeping("ManaCheck") && menu.ManaChecker.Enabled)
            //{
            //    var heroMana = hero.Mana;

            //    var manaCost = hero.Spellbook.Spells.Concat(hero.Inventory.Items)
            //        .Where(x => menu.ManaChecker.AbilityEnabled(x.StoredName()))
            //        .Aggregate(0u, (current, ability) => current + ability.ManaCost);

            //    if (itemManager.PowerTreads.IsValid()
            //        && itemManager.PowerTreads.DefaultAttribute != Attribute.Intelligence
            //        && menu.ManaChecker.IncludePtCalcualtions)
            //    {
            //        heroMana += heroMana / hero.MaximumMana * 117;
            //    }

            //    manaLeft = (int)Math.Ceiling(heroMana - manaCost);
            //    Sleeper.Sleep(1000, "ManaCheck");
            //}

            var healing = false;

            if ((Menu.RecoveryMenu.IsActive || Menu.PowerTreadsSwitcherMenu.IsActive
                 && Menu.PowerTreadsSwitcherMenu.SwitchOnHeal) && hero.HasModifiers(ModifierUtils.Heal, false)
                && (hero.Health < hero.MaximumHealth || hero.Mana < hero.MaximumMana))
            {
                var attribute = itemManager.Bottle.GetPowerTreadsAttribute();
                if (attribute != Attribute.Invalid)
                {
                    itemManager.PowerTreads.SwitchTo(attribute);
                }
                healing = true;
            }

            if (Menu.RecoveryMenu.IsActive && itemManager.UsableItems.Any(x => x.CanBeCasted()))
            {
                if (IsEnemyNear())
                {
                    itemManager.PickUpItems();
                    return;
                }

                if (hero.NetworkActivity != NetworkActivity.Idle)
                {
                    hero.Stop();
                }

                if (itemManager.StashBottle.CanBeCasted() && !itemManager.StashBottleTaken)
                {
                    itemManager.TakeBottleFromStash();
                }

                foreach (var item in itemManager.UsableItems.Where(x => x.CanBeCasted()))
                {
                    itemManager.PowerTreads.SwitchTo(item.GetPowerTreadsAttribute());
                    itemManager.DropItems(item.GetDropItemStats(), item.Item);
                    item.Use();
                }

                Sleeper.Sleep(80 * itemManager.DroppedItemsCount() + Game.Ping, "Used", true);
            }
            else if (!healing && !itemManager.StashBottleTaken || !Menu.RecoveryMenu.IsActive)
            {
                if (Sleeper.Sleeping("Used"))
                {
                    return;
                }

                //if (!menu.TranquilBoots.DropActive)
                //{
                itemManager.PickUpItems();
                //}
            }

            //if (menu.TranquilBoots.DropActive)
            //{
            //    if (IsEnemyNear())
            //    {
            //        itemManager.PickUpItems();
            //        return;
            //    }
            //    if (itemManager.TranquilBoots.IsValid())
            //    {
            //        itemManager.DropItem(itemManager.TranquilBoots.Item, false, true);
            //        Sleeper.Sleep(200 + Game.Ping, "Main");
            //    }
            //    return;
            //}

            if (!Menu.RecoveryMenu.IsActive && fountain.BottleCanBeRefilled() && itemManager.Bottle.CanBeAutoCasted())
            {
                var useOnAllies = Menu.RecoveryMenu.AutoAllyBottle;
                var useOnSelf = Menu.RecoveryMenu.AutoSelfBottle;

                var bottleTarget = ObjectManager.GetEntitiesParallel<Hero>()
                    .Where(
                        x => !x.IsIllusion && x.Distance2D(hero) <= itemManager.Bottle.CastRange && x.IsAlive
                             && x.Team == hero.Team && !x.IsInvul())
                    .OrderBy(x => x.FindModifier(ModifierUtils.BottleRegeneration)?.RemainingTime)
                    .FirstOrDefault(
                        x => (useOnAllies && !x.Equals(hero) || useOnSelf && x.Equals(hero))
                             && (x.Health < x.MaximumHealth || x.Mana < x.MaximumMana));

                if (bottleTarget != null)
                {
                    itemManager.Bottle.Use(bottleTarget.Equals(hero) ? null : bottleTarget);
                }
            }

            //if (menu.TranquilBoots.CombineEnabled && !menu.RecoveryMenu.IsActive)
            //{
            //    if (itemManager.TranquilBoots.IsValid() && itemManager.TranquilBoots.AssembleTime(10))
            //    {
            //        menu.TranquilBoots.DisableCombine();
            //        return;
            //    }

            //    if (menu.TranquilBoots.CombineActive)
            //    {
            //        if (itemManager.TranquilBoots.IsValid())
            //        {
            //            if (itemManager.TranquilBoots.AssembleTime(8))
            //            {
            //                itemManager.TranquilBoots.Disassemble();
            //                Sleeper.Sleep(200 + Game.Ping, "Main");
            //            }
            //        }
            //        else
            //        {
            //            if (ObjectManager.GetEntitiesParallel<PhysicalItem>()
            //                .Any(
            //                    x => x.IsValid && x.Distance2D(hero) < 400
            //                         && itemManager.TranquilBoots.RequiredItems.Contains(x.Item.Id)))
            //            {
            //                itemManager.PickUpItems(itemManager.TranquilBoots.RequiredItems);
            //                Sleeper.Sleep(200 + Game.Ping, "Main");
            //                return;
            //            }

            //            foreach (var item in hero.Inventory.Items.Concat(hero.Inventory.Backpack)
            //                .Where(x => x.IsCombineLocked && itemManager.TranquilBoots.RequiredItems.Contains(x.Id)))
            //            {
            //                item.UnlockCombining();
            //            }

            //            Sleeper.Sleep(300 + Game.Ping, "Main");
            //        }
            //    }
            //    else
            //    {
            //        if (itemManager.TranquilBoots.IsValid())
            //        {
            //            itemManager.TranquilBoots.Disassemble();
            //            Sleeper.Sleep(200 + Game.Ping, "Main");
            //            return;
            //        }

            //        if (ObjectManager.GetEntitiesParallel<PhysicalItem>()
            //            .Any(
            //                x => x.IsValid && x.Distance2D(hero) < 400
            //                     && itemManager.TranquilBoots.RequiredItems.Contains(x.Item.Id)))
            //        {
            //            itemManager.PickUpItems(itemManager.TranquilBoots.RequiredItems);
            //            Sleeper.Sleep(200 + Game.Ping, "Main");
            //            return;
            //        }
            //    }
            //}

            if (!Menu.PowerTreadsSwitcherMenu.IsActive || !itemManager.PowerTreads.IsValid())
            {
                return;
            }

            if (!Sleeper.Sleeping("SwitchBackPT") && !itemManager.PowerTreads.DelaySwitch()
                && itemManager.PowerTreads.ActiveAttribute != itemManager.PowerTreads.DefaultAttribute && !healing
                && !hero.IsChanneling())
            {
                //if (heroAttacking)
                //{
                //    var attribute = menu.PowerTreads.Attributes.ElementAt(menu.PowerTreads.SwitchOnAttack).Value;
                //    if (itemManager.PowerTreads.ActiveAttribute != attribute)
                //    {
                //        itemManager.PowerTreads.SwitchTo(attribute);
                //    }
                //}
                //else
                //{
                itemManager.PowerTreads.SwitchTo(itemManager.PowerTreads.DefaultAttribute);
                //}
            }
        }

        private bool IsEnemyNear()
        {
            return ObjectManager.GetEntitiesParallel<Hero>()
                .Any(
                    x => x.IsAlive && x.Team == enemyTeam
                         && x.Distance2D(hero) <= 800 /*menu.Recovery.ForcePickEnemyDistance*/);
        }

        private void UseAbility(ExecuteOrderEventArgs args)
        {
            var ability = args.Ability;

            if (itemManager.PowerTreads.Equals(ability))
            {
                switch (itemManager.PowerTreads.ActiveAttribute)
                {
                    case Attribute.Strength:
                        itemManager.PowerTreads.DefaultAttribute = Attribute.Intelligence;
                        break;
                    case Attribute.Agility:
                        itemManager.PowerTreads.DefaultAttribute = Attribute.Strength;
                        break;
                    case Attribute.Intelligence:
                        itemManager.PowerTreads.DefaultAttribute = Attribute.Agility;
                        break;
                }
                Sleeper.Sleep(100 + Game.Ping, "SwitchBackPT");
                return;
            }

            if (ability.ManaCost <= 25 /*menu.PowerTreads.ManaThreshold*/)
            {
                return;
            }

            var soulRingCondition = itemManager.SoulRing.CanBeCasted();
            //    && menu.SoulRingMenu.AbilityEnabled(ability.StoredName());

            var powerTreadsCondition = itemManager.PowerTreads.CanBeCasted();
            //     && menu.PowerTreads.AbilityEnabled(ability.StoredName());

            if (!soulRingCondition && !powerTreadsCondition)
            {
                return;
            }

            args.Process = false;

            if (soulRingCondition)
            {
                itemManager.SoulRing.Use(false);
            }

            var sleep = Math.Max(ability.FindCastPoint() * 1000, 100) + Game.Ping / 1000 + 300;

            switch (args.OrderId)
            {
                case OrderId.AbilityTarget:
                    var target = args.Target as Unit;
                    if (target != null && target.IsValid && target.IsAlive)
                    {
                        if (powerTreadsCondition && hero.Distance2D(target) <= ability.GetCastRange() + 300)
                        {
                            itemManager.PowerTreads.SwitchTo(Attribute.Intelligence);
                            sleep += hero.GetTurnTime(target) * 1000;
                        }
                        ability.UseAbility(target);
                    }
                    break;
                case OrderId.AbilityLocation:
                    var targetLocation = args.TargetPosition;
                    if (powerTreadsCondition && (hero.Distance2D(targetLocation) <= ability.GetCastRange() + 300
                                                 || AbilityDatabase.Find(ability.StoredName()).FakeCastRange))
                    {
                        itemManager.PowerTreads.SwitchTo(Attribute.Intelligence);
                        sleep += hero.GetTurnTime(targetLocation) * 1000;
                    }
                    ability.UseAbility(targetLocation);
                    break;
                case OrderId.Ability:
                    if (powerTreadsCondition)
                    {
                        itemManager.PowerTreads.SwitchTo(Attribute.Intelligence);
                    }
                    ability.UseAbility();
                    break;
                case OrderId.ToggleAbility:
                    if (powerTreadsCondition && !itemManager.PowerTreads.DelaySwitch())
                    {
                        itemManager.PowerTreads.SwitchTo(Attribute.Intelligence);
                    }
                    ability.ToggleAbility();
                    break;
            }

            Sleeper.Sleep((float)sleep, "SwitchBackPT", true);
        }
    }
}