namespace HpMpAbuse
{
    using System;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;
    using Ensage.Common.Objects.UtilityObjects;

    using HpMpAbuse.Helpers;
    using HpMpAbuse.Items;
    using HpMpAbuse.Menu;

    using SharpDX;

    using Attribute = Ensage.Attribute;

    internal class Abuse
    {
        #region Fields

        private AbilityUpdater abilityUpdater;

        private Team enemyTeam;

        private Fountain fountain;

        private bool heroAttacking;

        private Team heroTeam;

        private ItemManager itemManager;

        private int manaLeft;

        #endregion

        #region Properties

        private static Hero Hero
        {
            get
            {
                return Variables.Hero;
            }
            set
            {
                Variables.Hero = value;
            }
        }

        private static MenuManager Menu
        {
            get
            {
                return Variables.Menu;
            }
            set
            {
                Variables.Menu = value;
            }
        }

        private static MultiSleeper Sleeper
        {
            get
            {
                return Variables.Sleeper;
            }
            set
            {
                Variables.Sleeper = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            itemManager.OnClose();
            abilityUpdater.OnClose();
            Menu.OnClose();
        }

        public void OnDraw()
        {
            if (!Menu.ManaChecker.Enabled)
            {
                return;
            }

            var text = manaLeft >= 0 ? "Yes" : "No";

            if (Menu.ManaChecker.ShowManaInfo)
            {
                text += " (" + manaLeft + ")";
            }

            Drawing.DrawText(
                text,
                "Arial",
                new Vector2(Menu.ManaChecker.TextPositionX, Menu.ManaChecker.TextPositionY),
                new Vector2(Menu.ManaChecker.TextSize),
                manaLeft >= 0 ? Color.Yellow : Color.DarkOrange,
                FontFlags.None);
        }

        public void OnExecuteAction(Player sender, ExecuteOrderEventArgs args)
        {
            if (Hero.IsInvisible())
            {
                return;
            }
            switch (args.Order)
            {
                case Order.MoveItem:
                case Order.DropItem:
                case Order.PickItem:
                case Order.BuyItem:
                    Sleeper.Sleep(200 + Game.Ping, itemManager);
                    break;
                case Order.AttackTarget:
                case Order.AttackLocation:
                    if (itemManager.PowerTreads.IsValid())
                    {
                        var attribute = itemManager.PowerTreads.GetAttackAttribute();
                        if (attribute != itemManager.PowerTreads.ActiveAttribute)
                        {
                            itemManager.PowerTreads.SwitchTo(attribute);
                            heroAttacking = true;
                        }
                    }
                    break;
                case Order.AbilityTarget:
                case Order.AbilityLocation:
                case Order.Ability:
                case Order.ToggleAbility:
                    if (!args.IsQueued)
                    {
                        UseAbility(args);
                    }
                    break;
                case Order.MoveLocation:
                case Order.MoveTarget:
                    if (Menu.Recovery.Active)
                    {
                        itemManager.PickUpItemsOnMove(args);
                    }
                    if (itemManager.PowerTreads.IsValid() && !itemManager.PowerTreads.DelaySwitch()
                        && !Sleeper.Sleeping("SwitchBackPT"))
                    {
                        var attribute = itemManager.PowerTreads.GetMoveAttribute();
                        if (attribute != itemManager.PowerTreads.ActiveAttribute)
                        {
                            itemManager.PowerTreads.SwitchTo(attribute);
                            itemManager.PowerTreads.DefaultAttribute = attribute;
                            heroAttacking = false;
                        }
                    }
                    break;
                default:
                    heroAttacking = false;
                    break;
            }
        }

        public void OnLoad()
        {
            Hero = ObjectManager.LocalHero;
            enemyTeam = Hero.GetEnemyTeam();
            heroTeam = Hero.Team;
            Menu = new MenuManager();
            Sleeper = new MultiSleeper();
            fountain = new Fountain(heroTeam);
            itemManager = new ItemManager();
            abilityUpdater = new AbilityUpdater();
        }

        public void OnUpdate()
        {
            if (Sleeper.Sleeping("Main"))
            {
                return;
            }

            Sleeper.Sleep(100, "Main");

            if (!Hero.IsAlive || !Hero.CanUseItems() || Game.IsPaused)
            {
                return;
            }

            if (!Sleeper.Sleeping("ManaCheck") && Menu.ManaChecker.Enabled)
            {
                var heroMana = Hero.Mana;

                var manaCost =
                    Hero.Spellbook.Spells.Concat(Hero.Inventory.Items)
                        .Where(x => Menu.ManaChecker.AbilityEnabled(x.StoredName()))
                        .Aggregate(0u, (current, ability) => current + ability.ManaCost);

                if (itemManager.PowerTreads.IsValid()
                    && itemManager.PowerTreads.DefaultAttribute != Attribute.Intelligence
                    && Menu.ManaChecker.IncludePtCalcualtions)
                {
                    heroMana += heroMana / Hero.MaximumMana * 117;
                }

                manaLeft = (int)Math.Ceiling(heroMana - manaCost);
                Sleeper.Sleep(1000, "ManaCheck");
            }

            var healing = false;

            if ((Menu.Recovery.Active || (Menu.PowerTreads.Enabled && Menu.PowerTreads.SwitchOnHeal))
                && Hero.HasModifiers(Modifiers.Heal, false)
                && (Hero.Health < Hero.MaximumHealth || Hero.Mana < Hero.MaximumMana))
            {
                var attribute = itemManager.Bottle.GetPowerTreadsAttribute();
                if (attribute != Attribute.Invalid)
                {
                    itemManager.PowerTreads.SwitchTo(attribute);
                }
                healing = true;
            }

            if (Menu.Recovery.Active && itemManager.UsableItems.Any(x => x.CanBeCasted()))
            {
                if (EnemyNear())
                {
                    itemManager.PickUpItems();
                    return;
                }

                if (Hero.NetworkActivity != NetworkActivity.Idle)
                {
                    Hero.Stop();
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

                Sleeper.Sleep(80 * itemManager.DroppedItemsCount() + Game.Ping, "Used");
            }
            else if (!healing || !Menu.Recovery.Active)
            {
                if (Sleeper.Sleeping("Used"))
                {
                    return;
                }
                if (!Menu.TranquilBoots.DropActive)
                {
                    itemManager.PickUpItems();
                }
            }

            if (Menu.TranquilBoots.DropActive)
            {
                if (EnemyNear())
                {
                    itemManager.PickUpItems();
                    return;
                }
                if (itemManager.TranquilBoots.IsValid())
                {
                    itemManager.DropItem(itemManager.TranquilBoots.Item, false);
                    Sleeper.Sleep(200 + Game.Ping, "Main");
                }
                return;
            }

            if (!Menu.Recovery.Active && fountain.BottleCanBeRefilled() && itemManager.Bottle.CanBeAutoCasted())
            {
                var ally =
                    Heroes.GetByTeam(heroTeam)
                        .OrderBy(x => x.FindModifier(Modifiers.BottleRegeneration)?.RemainingTime)
                        .FirstOrDefault(
                            x =>
                            x.IsAlive && !x.IsInvul() && !x.IsIllusion
                            && (x.Health < x.MaximumHealth || x.Mana < x.MaximumMana)
                            && x.Distance2D(Hero) <= itemManager.Bottle.CastRange);

                if (ally != null)
                {
                    itemManager.Bottle.UseOn(ally);
                }
            }

            if (Menu.TranquilBoots.CombineEnabled && !Menu.Recovery.Active)
            {
                if (itemManager.TranquilBoots.IsValid() && itemManager.TranquilBoots.AssembleTime(10))
                {
                    Menu.TranquilBoots.DisableCombine();
                    return;
                }

                var regen = Hero.FindItem("item_ring_of_regen");

                if (Menu.TranquilBoots.CombineActive)
                {
                    if (itemManager.TranquilBoots.IsValid())
                    {
                        if (itemManager.TranquilBoots.AssembleTime(8))
                        {
                            itemManager.TranquilBoots.Disassemble();
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
                        itemManager.PickUpItems("item_boots", "item_ring_of_protection", "item_ring_of_regen");
                        Sleeper.Sleep(300 + Game.Ping, "Main");
                    }
                }
                else
                {
                    if (itemManager.TranquilBoots.IsValid())
                    {
                        itemManager.TranquilBoots.Disassemble();
                        Sleeper.Sleep(200 + Game.Ping, "Main");
                        return;
                    }
                    if (regen != null && !regen.IsCombineLocked)
                    {
                        regen.LockCombining();
                    }
                    else if (regen == null)
                    {
                        itemManager.PickUpItems("item_ring_of_regen");
                        Sleeper.Sleep(200 + Game.Ping, "Main");
                    }
                    else
                    {
                        if (Hero.FindItem("item_boots") == null || Hero.FindItem("item_ring_of_protection") == null)
                        {
                            itemManager.PickUpItems("item_boots", "item_ring_of_protection");
                            Sleeper.Sleep(200 + Game.Ping, "Main");
                        }
                    }
                }
            }

            if (!Menu.PowerTreads.Enabled || !itemManager.PowerTreads.IsValid())
            {
                return;
            }

            if (!Sleeper.Sleeping("SwitchBackPT") && !itemManager.PowerTreads.DelaySwitch()
                && itemManager.PowerTreads.ActiveAttribute != itemManager.PowerTreads.DefaultAttribute && !healing)
            {
                if (heroAttacking)
                {
                    var attribute = Menu.PowerTreads.Attributes.ElementAt(Menu.PowerTreads.SwitchOnAttack).Value;
                    if (itemManager.PowerTreads.ActiveAttribute != attribute)
                    {
                        itemManager.PowerTreads.SwitchTo(attribute);
                    }
                }
                else
                {
                    itemManager.PowerTreads.SwitchTo(itemManager.PowerTreads.DefaultAttribute);
                }
            }
        }

        #endregion

        #region Methods

        private bool EnemyNear()
        {
            return
                Heroes.GetByTeam(enemyTeam)
                    .Any(x => x.IsAlive && x.Distance2D(Hero) <= Menu.Recovery.ForcePickEnemyDistance);
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

            if (ability.ManaCost <= Menu.PowerTreads.ManaThreshold
                || abilityUpdater.IgnoredAbilities.Contains(ability.StoredName()))
            {
                return;
            }

            var soulRingCondition = itemManager.SoulRing.CanBeCasted()
                                    && Menu.SoulRing.AbilityEnabled(ability.StoredName());

            var powerTreadsCondition = itemManager.PowerTreads.CanBeCasted()
                                       && Menu.PowerTreads.AbilityEnabled(ability.StoredName());

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

            switch (args.Order)
            {
                case Order.AbilityTarget:
                    var target = args.Target as Unit;
                    if (target != null && target.IsValid && target.IsAlive)
                    {
                        if (powerTreadsCondition && Hero.Distance2D(target) <= ability.GetCastRange() + 300)
                        {
                            itemManager.PowerTreads.SwitchTo(Attribute.Intelligence);
                            sleep += Hero.GetTurnTime(target) * 1000;
                        }
                        ability.UseAbility(target);
                    }
                    break;
                case Order.AbilityLocation:
                    var targetLocation = args.TargetPosition;
                    if (powerTreadsCondition && Hero.Distance2D(targetLocation) <= ability.GetCastRange() + 300)
                    {
                        itemManager.PowerTreads.SwitchTo(Attribute.Intelligence);
                        sleep += Hero.GetTurnTime(targetLocation) * 1000;
                    }
                    ability.UseAbility(targetLocation);
                    break;
                case Order.Ability:
                    if (powerTreadsCondition)
                    {
                        itemManager.PowerTreads.SwitchTo(Attribute.Intelligence);
                    }
                    ability.UseAbility();
                    break;
                case Order.ToggleAbility:
                    if (powerTreadsCondition && !itemManager.PowerTreads.DelaySwitch())
                    {
                        itemManager.PowerTreads.SwitchTo(Attribute.Intelligence);
                    }
                    ability.ToggleAbility();
                    break;
            }

            Sleeper.Sleep((float)sleep, "SwitchBackPT", true);
        }

        #endregion
    }
}