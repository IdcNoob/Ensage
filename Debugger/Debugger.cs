namespace Debugger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    using Menus;

    using SharpDX;

    using Color = System.ConsoleColor;

    internal class Debugger
    {
        private Logger logger;

        private MenuManager mainMenu;

        private Player player;

        private Entity SelectedEntity => player.QueryUnit ?? player.Selection.FirstOrDefault();

        public void OnClose()
        {
            Unit.OnModifierAdded -= OnModifierAdded;
            Unit.OnModifierRemoved -= OnModifierRemoved;
            Entity.OnParticleEffectAdded -= OnParticleEffectAdded;
            ObjectManager.OnAddEntity -= OnEntityAdded;
            ObjectManager.OnRemoveEntity -= OnEntityRemoved;
            ObjectManager.OnAddTrackingProjectile -= OnTrackingProjectileAdded;
            ObjectManager.OnRemoveTrackingProjectile -= OnTrackingProjectileRemoved;

            Entity.OnAnimationChanged -= OnAnimationChanged;
            Entity.OnBoolPropertyChange -= OnBoolPropertyChanged;
            Entity.OnFloatPropertyChange -= OnFloatPropertyChanged;
            Entity.OnHandlePropertyChange -= OnHandlePropertyChanged;
            Entity.OnInt32PropertyChange -= OnInt32PropertyChanged;
            Entity.OnInt64PropertyChange -= OnInt64PropertyChanged;
            Entity.OnStringPropertyChange -= OnStringPropertyChanged;

            Player.OnExecuteOrder -= OnExecuteOrder;

            Game.OnFireEvent -= OnFireEvent;
            Game.OnGCMessageReceive -= OnGcMessageReceive;
            Game.OnMessage -= OnMessage;
            Game.OnUIStateChanged -= OnUiStateChanged;

            Drawing.OnDraw -= OnDraw;
            Game.OnWndProc -= GameOnOnWndProc;

            mainMenu.DumpMenu.Spells.OnDump -= SpellsOnDump;
            mainMenu.DumpMenu.Items.OnDump -= ItemsOnDump;
            mainMenu.DumpMenu.Modifiers.OnDump -= ModifiersOnDump;
            mainMenu.DumpMenu.Units.OnDump -= UnitsOnDump;

            mainMenu.OnClose();
        }

        public void OnLoad()
        {
            mainMenu = new MenuManager();
            logger = new Logger();
            player = ObjectManager.LocalPlayer;

            Unit.OnModifierAdded += OnModifierAdded;
            Unit.OnModifierRemoved += OnModifierRemoved;
            Entity.OnParticleEffectAdded += OnParticleEffectAdded;
            ObjectManager.OnAddEntity += OnEntityAdded;
            ObjectManager.OnRemoveEntity += OnEntityRemoved;
            ObjectManager.OnAddTrackingProjectile += OnTrackingProjectileAdded;
            ObjectManager.OnRemoveTrackingProjectile += OnTrackingProjectileRemoved;

            Entity.OnAnimationChanged += OnAnimationChanged;
            Entity.OnBoolPropertyChange += OnBoolPropertyChanged;
            Entity.OnFloatPropertyChange += OnFloatPropertyChanged;
            Entity.OnHandlePropertyChange += OnHandlePropertyChanged;
            Entity.OnInt32PropertyChange += OnInt32PropertyChanged;
            Entity.OnInt64PropertyChange += OnInt64PropertyChanged;
            Entity.OnStringPropertyChange += OnStringPropertyChanged;

            Player.OnExecuteOrder += OnExecuteOrder;

            Game.OnFireEvent += OnFireEvent;
            Game.OnGCMessageReceive += OnGcMessageReceive;
            Game.OnMessage += OnMessage;
            Game.OnUIStateChanged += OnUiStateChanged;

            Drawing.OnDraw += OnDraw;
            Game.OnWndProc += GameOnOnWndProc;

            mainMenu.DumpMenu.Spells.OnDump += SpellsOnDump;
            mainMenu.DumpMenu.Items.OnDump += ItemsOnDump;
            mainMenu.DumpMenu.Modifiers.OnDump += ModifiersOnDump;
            mainMenu.DumpMenu.Units.OnDump += UnitsOnDump;
        }

        private void GameOnOnWndProc(WndEventArgs args)
        {
            if (mainMenu.InfoMenu.ShowGameMousePosition && args.Msg == (uint)WindowsMessages.LBUTTONDOWN)
            {
                logger.Write(
                    "Mouse position: " + Math.Round(Game.MousePosition.X) + ", " + Math.Round(Game.MousePosition.Y)
                    + ", " + Math.Round(Game.MousePosition.Z),
                    Logger.Type.Information,
                    Color.White);
            }
        }

        private void ItemsOnDump(object sender, EventArgs e)
        {
            var unit = SelectedEntity as Unit;
            if (unit == null)
            {
                return;
            }

            var menu = mainMenu.DumpMenu.Items;

            const Color Color = Color.Yellow;
            const Logger.Type Type = Logger.Type.Item;

            logger.Write("Item dump", Type, Color, true);
            logger.Write("Unit name: " + unit.Name, Type, Color);
            logger.Write("Unit classID: " + unit.ClassId, Type, Color);
            logger.EmptyLine();

            if (!unit.HasInventory)
            {
                return;
            }

            var items = new List<Item>();

            if (menu.ShowInventoryItems)
            {
                items.AddRange(unit.Inventory.Items);
            }
            if (menu.ShowBackpackItems)
            {
                items.AddRange(unit.Inventory.Backpack);
            }
            if (menu.ShowStashItems)
            {
                items.AddRange(unit.Inventory.Stash);
            }

            foreach (var item in items)
            {
                logger.Write("Name: " + item.Name, Type, Color);
                logger.Write("ClassID: " + item.ClassId, Type, Color);

                if (menu.ShowManaCost)
                {
                    logger.Write("Mana cost: " + item.ManaCost, Type, Color);
                }
                if (menu.ShowCastRange)
                {
                    logger.Write("Cast range: " + item.GetCastRange(), Type, Color);
                }
                if (menu.ShowBehavior)
                {
                    logger.Write("Behavior: " + item.AbilityBehavior, Type, Color);
                }
                if (menu.ShowTargetType)
                {
                    logger.Write("Target type: " + item.TargetType, Type, Color);
                    logger.Write("Target team type: " + item.TargetTeamType, Type, Color);
                }
                if (menu.ShowSpecialData)
                {
                    logger.Write("Special data =>", Type, Color);
                    foreach (var abilitySpecialData in item.AbilitySpecialData)
                    {
                        logger.Write("  " + abilitySpecialData.Name + ": " + abilitySpecialData.Value, Type, Color);
                    }
                }

                logger.EmptyLine();
            }

            logger.EmptyLine();
        }

        private void ModifiersOnDump(object sender, EventArgs e)
        {
            var unit = SelectedEntity as Unit;

            if (unit == null)
            {
                return;
            }

            var menu = mainMenu.DumpMenu.Modifiers;

            const Color Color = Color.Yellow;
            const Logger.Type Type = Logger.Type.Modifier;

            logger.Write("Modifier dump", Type, Color, true);
            logger.Write("Unit name: " + unit.Name, Type, Color);
            logger.Write("Unit classID: " + unit.ClassId, Type, Color);
            logger.EmptyLine();

            foreach (var modifier in unit.Modifiers)
            {
                if (!menu.ShowHidden && modifier.IsHidden)
                {
                    continue;
                }

                logger.Write("Name: " + modifier.Name, Type, Color);

                if (menu.ShowTextureName)
                {
                    logger.Write("Texture name: " + modifier.TextureName, Type, Color);
                }
                if (menu.ShowElapsedTime)
                {
                    logger.Write("Elapsed time: " + modifier.ElapsedTime, Type, Color);
                }
                if (menu.ShowRemainingTime)
                {
                    logger.Write("Remaining time: " + modifier.RemainingTime, Type, Color);
                }

                logger.EmptyLine();
            }

            logger.EmptyLine();
        }

        private void OnAnimationChanged(Entity sender, EventArgs args)
        {
            var menu = mainMenu.OnChangeMenu.Animations;

            if (!menu.Enabled || menu.HeroesOnly && !(sender is Hero))
            {
                return;
            }

            const Color Color = Color.Cyan;
            const Logger.Type Type = Logger.Type.Animation;

            logger.Write("Animation changed", Type, Color, true);
            logger.Write("Sender name: " + sender.Name, Type, Color);
            logger.Write("Sender classID: " + sender.ClassId, Type, Color);
            if (sender.Owner != null)
            {
                logger.Write("Owner Name: " + sender.Owner.Name, Type, Color);
                logger.Write("Owner classID: " + sender.Owner.ClassId, Type, Color);
            }
            logger.Write("Name: " + sender.Animation.Name, Type, Color);
            logger.EmptyLine();
        }

        private void OnBoolPropertyChanged(Entity sender, BoolPropertyChangeEventArgs args)
        {
            if (args.OldValue.Equals(args.NewValue))
            {
                return;
            }

            var menu = mainMenu.OnChangeMenu.Bools;

            if (!menu.Enabled || menu.HeroesOnly && !(sender is Hero)
                || menu.IgnoreUseless && Data.IgnoredBools.Contains(args.PropertyName))
            {
                return;
            }
            
            const Color Color = Color.Cyan;
            const Logger.Type Type = Logger.Type.Bool;

            logger.Write("Bool property changed", Type, Color, true);
            logger.Write("Sender name: " + sender.Name, Type, Color);
            logger.Write("Sender classID: " + sender.ClassId, Type, Color);
            if (sender.Owner != null)
            {
                logger.Write("Owner Name: " + sender.Owner.Name, Type, Color);
                logger.Write("Owner classID: " + sender.Owner.ClassId, Type, Color);
            }
            logger.Write("Property name: " + args.PropertyName, Type, Color);
            logger.Write("Property values: " + args.OldValue + " => " + args.NewValue, Type, Color);
            logger.EmptyLine();
        }

        private void OnDraw(EventArgs args)
        {
            if (mainMenu.InfoMenu.ShowGameMousePosition)
            {
                Drawing.DrawText(
                    Math.Round(Game.MousePosition.X) + " " + Math.Round(Game.MousePosition.Y) + " "
                    + Math.Round(Game.MousePosition.Z),
                    "Arial",
                    Game.MouseScreenPosition + new Vector2(35, 0),
                    new Vector2(20),
                    SharpDX.Color.White,
                    FontFlags.None);
            }
        }

        private void OnEntityAdded(EntityEventArgs args)
        {
            var unit = args.Entity as Unit;

            if (unit != null && unit.IsValid)
            {
                var menu = mainMenu.OnAddRemove.Units;

                if (!menu.OnAddEnabled || menu.IgnoreUseless && Data.IgnoredUnits.Contains(unit.Name))
                {
                    return;
                }

                const Color Color = Color.Green;
                const Logger.Type Type = Logger.Type.Unit;

                logger.Write("Unit added", Type, Color, true);
                logger.Write("Name: " + unit.Name, Type, Color);
                logger.Write("ClassID: " + unit.ClassId, Type, Color);
                logger.Write("Position: " + unit.Position, Type, Color);
                logger.Write("Attack capability: " + unit.AttackCapability, Type, Color);
                logger.Write("Vision: " + unit.DayVision, Type, Color);
                logger.Write("Health: " + unit.Health, Type, Color);
                logger.EmptyLine();

                return;
            }

            var ability = args.Entity as Ability;

            if (ability != null && ability.IsValid)
            {
                var menu = mainMenu.OnAddRemove.Abilities;

                if (!menu.OnAddEnabled || menu.IgnoreUseless && Data.IgnoredAbilities.Any(ability.Name.Contains)
                    || menu.HeroesOnly && !(ability.Owner is Hero))
                {
                    return;
                }

                const Color Color = Color.Green;
                const Logger.Type Type = Logger.Type.Ability;

                logger.Write("Ability added", Type, Color, true);
                logger.Write("Name: " + ability.Name, Type, Color);
                logger.Write("ClassID: " + ability.ClassId, Type, Color);
                logger.Write("Is item: " + (ability is Item), Type, Color);
                logger.Write("Level: " + ability.Level, Type, Color);
                logger.Write("Owner name: " + ability.Owner?.Name, Type, Color);
                logger.Write("Owner classID: " + ability.Owner?.ClassId, Type, Color);
                logger.EmptyLine();
            }
        }

        private void OnEntityRemoved(EntityEventArgs args)
        {
            var unit = args.Entity as Unit;

            if (unit != null && unit.IsValid)
            {
                var menu = mainMenu.OnAddRemove.Units;

                if (!menu.OnRemoveEnabled || menu.IgnoreUseless && Data.IgnoredUnits.Contains(unit.Name))
                {
                    return;
                }

                const Color Color = Color.Red;
                const Logger.Type Type = Logger.Type.Unit;

                logger.Write("Unit removed", Type, Color, true);
                logger.Write("Name: " + unit.Name, Type, Color);
                logger.Write("ClassID: " + unit.ClassId, Type, Color);
                logger.Write("Position: " + unit.Position, Type, Color);
                logger.EmptyLine();

                return;
            }

            var ability = args.Entity as Ability;

            if (ability != null && ability.IsValid)
            {
                var menu = mainMenu.OnAddRemove.Abilities;

                if (!menu.OnRemoveEnabled || menu.IgnoreUseless && Data.IgnoredAbilities.Any(ability.Name.Contains)
                    || menu.HeroesOnly && !(ability.Owner is Hero))
                {
                    return;
                }

                const Color Color = Color.Red;
                const Logger.Type Type = Logger.Type.Ability;

                logger.Write("Ability removed", Type, Color, true);
                logger.Write("Name: " + ability.Name, Type, Color);
                logger.Write("ClassID: " + ability.ClassId, Type, Color);
                logger.Write("Is item: " + (ability is Item), Type, Color);
                logger.Write("Level: " + ability.Level, Type, Color);
                logger.Write("Owner name: " + ability.Owner?.Name, Type, Color);
                logger.Write("Owner classID: " + ability.Owner?.ClassId, Type, Color);
                logger.EmptyLine();
            }
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            switch (args.OrderId)
            {
                case OrderId.Ability:
                case OrderId.AbilityLocation:
                case OrderId.AbilityTarget:
                case OrderId.AbilityTargetRune:
                case OrderId.AbilityTargetTree:
                case OrderId.ToggleAbility:
                    if (!mainMenu.OnExecuteOrderMenu.Abilities.Enabled)
                    {
                        return;
                    }
                    break;
                case OrderId.AttackLocation:
                case OrderId.AttackTarget:
                case OrderId.MoveLocation:
                case OrderId.MoveTarget:
                case OrderId.Stop:
                case OrderId.Hold:
                case OrderId.Continue:
                case OrderId.Patrol:
                    if (!mainMenu.OnExecuteOrderMenu.AttackMove.Enabled)
                    {
                        return;
                    }
                    break;
                default:
                    if (!mainMenu.OnExecuteOrderMenu.Other.Enabled)
                    {
                        return;
                    }
                    break;
            }

            const Color Color = Color.Magenta;
            const Logger.Type Type = Logger.Type.ExecuteOrder;

            logger.Write("Executed order", Type, Color, true);
            logger.Write("Sender name: " + args.Entities.FirstOrDefault()?.Name, Type, Color);
            logger.Write("Sender classID: " + args.Entities.FirstOrDefault()?.ClassId, Type, Color);
            logger.Write("Order: " + args.OrderId, Type, Color);
            if (args.Ability != null)
            {
                logger.Write("Ability name: " + args.Ability?.Name, Type, Color);
                logger.Write("Ability classID: " + args.Ability?.ClassId, Type, Color);
            }
            if (!args.TargetPosition.IsZero)
            {
                logger.Write("Position: " + args.TargetPosition, Type, Color);
            }
            if (args.Target != null)
            {
                logger.Write("Target name: " + args.Target.Name, Type, Color);
                logger.Write("Target classID: " + args.Target.ClassId, Type, Color);
            }
            logger.EmptyLine();
        }

        private void OnFireEvent(FireEventEventArgs args)
        {
            var menu = mainMenu.GameEventsMenu.FireEvent;

            if (!menu.Enabled || menu.IgnoreUseless && Data.IgnoredFireEvents.Contains(args.GameEvent.Name))
            {
                return;
            }

            const Color Color = Color.DarkYellow;
            const Logger.Type Type = Logger.Type.GameEvent;

            logger.Write("Fire event", Type, Color, true);
            logger.Write("Name: " + args.GameEvent.Name, Type, Color);
            logger.EmptyLine();
        }

        private void OnFloatPropertyChanged(Entity sender, FloatPropertyChangeEventArgs args)
        {
            if (args.OldValue.Equals(args.NewValue))
            {
                return;
            }

            var menu = mainMenu.OnChangeMenu.Floats;

            if (!menu.Enabled || menu.HeroesOnly && !(sender is Hero)
                || menu.IgnoreUseless && Data.IgnoredFloats.Contains(args.PropertyName)
                || menu.IgnoreSemiUseless && Data.SemiIgnoredFloats.Contains(args.PropertyName))
            {
                return;
            }

            const Color Color = Color.Cyan;
            const Logger.Type Type = Logger.Type.Float;

            logger.Write("Float property changed", Type, Color, true);
            logger.Write("Sender name: " + sender.Name, Type, Color);
            logger.Write("Sender classID: " + sender.ClassId, Type, Color);
            if (sender.Owner != null)
            {
                logger.Write("Owner Name: " + sender.Owner.Name, Type, Color);
                logger.Write("Owner classID: " + sender.Owner.ClassId, Type, Color);
            }
            logger.Write("Property name: " + args.PropertyName, Type, Color);
            logger.Write("Property values: " + args.OldValue + " => " + args.NewValue, Type, Color);
            logger.EmptyLine();
        }

        private void OnGcMessageReceive(GCMessageEventArgs args)
        {
            var menu = mainMenu.GameEventsMenu.GcMessage;

            if (!menu.Enabled)
            {
                return;
            }

            const Color Color = Color.DarkYellow;
            const Logger.Type Type = Logger.Type.GameEvent;

            logger.Write("Game client message", Type, Color, true);
            logger.Write("Name: " + args.MessageID, Type, Color);
            logger.EmptyLine();
        }

        private void OnHandlePropertyChanged(Entity sender, HandlePropertyChangeEventArgs args)
        {
            var menu = mainMenu.OnChangeMenu.Handles;

            if (!menu.Enabled || menu.HeroesOnly && !(sender is Hero))
            {
                return;
            }

            const Color Color = Color.Cyan;
            const Logger.Type Type = Logger.Type.Handle;

            logger.Write("Handle property changed", Type, Color, true);
            logger.Write("Sender name: " + sender.Name, Type, Color);
            logger.Write("Sender classID: " + sender.ClassId, Type, Color);
            if (sender.Owner != null)
            {
                logger.Write("Owner Name: " + sender.Owner.Name, Type, Color);
                logger.Write("Owner classID: " + sender.Owner.ClassId, Type, Color);
            }
            logger.Write("Property name: " + args.PropertyName, Type, Color);
            logger.Write("Property value: " + args.OldValue?.Name, Type, Color);
            logger.EmptyLine();
        }

        private void OnInt32PropertyChanged(Entity sender, Int32PropertyChangeEventArgs args)
        {
            if (args.OldValue.Equals(args.NewValue))
            {
                return;
            }

            var menu = mainMenu.OnChangeMenu.Int32;

            if (!menu.Enabled || menu.HeroesOnly && !(sender is Hero)
                || menu.IgnoreUseless && Data.IgnoredInt32.Contains(args.PropertyName)
                || menu.IgnoreSemiUseless && Data.SemiIgnoredInt32.Contains(args.PropertyName))
            {
                return;
            }

            const Color Color = Color.Cyan;
            const Logger.Type Type = Logger.Type.Int32;

            logger.Write("Int32 property changed", Type, Color, true);
            logger.Write("Sender name: " + sender.Name, Type, Color);
            logger.Write("Sender classID: " + sender.ClassId, Type, Color);
            if (sender.Owner != null)
            {
                logger.Write("Owner Name: " + sender.Owner.Name, Type, Color);
                logger.Write("Owner classID: " + sender.Owner.ClassId, Type, Color);
            }
            logger.Write("Property name: " + args.PropertyName, Type, Color);
            logger.Write("Property values: " + args.OldValue + " => " + args.NewValue, Type, Color);
            logger.EmptyLine();
        }

        private void OnInt64PropertyChanged(Entity sender, Int64PropertyChangeEventArgs args)
        {
            if (args.OldValue.Equals(args.NewValue))
            {
                return;
            }

            var menu = mainMenu.OnChangeMenu.Int64;

            if (!menu.Enabled || menu.HeroesOnly && !(sender is Hero))
            {
                return;
            }

            const Color Color = Color.Cyan;
            const Logger.Type Type = Logger.Type.Int64;

            logger.Write("Int64 property changed", Type, Color, true);
            logger.Write("Sender name: " + sender.Name, Type, Color);
            logger.Write("Sender classID: " + sender.ClassId, Type, Color);
            if (sender.Owner != null)
            {
                logger.Write("Owner Name: " + sender.Owner.Name, Type, Color);
                logger.Write("Owner classID: " + sender.Owner.ClassId, Type, Color);
            }
            logger.Write("Property name: " + args.PropertyName, Type, Color);
            logger.Write("Property values: " + args.OldValue + " => " + args.NewValue, Type, Color);
            logger.EmptyLine();
        }

        private void OnMessage(MessageEventArgs args)
        {
            var menu = mainMenu.GameEventsMenu.Message;

            if (!menu.Enabled)
            {
                return;
            }

            const Color Color = Color.DarkYellow;
            const Logger.Type Type = Logger.Type.GameEvent;

            logger.Write("Message", Type, Color, true);
            logger.Write("Text: " + args.Message, Type, Color);
            logger.EmptyLine();
        }

        private void OnModifierAdded(Unit sender, ModifierChangedEventArgs args)
        {
            var menu = mainMenu.OnAddRemove.Modifiers;

            if (!menu.OnAddEnabled || menu.HeroesOnly && !(sender is Hero))
            {
                return;
            }

            var modifier = args.Modifier;

            DelayAction.Add(
                1,
                () =>
                    {
                        if (modifier == null || !modifier.IsValid
                            || menu.IgnoreUseless && Data.IgnoredModifiers.Contains(modifier.Name))
                        {
                            return;
                        }

                        const Color Color = Color.Green;
                        const Logger.Type Type = Logger.Type.Modifier;

                        logger.Write("Modifier added", Type, Color, true);
                        logger.Write("Sender name: " + sender?.Name, Type, Color);
                        logger.Write("Sender classID: " + sender?.ClassId, Type, Color);
                        logger.Write("Name: " + modifier.Name, Type, Color);
                        logger.Write("Texture name: " + modifier.TextureName, Type, Color);
                        logger.Write("Elapsed time: " + modifier.ElapsedTime, Type, Color);
                        logger.Write("Remaining time: " + modifier.RemainingTime, Type, Color);
                        logger.EmptyLine();
                    });
        }

        private void OnModifierRemoved(Unit sender, ModifierChangedEventArgs args)
        {
            var menu = mainMenu.OnAddRemove.Modifiers;

            if (!menu.OnRemoveEnabled || menu.HeroesOnly && !(sender is Hero))
            {
                return;
            }

            var modifier = args.Modifier;

            if (modifier == null || !modifier.IsValid || menu.IgnoreUseless
                && Data.IgnoredModifiers.Contains(modifier.Name))
            {
                return;
            }

            const Color Color = Color.Red;
            const Logger.Type Type = Logger.Type.Modifier;

            logger.Write("Modifier removed", Type, Color, true);
            logger.Write("Sender name: " + sender?.Name, Type, Color);
            logger.Write("Sender classID: " + sender?.ClassId, Type, Color);
            logger.Write("Name: " + modifier.Name, Type, Color);
            logger.Write("Texture name: " + modifier.TextureName, Type, Color);
            logger.EmptyLine();
        }

        private void OnParticleEffectAdded(Entity sender, ParticleEffectAddedEventArgs args)
        {
            var menu = mainMenu.OnAddRemove.Particles;

            if (!menu.OnAddEnabled || menu.HeroesOnly && !args.Name.Contains("hero"))
            {
                return;
            }

            var particle = args.ParticleEffect;

            DelayAction.Add(
                1,
                () =>
                    {
                        if (particle == null || !particle.IsValid
                            || menu.IgnoreUseless && Data.IgnoredParticles.Any(args.Name.Contains))
                        {
                            return;
                        }

                        const Color Color = Color.Green;
                        const Logger.Type Type = Logger.Type.Particle;

                        logger.Write("Particle added", Type, Color, true);
                        logger.Write("Name: " + args.Name, Type, Color);
                        logger.Write("Highest control point: " + particle.HighestControlPoint, Type, Color);

                        if (menu.ShowControlPointValues)
                        {
                            for (var i = 0u; i <= args.ParticleEffect.HighestControlPoint; i++)
                            {
                                var point = args.ParticleEffect.GetControlPoint(i);
                                if (menu.IgnoreZeroControlPointValues && point.IsZero)
                                {
                                    continue;
                                }

                                logger.Write("CP: " + i + " => " + point, Type, Color);
                            }
                        }

                        logger.EmptyLine();
                    });
        }

        private void OnStringPropertyChanged(Entity sender, StringPropertyChangeEventArgs args)
        {
            if (args.OldValue.Equals(args.NewValue))
            {
                return;
            }

            var menu = mainMenu.OnChangeMenu.Strings;

            if (!menu.Enabled || menu.HeroesOnly && !(sender is Hero))
            {
                return;
            }

            const Color Color = Color.Cyan;
            const Logger.Type Type = Logger.Type.String;

            logger.Write("String property changed", Type, Color, true);
            logger.Write("Sender name: " + sender.Name, Type, Color);
            logger.Write("Sender classID: " + sender.ClassId, Type, Color);
            if (sender.Owner != null)
            {
                logger.Write("Owner Name: " + sender.Owner.Name, Type, Color);
                logger.Write("Owner classID: " + sender.Owner.ClassId, Type, Color);
            }
            logger.Write("Property name: " + args.PropertyName, Type, Color);
            logger.Write("Property values: " + args.OldValue + " => " + args.NewValue, Type, Color);
            logger.EmptyLine();
        }

        private void OnTrackingProjectileAdded(TrackingProjectileEventArgs args)
        {
            var menu = mainMenu.OnAddRemove.Projectiles;

            if (!menu.OnAddEnabled || menu.HeroesOnly && !(args.Projectile?.Source is Hero))
            {
                return;
            }

            var projectile = args.Projectile;

            if (projectile?.Source == null)
            {
                return;
            }

            const Color Color = Color.Green;
            const Logger.Type Type = Logger.Type.Projectile;

            logger.Write("Projectile added", Type, Color, true);
            logger.Write("Source name: " + projectile.Source.Name, Type, Color);
            logger.Write("Source classID: " + projectile.Source.ClassId, Type, Color);
            logger.Write("Speed: " + projectile.Speed, Type, Color);
            logger.Write("Position: " + projectile.Position, Type, Color);
            logger.Write("Target name: " + projectile.Target?.Name, Type, Color);
            logger.Write("Target classID: " + projectile.Target?.ClassId, Type, Color);
            logger.Write("Target position: " + projectile.TargetPosition, Type, Color);
            logger.EmptyLine();
        }

        private void OnTrackingProjectileRemoved(TrackingProjectileEventArgs args)
        {
            var menu = mainMenu.OnAddRemove.Projectiles;

            if (!menu.OnRemoveEnabled || menu.HeroesOnly && !(args.Projectile?.Source is Hero))
            {
                return;
            }

            var projectile = args.Projectile;

            if (projectile?.Source == null)
            {
                return;
            }

            const Color Color = Color.Red;
            const Logger.Type Type = Logger.Type.Projectile;

            logger.Write("Projectile removed", Type, Color, true);
            logger.Write("Source name: " + projectile.Source.Name, Type, Color);
            logger.Write("Source classID: " + projectile.Source.ClassId, Type, Color);
            logger.Write("Speed: " + projectile.Speed, Type, Color);
            logger.Write("Position: " + projectile.Position, Type, Color);
            logger.Write("Target name: " + projectile.Target?.Name, Type, Color);
            logger.Write("Target classID: " + projectile.Target?.ClassId, Type, Color);
            logger.Write("Target position: " + projectile.TargetPosition, Type, Color);
            logger.EmptyLine();
        }

        private void OnUiStateChanged(UIStateChangedEventArgs args)
        {
            var menu = mainMenu.GameEventsMenu.UiState;

            if (!menu.Enabled)
            {
                return;
            }

            const Color Color = Color.DarkYellow;
            const Logger.Type Type = Logger.Type.GameEvent;

            logger.Write("UI state change", Type, Color, true);
            logger.Write("Name: " + args.UIState, Type, Color);
            logger.EmptyLine();
        }

        private void SpellsOnDump(object sender, EventArgs eventArgs)
        {
            var unit = SelectedEntity as Unit;

            if (unit == null)
            {
                return;
            }

            var menu = mainMenu.DumpMenu.Spells;

            const Color Color = Color.Yellow;
            const Logger.Type Type = Logger.Type.Spell;

            logger.Write("Spell dump", Type, Color, true);
            logger.Write("Unit name: " + unit.Name, Type, Color);
            logger.Write("Unit classID: " + unit.ClassId, Type, Color);
            logger.EmptyLine();

            foreach (var spell in unit.Spellbook.Spells)
            {
                if (!menu.ShowHidden && spell.IsHidden)
                {
                    continue;
                }

                if (!menu.ShowTalents && spell.Name.StartsWith("special_"))
                {
                    continue;
                }

                logger.Write("Name: " + spell.Name, Type, Color);
                logger.Write("ClassID: " + spell.ClassId, Type, Color);

                if (menu.ShowLevels)
                {
                    logger.Write("Level: " + spell.Level, Type, Color);
                }
                if (menu.ShowManaCost)
                {
                    logger.Write("Mana cost: " + spell.ManaCost, Type, Color);
                }
                if (menu.ShowCastRange)
                {
                    logger.Write("Cast range: " + spell.GetCastRange(), Type, Color);
                }
                if (menu.ShowBehavior)
                {
                    logger.Write("Behavior: " + spell.AbilityBehavior, Type, Color);
                }
                if (menu.ShowTargetType)
                {
                    logger.Write("Target type: " + spell.TargetType, Type, Color);
                    logger.Write("Target team type: " + spell.TargetTeamType, Type, Color);
                }
                if (menu.ShowSpecialData)
                {
                    logger.Write("Special data =>", Type, Color);
                    foreach (var abilitySpecialData in spell.AbilitySpecialData)
                    {
                        logger.Write("  " + abilitySpecialData.Name + ": " + abilitySpecialData.Value, Type, Color);
                    }
                }

                logger.EmptyLine();
            }

            logger.EmptyLine();
        }

        private void UnitsOnDump(object sender, EventArgs e)
        {
            var unit = SelectedEntity as Unit;

            if (unit == null)
            {
                return;
            }

            const Color Color = Color.Yellow;
            const Logger.Type Type = Logger.Type.Unit;

            logger.Write("Unit dump", Type, Color, true);
            logger.Write("Name: " + unit.Name, Type, Color);
            logger.Write("ClassID: " + unit.ClassId, Type, Color);
            logger.Write("Level: " + unit.Level, Type, Color);
            logger.Write("Position: " + unit.Position, Type, Color);
            logger.Write("Team: " + unit.Team, Type, Color);
            logger.Write("Health: " + unit.Health + "/" + unit.MaximumHealth, Type, Color);
            logger.Write("Mana: " + (int)unit.Mana + "/" + (int)unit.MaximumMana, Type, Color);
            logger.Write("AttackCapability: " + unit.AttackCapability, Type, Color);
            logger.Write("Day vision: " + unit.DayVision, Type, Color);
            logger.Write("Night vision: " + unit.NightVision, Type, Color);
            logger.Write("State: " + unit.UnitState, Type, Color);
            logger.EmptyLine();
            logger.Write("Spells =>", Type, Color);
            logger.Write(
                "  Talents count: " + unit.Spellbook.Spells.Count(x => x.Name.StartsWith("special_")),
                Type,
                Color);
            logger.Write(
                "  Active spells count: " + unit.Spellbook.Spells.Count(
                    x => !x.Name.StartsWith("special_") && x.AbilityBehavior != AbilityBehavior.Passive),
                Type,
                Color);
            logger.Write(
                "  Passive spells count: " + unit.Spellbook.Spells.Count(
                    x => !x.Name.StartsWith("special_") && x.AbilityBehavior == AbilityBehavior.Passive),
                Type,
                Color);
            if (unit.HasInventory)
            {
                logger.EmptyLine();
                logger.Write("Items =>", Type, Color);
                logger.Write("  Inventory Items count: " + unit.Inventory.Items.Count(), Type, Color);
                logger.Write("  Backpack Items count: " + unit.Inventory.Backpack.Count(), Type, Color);
                logger.Write("  Stash Items count: " + unit.Inventory.Stash.Count(), Type, Color);
            }
            logger.EmptyLine();
            logger.Write("Modifiers =>", Type, Color);
            logger.Write("  Active modifiers count: " + unit.Modifiers.Count(x => !x.IsHidden), Type, Color);
            logger.Write("  Hidden modifiers count: " + unit.Modifiers.Count(x => x.IsHidden), Type, Color);
            logger.EmptyLine();
        }
    }
}