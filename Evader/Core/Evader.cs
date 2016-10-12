namespace Evader.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using EvadableAbilities.Base;
    using EvadableAbilities.Base.Interfaces;
    using EvadableAbilities.Heroes;

    using SharpDX;

    using UsableAbilities.Base;
    using UsableAbilities.Items;

    using Utils;

    using Projectile = EvadableAbilities.Base.Projectile;

    internal class Evader
    {
        #region Fields

        private readonly List<uint> addedAbilities = new List<uint>();

        private readonly List<EvadableAbility> evadableAbilities = new List<EvadableAbility>();

        private readonly List<UsableAbility> usableAbilities = new List<UsableAbility>();

        private List<Vector3> debugPath = new List<Vector3>();

        private Unit fountain;

        private Vector3 movePosition;

        private MultiSleeper sleeper;

        private StatusDrawer statusDrawer;

        #endregion

        #region Properties

        private static Hero Hero => Variables.Hero;

        private static Team HeroTeam => Variables.HeroTeam;

        private static MenuManager Menu => Variables.Menu;

        private static Pathfinder Pathfinder => Variables.Pathfinder;

        #endregion

        #region Public Methods and Operators

        public void OnAddEntity(EntityEventArgs args)
        {
            if (!Menu.Enabled)
            {
                return;
            }

            var unit = args.Entity as Unit;

            if (unit == null || !unit.IsValid)
            {
                return;
            }

            Debugger.WriteLine("====", Debugger.Type.Units);
            Debugger.WriteLine("unit name: " + unit.Name, Debugger.Type.Units);
            Debugger.WriteLine("unit id: " + unit.ClassID, Debugger.Type.Units);
            Debugger.WriteLine("vision: " + unit.DayVision, Debugger.Type.Units);
            if (unit.Owner != null && unit.Owner.IsValid)
            {
                Debugger.WriteLine("owner: " + unit.Owner.Name, Debugger.Type.Units);
            }
            else
            {
                Debugger.WriteLine("owner not valid", Debugger.Type.Units);
            }

            if (unit.ClassID == ClassID.CDOTA_BaseNPC && unit.DayVision == 650)
            {
                (evadableAbilities.FirstOrDefault(x => x.Name == "mirana_arrow") as SacredArrow)?.AddUnit(unit);
            }
        }

        public void OnAddTrackingProjectile(TrackingProjectileEventArgs args)
        {
            Debugger.WriteLine("==== projectile", Debugger.Type.Projectiles);
            if (args.Projectile.Source != null)
            {
                Debugger.WriteLine("source: " + args.Projectile.Source.Name, Debugger.Type.Projectiles);
            }
            Debugger.WriteLine("speed: " + args.Projectile.Speed, Debugger.Type.Projectiles);
            if (args.Projectile.Target != null)
            {
                Debugger.WriteLine("target: " + args.Projectile.Target.Name, Debugger.Type.Projectiles);
            }
        }

        public void OnClose()
        {
            Pathfinder.Close();
            Menu.Close();
            evadableAbilities.Clear();
            usableAbilities.Clear();
            addedAbilities.Clear();
        }

        public void OnDraw()
        {
            statusDrawer.Draw();

            if (Menu.DebugAbilities)
            {
                foreach (var evadableAbility in evadableAbilities)
                {
                    evadableAbility.Draw();
                }
            }

            if (Menu.DebugMap)
            {
                MapDrawer.Draw(debugPath);
            }
        }

        public void OnExecuteAction(Player sender, ExecuteOrderEventArgs args)
        {
            if (!args.Entities.Contains(Hero) || !Menu.Enabled)
            {
                return;
            }

            switch (args.Order)
            {
                case Order.MoveLocation:
                    movePosition = args.TargetPosition;

                    if (sleeper.Sleeping("block"))
                    {
                        args.Process = false;
                    }

                    if (sleeper.Sleeping("avoiding"))
                    {
                        if (Menu.BlockPlayerMovement)
                        {
                            args.Process = false;
                            return;
                        }

                        var obstacles = Pathfinder.GetIntersectingObstacles(movePosition, Hero.HullRadius).ToList();
                        var ability =
                            evadableAbilities.FirstOrDefault(
                                x => x.Obstacle != null && obstacles.Contains(x.Obstacle.Value));

                        if (ability == null)
                        {
                            return;
                        }

                        Debugger.WriteLine("avoiding OnExecute");

                        bool success;

                        var pathFromObstalce =
                            Pathfinder.CalculatePathFromObstacle(movePosition, 5, out success).ToList();

                        if (success)
                        {
                            movePosition = pathFromObstalce.Last();
                        }

                        var path = Pathfinder.CalculateLongPath(movePosition, out success).ToList();

                        if (success)
                        {
                            debugPath = Pathfinder.CalculateLongDebugPath(movePosition).ToList();
                            args.Process = false;

                            for (var i = 0; i < path.Count; i++)
                            {
                                Hero.Move(path[i], i != 0);
                            }

                            sleeper.Sleep(100, "block");
                            sleeper.Sleep(1000, debugPath);
                            Debugger.WriteLine("move OnExecute // block player movement");
                        }
                    }
                    break;
                case Order.AbilityTarget:
                case Order.AbilityLocation:
                    movePosition = args.TargetPosition;
                    break;
                default:
                    movePosition = new Vector3();
                    break;
            }
        }

        public void OnLoad()
        {
            sleeper = new MultiSleeper();
            statusDrawer = new StatusDrawer();

            Variables.Hero = ObjectManager.LocalHero;
            Variables.Menu = new MenuManager();
            Variables.Pathfinder = new Pathfinder();
            Variables.HeroTeam = Hero.Team;
            fountain =
                ObjectManager.GetEntities<Unit>()
                    .First(x => x.ClassID == ClassID.CDOTA_Unit_Fountain && x.Team == HeroTeam);

            Debugger.WriteLine();
            Debugger.WriteLine("***** Evader");
            Debugger.WriteLine(
                "* Total abilities countered: " + Data.Abilities.Count + " ("
                + Assembly.GetExecutingAssembly()
                      .GetTypes()
                      .Count(
                          x =>
                          x.IsClass && x.IsNotPublic
                          && (x.Namespace == "Evader.EvadableAbilities.Heroes"
                              || x.Namespace == "Evader.EvadableAbilities.Units"
                              || x.Namespace == "Evader.EvadableAbilities.Items")) + " unique)");
            Debugger.WriteLine("* Total usable evade abilities: " + Data.EvadeCounterAbilities.Count);
            Debugger.WriteLine("* Total usable blink abilities: " + Data.EvadeBlinkAbilities.Count);
            Debugger.WriteLine("* Total usable disable abilities: " + Data.EvadeDisableAbilities.Count);
            Debugger.WriteLine("***** Hero: " + Hero.GetRealName());
            Debugger.WriteLine("* Turn rate: " + Hero.GetTurnRate().ToString("0.0"));
            Debugger.WriteLine("* Hull radius: " + Hero.HullRadius);
            Debugger.Write("* Default priority: ");
            var priority = Menu.DefaultPriority;
            for (var i = 0; i < priority.Count; i++)
            {
                Debugger.Write(priority[i].ToString());
                if (priority.Count - 1 > i)
                {
                    Debugger.Write(" => ");
                }
            }
            Debugger.WriteLine();
        }

        public void OnModifierAdded(Unit sender, Modifier modifier)
        {
            if (!Menu.Enabled)
            {
                return;
            }

            Debugger.WriteLine("====", Debugger.Type.Modifiers);
            Debugger.WriteLine("modifier name: " + modifier.Name, Debugger.Type.Modifiers);
            Debugger.WriteLine("modifier tname: " + modifier.TextureName, Debugger.Type.Modifiers);
            Debugger.WriteLine("sender: " + sender.Name, Debugger.Type.Modifiers);
            Debugger.WriteLine("e time: " + modifier.ElapsedTime, Debugger.Type.Modifiers);
            Debugger.WriteLine("r time: " + modifier.RemainingTime, Debugger.Type.Modifiers);

            string name;
            if (Data.AbilityModifiers.TryGetValue(modifier.Name, out name))
            {
                var ability = evadableAbilities.FirstOrDefault(x => x.Name == name && x.Enabled) as IModifierThinker;
                ability?.AddModifierThinker(modifier, sender);
                return;
            }

            //if (sender.Team == HeroTeam && modifier.IsStunDebuff && !sender.Equals(Hero))
            //{
            //    var strongDispel =
            //        usableAbilities.FirstOrDefault(x => x.CanBeUsedOnAlly && x.StrongDispel && x.CanBeCasted(sender));
            //    strongDispel?.Use(null, sender);
            //    return;
            //}

            //var hero = sender as Hero;

            //if (hero == null)
            //{
            //    return;
            //}

            //var modifierAbilityClassID = modifier.AbilityClassID();
            //if (modifierAbilityClassID == null)
            //{
            //    Console.WriteLine("null");
            //    return;
            //}

            //var modiferCounterAbility =
            //    evadableAbilities.FirstOrDefault(
            //        x => x.ModifierCounterEnabled && (x.ModifierName == modifier.TextureName || x.ModifierName == modifier.Name)) as IModifier;

            //modiferCounterAbility?.AddModifer(modifier, hero);
        }

        public void OnModifierRemoved(Modifier modifier)
        {
            Debugger.WriteLine("====", Debugger.Type.Modifiers);
            Debugger.WriteLine("-modifier name: " + modifier.Name, Debugger.Type.Modifiers);
            Debugger.WriteLine("-modifier tname: " + modifier.TextureName, Debugger.Type.Modifiers);
            //var modifierAbility = modifier.Ability;

            //if (modifierAbility == null)
            //{
            //    return;
            //}

            //Console.WriteLine(modifierAbility.Name);
            //Console.WriteLine(modifierAbility.ClassID);

            //var modiferCounterAbility =
            //    evadableAbilities.FirstOrDefault(x => x.ModifierCounterEnabled && x.ModifierName == modifier.Name) as
            //    IModifier;

            //modiferCounterAbility?.RemoveModifier();
        }

        public void OnParticleEffectAdded(Entity sender, ParticleEffectAddedEventArgs args)
        {
            if (!Menu.Enabled)
            {
                return;
            }

            var particleName = args.Name;

            if (Menu.DebugConsoleParticles)
            {
                if (particleName.Contains("ui_mouseactions") || particleName.Contains("generic_hit_blood")
                    || particleName.Contains("base_attacks") || particleName.Contains("generic_gameplay")
                    || particleName.Contains("ensage_ui"))
                {
                    return;
                }

                Debugger.WriteLine("====", Debugger.Type.Particles);
                Debugger.WriteLine("particle: " + particleName, Debugger.Type.Particles);
            }

            var abilityName = Data.AbilityParticles.FirstOrDefault(x => particleName.Contains(x.Key)).Value;

            if (string.IsNullOrEmpty(abilityName))
            {
                return;
            }

            var ability = evadableAbilities.FirstOrDefault(x => x.Name == abilityName && x.Enabled) as IParticle;
            ability?.AddParticle(args.ParticleEffect);
        }

        public void OnRemoveEntity(Entity entity)
        {
            evadableAbilities.RemoveAll(x => x.Handle == entity.Handle || x.OwnerHandle == entity.Handle);
            usableAbilities.RemoveAll(x => x.Handle == entity.Handle);
        }

        public void OnUpdate()
        {
            if (!Game.IsInGame || Game.IsPaused || !Menu.Enabled)
            {
                return;
            }

            if (Menu.DebugMap && debugPath.Any() && !sleeper.Sleeping(debugPath))
            {
                debugPath.Clear();
            }

            if (!sleeper.Sleeping(addedAbilities))
            {
                foreach (var unit in
                    ObjectManager.GetEntitiesParallel<Unit>()
                        .Where(
                            x =>
                            (x is Hero || x is Creep) && x.IsValid && x.IsAlive
                            && (!x.IsIllusion
                                || x.HasModifiers(
                                    new[]
                                        {
                                            "modifier_arc_warden_tempest_double",
                                            "modifier_vengefulspirit_hybrid_special"
                                        },
                                    false))))
                {
                    var abilities = new List<Ability>();

                    try
                    {
                        abilities.AddRange(unit.Spellbook.Spells);
                    }
                    catch (Exception e)
                    {
                        Debugger.WriteLine("=============>>>>>>>>>>>>>>>>>>>> Evader exception: " + e);
                        Debugger.WriteLine("Unit name: " + unit.Name);
                        Debugger.WriteLine("Unit position: " + unit.Position);
                        Debugger.WriteLine("Unit alive: " + unit.IsAlive);
                        Debugger.WriteLine("Unit spawned: " + unit.IsSpawned);
                        Debugger.WriteLine("Unit valid: " + unit.IsValid);
                        Debugger.WriteLine("Unit visible: " + unit.IsVisible);
                    }

                    if (unit.HasInventory)
                    {
                        abilities.AddRange(unit.Inventory.Items);
                    }

                    foreach (var ability in
                        abilities.Where(x => x.IsValid && !addedAbilities.Contains(x.Handle) && x.Level > 0))
                    {
                        if (unit.Equals(Hero))
                        {
                            var abilityName = ability.Name;

                            Func<Ability, UsableAbility> func;
                            if (Data.EvadeCounterAbilities.TryGetValue(abilityName, out func))
                            {
                                usableAbilities.Add(func.Invoke(ability));
                                Menu.AddUsableCounterAbility(abilityName);
                            }
                            if (Data.EvadeDisableAbilities.TryGetValue(abilityName, out func))
                            {
                                usableAbilities.Add(func.Invoke(ability));
                                Menu.AddUsableDisableAbility(abilityName);
                            }
                            if (Data.EvadeBlinkAbilities.TryGetValue(abilityName, out func))
                            {
                                usableAbilities.Add(func.Invoke(ability));
                                Menu.AddUsableBlinkAbility(abilityName);
                            }
                        }
                        else if (unit.Team != HeroTeam
                                 || ability.ClassID == ClassID.CDOTA_Ability_FacelessVoid_Chronosphere)
                        {
                            Func<Ability, EvadableAbility> func;
                            if (Data.Abilities.TryGetValue(ability.Name, out func))
                            {
                                var evadableAbility = func.Invoke(ability);
                                evadableAbilities.Add(evadableAbility);
                                Menu.AddEvadableAbility(evadableAbility);
                            }
                        }

                        addedAbilities.Add(ability.Handle);
                    }
                }

                sleeper.Sleep(3000, addedAbilities);
            }

            var heroCanCast = Hero.CanCast();
            var heroCanUseItems = Hero.CanUseItems();

            foreach (var ability in evadableAbilities)
            {
                if (!ability.Enabled)
                {
                    continue;
                }

                ability.Check();

                //var modifierCounter = ability as IModifier;

                //if (modifierCounter != null && !sleeper.Sleeping(ability.ModifierName)
                //    && modifierCounter.CanBeCountered())
                //{
                //    var unit = modifierCounter.GetModifierHero();
                //    if (unit != null && unit.IsValid)
                //    {
                //        var modifierRemainingTime = modifierCounter.GetModiferRemainingTime();

                //        var modifierCounterAbilities =
                //            from abilityName in
                //                unit.Team == HeroTeam ? ability.ModifierAllyCounter : ability.ModifierEnemyCounter
                //            join usableAbility in usableAbilities on abilityName equals usableAbility.Name
                //            where
                //                usableAbility.Type == AbilityType.Counter
                //                && Menu.UsableAbilityEnabled(abilityName, AbilityType.Counter)
                //                && (usableAbility.IsItem && heroCanUseItems || !usableAbility.IsItem && heroCanCast)
                //            select usableAbility;

                //        foreach (var modifierCounterAbility in modifierCounterAbilities)
                //        {
                //            if (!modifierCounterAbility.CanBeCasted(unit))
                //            {
                //                continue;
                //            }

                //            var requiredTime = modifierCounterAbility.GetRequiredTime(ability, unit) + Game.Ping / 1000
                //                               + 0.5f;

                //            if (requiredTime <= modifierRemainingTime)
                //            {
                //                modifierCounterAbility.Use(ability, unit);
                //                sleeper.Sleep(modifierRemainingTime * 1000, ability.ModifierName);
                //            }
                //        }
                //    }
                //}

                if (ability.IsStopped())
                {
                    ability.End();

                    if (sleeper.Sleeping(ability))
                    {
                        sleeper.Reset(ability);
                        sleeper.Reset("avoiding");

                        if (!Hero.IsChanneling() && !Hero.IsInvul())
                        {
                            Debugger.WriteLine("Hero stop");
                            Hero.Stop();
                        }
                    }
                }

                if (ability.Obstacle != null && Math.Abs(ability.GetRemainingTime()) > 60)
                {
                    Console.WriteLine("////////// Evader // Critical // Broken ability: " + ability.Name);
                    Console.WriteLine("////////// Evader // Critical // Broken ability: " + ability.Name);
                    Console.WriteLine("////////// Evader // Critical // Broken ability: " + ability.Name);
                    Console.WriteLine("////////// Evader // Critical // Broken ability: " + ability.Name);
                    Console.WriteLine("////////// Evader // Critical // Broken ability: " + ability.Name);
                    ability.End();
                }
            }

            if (!Hero.IsAlive)
            {
                return;
            }

            if (Menu.ForceBlink)
            {
                var blinkDagger =
                    usableAbilities.FirstOrDefault(x => x.ClassID == ClassID.CDOTA_Item_BlinkDagger) as BlinkDagger;

                if (blinkDagger != null && blinkDagger.CanBeCasted(null) && heroCanUseItems)
                {
                    blinkDagger.UseInFront();
                }
            }

            if (Hero.IsChanneling())
            {
                return;
            }

            foreach (var projectile in ObjectManager.TrackingProjectiles.Where(x => x?.Target is Hero))
            {
                var source = projectile.Source;
                if (source == null)
                {
                    var predictedAbilities =
                        evadableAbilities.OfType<Projectile>()
                            .Where(x => (int)x.GetProjectileSpeed() == projectile.Speed && x.TimeSinceCast() < 1.5)
                            .ToList();
                    if (predictedAbilities.Count == 1)
                    {
                        Debugger.WriteLine(
                            "predicted: " + predictedAbilities.First().Name + " => "
                            + ((Hero)projectile.Target).GetRealName(),
                            Debugger.Type.Projectiles);
                        predictedAbilities.First().SetProjectile(projectile.Position, (Hero)projectile.Target);
                    }
                    continue;
                }

                if (!source.IsValid || source.Team == HeroTeam)
                {
                    continue;
                }

                var ability =
                    evadableAbilities.OfType<Projectile>()
                        .FirstOrDefault(
                            x =>
                            x.OwnerClassID == source.ClassID && (int)x.GetProjectileSpeed() == projectile.Speed
                            && x.TimeSinceCast() < 1.5);

                if (ability != null)

                {
                    Debugger.WriteLine(
                        "projectile: " + ability.Name + " => " + ((Hero)projectile.Target).GetRealName(),
                        Debugger.Type.Projectiles);
                    ability.SetProjectile(projectile.Position, (Hero)projectile.Target);
                }
            }

            var allies =
                ObjectManager.GetEntitiesParallel<Hero>()
                    .Where(
                        x =>
                        x.Equals(Hero)
                        || (Menu.HelpAllies && x.IsValid && x.Team == HeroTeam && x.IsAlive && !x.IsIllusion));

            var obstacles = allies.ToDictionary(x => x, x => Pathfinder.GetIntersectingObstacles(x));

            //if (Menu.HelpAllies)
            //{
            //    var multiIntersection = obstacles.SelectMany(x => x.Value).GroupBy(x => x).SelectMany(x => x.Skip(1));

            //    foreach (var obstacle in multiIntersection)
            //    {
            //        var ability = evadableAbilities.FirstOrDefault(x => x.Obstacle == obstacle && x.IsDisable) as AOE;

            //        if (ability != null)
            //        {
            //            var remainingTime = ability.GetRemainingDisableTime() - Game.Ping / 1000;

            //            var disable =
            //                usableAbilities.FirstOrDefault(
            //                    x =>
            //                    x.Type == AbilityType.Disable && ability.CounterAbilities.Contains(x.Name)
            //                    && x.CanBeCasted(ability.Owner));

            //            if (disable != null && remainingTime - disable.GetRequiredTime(ability, ability.Owner) <= 0.15)
            //            {
            //                disable.Use(ability, ability.Owner);
            //                sleeper.Sleep(remainingTime * 1000 + 500, ability);
            //            }
            //        }
            //    }
            //}

            foreach (var obstaclePair in obstacles.OrderByDescending(x => x.Key.Equals(Hero)))
            {
                var ally = obstaclePair.Key;
                var allyLinkens = ally.IsLinkensProtected();
                var allyMagicImmune = ally.IsMagicImmune();
                var allyIsMe = ally.Equals(Hero);

                foreach (var ability in
                    obstaclePair.Value.Select(x => evadableAbilities.FirstOrDefault(z => z.Obstacle == x))?
                        .OrderByDescending(x => x.IsDisable))
                {
                    if (ability != null)
                    {
                        if (sleeper.Sleeping(ability))
                        {
                            continue;
                        }

                        Debugger.WriteLine(
                            ally.GetRealName() + " intersecting: " + ability.Name,
                            Debugger.Type.Intersectons);

                        if ((allyLinkens && (ability is Projectile || ability is LinearTarget))
                            || (!ability.PiercesMagicImmunity && allyMagicImmune))
                        {
                            continue;
                        }

                        var abilityOwner = ability.AbilityOwner;
                        var remainingTime = ability.GetRemainingTime(ally);

                        foreach (var priority in
                            ability.UseCustomPriority ? ability.Priority : Menu.DefaultPriority)
                        {
                            switch (priority)
                            {
                                case Priority.Walk:
                                    if (allyIsMe && !ability.IgnorePathfinder && Menu.EnabledPathfinder
                                        && Hero.CanMove())
                                    {
                                        var remainingWalkTime = remainingTime - 0.1f;

                                        if (sleeper.Sleeping("block"))
                                        {
                                            continue;
                                        }

                                        bool success;
                                        List<Vector3> path;

                                        if (Hero.IsMoving && !movePosition.IsZero)
                                        {
                                            if (ability.ObstacleStays && remainingWalkTime <= 0)
                                            {
                                                remainingWalkTime = ability.ObstacleRemainingTime();
                                            }

                                            if (remainingWalkTime <= 0)
                                            {
                                                continue;
                                            }

                                            if (Pathfinder.GetIntersectingObstacles(movePosition, Hero.HullRadius).Any()
                                                || Pathfinder.GetIntersectingObstacles(
                                                    Hero.NetworkPosition,
                                                    Hero.HullRadius).Any())
                                            {
                                                Debugger.WriteLine("moving into obstacle");

                                                var tempPath =
                                                    Pathfinder.CalculatePathFromObstacle(
                                                        movePosition,
                                                        remainingWalkTime,
                                                        out success).ToList();

                                                debugPath =
                                                    Pathfinder.CalculateDebugPathFromObstacle(remainingWalkTime)
                                                        .ToList();

                                                if (success)
                                                {
                                                    movePosition = tempPath.Last();
                                                }
                                                else
                                                {
                                                    continue;
                                                }
                                            }

                                            path = Pathfinder.CalculateLongPath(movePosition, out success).ToList();

                                            if (success)
                                            {
                                                var time = 0.1f;
                                                for (var i = 0; i < path.Count; i++)
                                                {
                                                    Hero.Move(path[i], i != 0);
                                                    time += Hero.NetworkPosition.Distance2D(path[i])
                                                            / Hero.MovementSpeed;
                                                }

                                                //  sleeper.Sleep(ability.ObstacleRemainingTime() * 1000, "avoiding");
                                                sleeper.Sleep(200, "block");
                                                sleeper.Sleep(Math.Min(time, 1) * 1000, ability);
                                                sleeper.Sleep(Math.Min(time, 1) * 1000, "avoiding");
                                                sleeper.Sleep(1000, debugPath);

                                                Debugger.WriteLine(">>>>>>>>>>>>>>>");
                                                Debugger.WriteLine("avoid while moving: " + ability.Name);
                                                Debugger.WriteLine("remaining time: " + remainingWalkTime);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            path =
                                                Pathfinder.CalculatePathFromObstacle(remainingWalkTime, out success)
                                                    .ToList();

                                            if (success)
                                            {
                                                debugPath =
                                                    Pathfinder.CalculateDebugPathFromObstacle(remainingWalkTime)
                                                        .ToList();
                                                var time = 0.1f;

                                                for (var i = 0; i < path.Count; i++)
                                                {
                                                    Hero.Move(path[i], i != 0);
                                                    time += Hero.NetworkPosition.Distance2D(path[i])
                                                            / Hero.MovementSpeed;
                                                }

                                                //   sleeper.Sleep(ability.ObstacleRemainingTime() * 1000, "avoiding");
                                                sleeper.Sleep(Math.Min(time, 1) * 1000, "avoiding");
                                                sleeper.Sleep(Math.Min(time, 1) * 1000, ability);
                                                sleeper.Sleep(1000, debugPath);

                                                Debugger.WriteLine(">>>>>>>>>>>>>>>");
                                                Debugger.WriteLine("avoid while standing: " + ability.Name);
                                                Debugger.WriteLine("remaining time: " + remainingTime);
                                                return;
                                            }
                                        }
                                    }

                                    break;
                                case Priority.Blink:
                                    if (!allyIsMe)
                                    {
                                        continue;
                                    }

                                    var blinkAbilities = from abilityName in ability.BlinkAbilities
                                                         join usableAbility in usableAbilities on abilityName equals
                                                             usableAbility.Name
                                                         where
                                                             usableAbility.Type == AbilityType.Blink
                                                             && Menu.UsableAbilityEnabled(
                                                                 abilityName,
                                                                 AbilityType.Blink)
                                                             && (usableAbility.IsItem && heroCanUseItems
                                                                 || !usableAbility.IsItem && heroCanCast)
                                                         select usableAbility;

                                    foreach (var blinkAbility in blinkAbilities)
                                    {
                                        if (!blinkAbility.CanBeCasted(fountain))
                                        {
                                            continue;
                                        }

                                        var requiredTime = blinkAbility.GetRequiredTime(ability, fountain)
                                                           + Game.Ping / 1000;

                                        var time = remainingTime - requiredTime;

                                        var ignoreRemainingTime = ability.IgnoreRemainingTime(blinkAbility, time);

                                        if (requiredTime > remainingTime && !ignoreRemainingTime)
                                        {
                                            continue;
                                        }

                                        if (time <= 0.10 && time > 0 || ignoreRemainingTime)
                                        {
                                            blinkAbility.Use(ability, fountain);
                                            sleeper.Sleep(Math.Min(ability.GetSleepTime(), 1000), ability);

                                            Debugger.WriteLine(">>>>>>>>>>>>>>>");
                                            Debugger.WriteLine("blink: " + blinkAbility.Name + " => " + ability.Name);
                                            Debugger.WriteLine("ally: " + ally.GetRealName());
                                            Debugger.WriteLine("remaining time: " + remainingTime);
                                            Debugger.WriteLine("required time: " + requiredTime);
                                        }

                                        return;
                                    }

                                    break;
                                case Priority.Counter:
                                    var counterAbilities = from abilityName in ability.CounterAbilities
                                                           join usableAbility in usableAbilities on abilityName equals
                                                               usableAbility.Name
                                                           where
                                                               usableAbility.Type == AbilityType.Counter
                                                               && Menu.UsableAbilityEnabled(
                                                                   abilityName,
                                                                   AbilityType.Counter)
                                                               && (usableAbility.IsItem && heroCanUseItems
                                                                   || !usableAbility.IsItem && heroCanCast)
                                                           select usableAbility;

                                    foreach (var counterAbility in counterAbilities)
                                    {
                                        var targetEnemy = counterAbility.TargetEnemy;

                                        if (!counterAbility.CanBeUsedOnAlly && !allyIsMe && !targetEnemy
                                            || !targetEnemy && !counterAbility.CanBeCasted(ally)
                                            || targetEnemy && !counterAbility.CanBeCasted(abilityOwner))
                                        {
                                            continue;
                                        }

                                        var requiredTime = counterAbility.GetRequiredTime(
                                            ability,
                                            targetEnemy ? abilityOwner : ally) + Game.Ping / 1000;

                                        var ignoreRemainingTime = false;

                                        var time = remainingTime - requiredTime;

                                        if (counterAbility.Name == Abilities.SleightOfFist)
                                        {
                                            var projectile = ability as Projectile;
                                            if (projectile != null)
                                            {
                                                ignoreRemainingTime = projectile.ProjectileLaunched();
                                            }
                                        }
                                        else
                                        {
                                            ignoreRemainingTime = ability.IgnoreRemainingTime(counterAbility, time);
                                        }

                                        if (requiredTime > remainingTime && !ignoreRemainingTime)
                                        {
                                            continue;
                                        }

                                        if (time <= 0.10 && time > 0 || ignoreRemainingTime)
                                        {
                                            counterAbility.Use(ability, targetEnemy ? abilityOwner : ally);
                                            sleeper.Sleep(ability.GetSleepTime(), ability);

                                            Debugger.WriteLine(">>>>>>>>>>>>>>>");
                                            Debugger.WriteLine(
                                                "counter: " + counterAbility.Name + " => " + ability.Name);
                                            Debugger.WriteLine("ally: " + ally.GetRealName());
                                            Debugger.WriteLine("remaining time: " + remainingTime);
                                            Debugger.WriteLine("required time: " + requiredTime);
                                        }

                                        return;
                                    }

                                    break;
                                case Priority.Disable:
                                    if (abilityOwner.IsMagicImmune() || abilityOwner.IsInvul())
                                    {
                                        continue;
                                    }

                                    var disableAbilities = from abilityName in ability.DisableAbilities
                                                           join usableAbility in usableAbilities on abilityName equals
                                                               usableAbility.Name
                                                           where
                                                               usableAbility.Type == AbilityType.Disable
                                                               && Menu.UsableAbilityEnabled(
                                                                   abilityName,
                                                                   AbilityType.Disable)
                                                               && (usableAbility.IsItem && heroCanUseItems
                                                                   || !usableAbility.IsItem && heroCanCast)
                                                           select usableAbility;

                                    foreach (var disableAbility in disableAbilities)
                                    {
                                        if (!disableAbility.CanBeCasted(abilityOwner))
                                        {
                                            continue;
                                        }

                                        var requiredTime = disableAbility.GetRequiredTime(ability, abilityOwner)
                                                           + Game.Ping / 1000;

                                        var remainingDisableTime = ability.GetRemainingDisableTime();

                                        var ignoreRemainingTime = ability.IgnoreRemainingTime(
                                            disableAbility,
                                            remainingDisableTime);

                                        if (requiredTime > remainingDisableTime && !ignoreRemainingTime)
                                        {
                                            continue;
                                        }

                                        if (remainingDisableTime - requiredTime <= 0.10 || ignoreRemainingTime)
                                        {
                                            disableAbility.Use(ability, abilityOwner);
                                            sleeper.Sleep(ability.GetSleepTime(), ability);

                                            Debugger.WriteLine(
                                                "disable: " + disableAbility.Name + " => " + ability.Name);
                                            Debugger.WriteLine("ally: " + ally.GetRealName());
                                            Debugger.WriteLine("remaining time: " + remainingDisableTime);
                                            Debugger.WriteLine("required time: " + requiredTime);
                                        }
                                        return;
                                    }

                                    break;
                            }
                        }
                    }

                    if (allyIsMe)
                    {
                        return;
                    }
                }
            }
        }

        #endregion
    }
}