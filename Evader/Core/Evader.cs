namespace Evader.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Common;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;
    using Ensage.Common.Objects.UtilityObjects;

    using EvadableAbilities.Base;
    using EvadableAbilities.Base.Interfaces;
    using EvadableAbilities.Heroes.Mirana;

    using Menus;

    using SharpDX;

    using UsableAbilities.Items;

    using AbilityType = Data.AbilityType;
    using Projectile = EvadableAbilities.Base.Projectile;
    using Utils = Ensage.Common.Utils;

    internal class Evader
    {
        #region Fields

        private AbilityUpdater abilityUpdater;

        private List<Vector3> debugPath = new List<Vector3>();

        private Unit fountain;

        private ParticleEffect heroPathfinderEffect;

        private Vector3 movePosition;

        private Random randomiser;

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
            if (!Menu.Hotkeys.EnabledEvader)
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
                (abilityUpdater.EvadableAbilities.FirstOrDefault(x => x.Name == "mirana_arrow") as SacredArrow)?.AddUnit
                    (unit);
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
            Pathfinder.Dispose();
            Menu.Dispose();
            abilityUpdater.Dispose();
        }

        public void OnDraw()
        {
            statusDrawer.Draw();

            if (Menu.Debug.DrawAbilities)
            {
                foreach (var evadableAbility in abilityUpdater.EvadableAbilities)
                {
                    evadableAbility.Draw();
                }
            }

            if (Menu.Debug.DrawMap)
            {
                MapDrawer.Draw(debugPath);
            }
        }

        public void OnExecuteAction(Player sender, ExecuteOrderEventArgs args)
        {
            if (!args.Entities.Contains(Hero) || !Menu.Hotkeys.EnabledEvader)
            {
                return;
            }

            switch (args.Order)
            {
                case Order.AttackLocation:
                case Order.AttackTarget:
                case Order.Stop:
                case Order.Hold:
                case Order.MoveTarget:
                case Order.MoveLocation:
                    movePosition = args.TargetPosition;

                    if (sleeper.Sleeping("block"))
                    {
                        args.Process = false;
                    }

                    if (sleeper.Sleeping("avoiding"))
                    {
                        if (Menu.Settings.BlockPlayerMovement)
                        {
                            args.Process = false;
                            return;
                        }

                        var obstacles = Pathfinder.GetIntersectingObstacles(movePosition, Hero.HullRadius).ToList();
                        var ability =
                            abilityUpdater.EvadableAbilities.FirstOrDefault(
                                x => x.Obstacle != null && obstacles.Contains(x.Obstacle.Value));

                        if (ability == null)
                        {
                            return;
                        }

                        Debugger.WriteLine("avoiding OnExecute");

                        bool success;

                        var pathFromObstalce =
                            Pathfinder.CalculatePathFromObstacle(Hero.NetworkPosition, movePosition, 5, out success)
                                .ToList();

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
            Variables.Hero = ObjectManager.LocalHero;
            Variables.HeroTeam = Hero.Team;
            Variables.Menu = new MenuManager();
            Variables.Pathfinder = new Pathfinder();

            sleeper = new MultiSleeper();
            statusDrawer = new StatusDrawer();
            abilityUpdater = new AbilityUpdater();
            randomiser = new Random();
            fountain =
                ObjectManager.GetEntities<Unit>()
                    .First(x => x.ClassID == ClassID.CDOTA_Unit_Fountain && x.Team == HeroTeam);
        }

        public void OnModifierAdded(Unit sender, Modifier modifier)
        {
            if (!Menu.Hotkeys.EnabledEvader)
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
            if (AdditionalAbilityData.ModifierThinkers.TryGetValue(modifier.Name, out name))
            {
                var ability =
                    abilityUpdater.EvadableAbilities.FirstOrDefault(x => x.Name == name && x.Enabled) as
                        IModifierObstacle;
                ability?.AddModifierObstacle(modifier, sender);
                return;
            }

            var hero = sender as Hero;

            if (hero == null)
            {
                return;
            }

            var abilityName = modifier.AbilityName();
            if (string.IsNullOrEmpty(abilityName))
            {
                return;
            }

            var modifierAbility =
                abilityUpdater.EvadableAbilities.FirstOrDefault(
                    x => x.ModifierCounterEnabled && x.Name == abilityName && x.TimeSinceCast() < 0.5) as IModifier;

            modifierAbility?.AddModifer(modifier, hero);
        }

        public void OnModifierRemoved(Modifier modifier)
        {
            Debugger.WriteLine("====", Debugger.Type.Modifiers);
            Debugger.WriteLine("- modifier name: " + modifier.Name, Debugger.Type.Modifiers);
            Debugger.WriteLine("- modifier tname: " + modifier.TextureName, Debugger.Type.Modifiers);
            Debugger.WriteLine("- modifier owner: " + modifier.Owner.Name, Debugger.Type.Modifiers);

            var abilityName = modifier.AbilityName();
            if (string.IsNullOrEmpty(abilityName))
            {
                return;
            }

            var modifierAbility =
                abilityUpdater.EvadableAbilities.OfType<IModifier>()
                    .FirstOrDefault(x => x.ModifierHandle == modifier.Handle);
            modifierAbility?.RemoveModifier(modifier);
        }

        public void OnParticleEffectAdded(Entity sender, ParticleEffectAddedEventArgs args)
        {
            if (!Menu.Hotkeys.EnabledEvader)
            {
                return;
            }

            var particleName = args.Name;

            if (Menu.Debug.LogParticles)
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

            var abilityName = AdditionalAbilityData.Particles.FirstOrDefault(x => particleName.Contains(x.Key)).Value;

            if (string.IsNullOrEmpty(abilityName))
            {
                return;
            }

            var ability =
                abilityUpdater.EvadableAbilities.FirstOrDefault(x => x.Name == abilityName && x.Enabled) as IParticle;
            ability?.AddParticle(args.ParticleEffect);
        }

        public void OnUpdate()
        {
            if (!Game.IsInGame || Game.IsPaused || !Menu.Hotkeys.EnabledEvader)
            {
                return;
            }

            if (Menu.Debug.DrawMap && debugPath.Any() && !sleeper.Sleeping(debugPath))
            {
                debugPath.Clear();
            }

            if (Menu.Settings.PathfinderEffect && heroPathfinderEffect != null && !sleeper.Sleeping("avoiding"))
            {
                heroPathfinderEffect.Dispose();
                heroPathfinderEffect = null;
            }

            foreach (var ability in abilityUpdater.EvadableAbilities)
            {
                if (!ability.Enabled)
                {
                    continue;
                }

                ability.Check();

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
                    ability.End();
                    throw new Exception(
                        "Broken ability => " + ability.GetType().Name + " (" + ability.AbilityOwner.Name + " // "
                        + ability.Name + " // " + ability.GetRemainingTime() + ")");
                }
            }

            if (Menu.Settings.InvisIgnore && Hero.IsInvisible() && !Hero.IsVisibleToEnemies)
            {
                return;
            }

            var heroCanCast = Hero.CanCast();
            var heroCanUseItems = Hero.CanUseItems();

            var allies =
                ObjectManager.GetEntitiesParallel<Hero>()
                    .Where(
                        x =>
                            x.Equals(Hero)
                            || (Menu.AlliesSettings.HelpAllies && Menu.AlliesSettings.Enabled(x.StoredName())
                                && x.IsValid && x.Team == HeroTeam && x.IsAlive && !x.IsIllusion
                                && x.Distance2D(Hero) < 3000));

            foreach (var ability in abilityUpdater.EvadableAbilities)
            {
                if (!ability.Enabled)
                {
                    continue;
                }

                var modifierAbility = ability as IModifier;

                if (modifierAbility != null && !sleeper.Sleeping(ability.Handle) && modifierAbility.CanBeCountered())
                {
                    var enemyCounterList = ability.ModifierEnemyCounter;
                    if (Menu.Settings.ModifierEnemyCounter && enemyCounterList.Any())
                    {
                        var enemy = ability.AbilityOwner;
                        var modifierRemainingTime = modifierAbility.GetModiferRemainingTime();

                        var modifierEnemyCounterAbilities = from abilityName in enemyCounterList
                                                            join usableAbility in abilityUpdater.UsableAbilities on
                                                            abilityName equals usableAbility.Name
                                                            where
                                                            usableAbility.CanBeUsedOnEnemy
                                                            && Menu.UsableAbilities.Enabled(
                                                                abilityName,
                                                                usableAbility.Type)
                                                            && (usableAbility.IsItem && heroCanUseItems
                                                                || !usableAbility.IsItem && heroCanCast)
                                                            select usableAbility;

                        foreach (var modifierEnemyCounterAbility in modifierEnemyCounterAbilities)
                        {
                            if (!modifierEnemyCounterAbility.CanBeCasted(ability, enemy))
                            {
                                continue;
                            }

                            var requiredTime = modifierEnemyCounterAbility.GetRequiredTime(ability, enemy)
                                               + Game.Ping / 1000;

                            if (requiredTime <= modifierRemainingTime)
                            {
                                modifierEnemyCounterAbility.Use(ability, enemy);
                                sleeper.Sleep(modifierRemainingTime * 1000, ability.Handle);

                                Debugger.WriteLine(">>>>>>>>>>>>>>>");
                                Debugger.WriteLine(
                                    "modifier enemy counter: " + modifierEnemyCounterAbility.Name + " => "
                                    + ability.Name);
                                Debugger.WriteLine("remaining time: " + modifierRemainingTime);
                                Debugger.WriteLine("required time: " + requiredTime);
                                return;
                            }
                        }
                    }

                    var allyCounterList = ability.ModifierAllyCounter;
                    if (Menu.Settings.ModifierAllyCounter && allyCounterList.Any())
                    {
                        var ally = modifierAbility.GetModifierHero(allies);

                        if (ally != null)
                        {
                            var modifierRemainingTime = modifierAbility.GetModiferRemainingTime();
                            var allyIsMe = ally.Equals(Hero);

                            var modifierAllyCounterAbilities = from abilityName in allyCounterList
                                                               join usableAbility in abilityUpdater.UsableAbilities on
                                                               abilityName equals usableAbility.Name
                                                               where
                                                               Menu.UsableAbilities.Enabled(
                                                                   abilityName,
                                                                   usableAbility.Type)
                                                               && (usableAbility.IsItem && heroCanUseItems
                                                                   || !usableAbility.IsItem && heroCanCast)
                                                               select usableAbility;

                            foreach (var modifierCounterAbility in modifierAllyCounterAbilities)
                            {
                                if (!allyIsMe && !modifierCounterAbility.CanBeUsedOnAlly)
                                {
                                    continue;
                                }

                                if (!modifierCounterAbility.CanBeCasted(ability, ally))
                                {
                                    continue;
                                }

                                var requiredTime = modifierCounterAbility.GetRequiredTime(ability, ally)
                                                   + Game.Ping / 1000;

                                if (requiredTime <= modifierRemainingTime)
                                {
                                    modifierCounterAbility.Use(ability, ally);
                                    sleeper.Sleep(modifierRemainingTime * 1000, ability.Handle);

                                    Debugger.WriteLine(">>>>>>>>>>>>>>>");
                                    Debugger.WriteLine(
                                        "modifier counter: " + modifierCounterAbility.Name + " => " + ability.Name);
                                    Debugger.WriteLine("ally: " + ally.GetName());
                                    Debugger.WriteLine("remaining time: " + modifierRemainingTime);
                                    Debugger.WriteLine("required time: " + requiredTime);
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            if (!Hero.IsAlive)
            {
                return;
            }

            if (Menu.Hotkeys.ForceBlink)
            {
                var blinkDagger =
                    abilityUpdater.UsableAbilities.FirstOrDefault(x => x.ClassID == ClassID.CDOTA_Item_BlinkDagger) as
                        BlinkDagger;

                if (blinkDagger != null && blinkDagger.CanBeCasted(null, null) && heroCanUseItems)
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
                        abilityUpdater.EvadableAbilities.OfType<Projectile>()
                            .Where(
                                x =>
                                    (int)x.GetProjectileSpeed() == projectile.Speed
                                    && (x.TimeSinceCast() < 1.5 || !x.AbilityOwner.IsVisible))
                            .ToList();
                    if (predictedAbilities.Count == 1)
                    {
                        Debugger.WriteLine(
                            "predicted: " + predictedAbilities.First().Name + " => "
                            + ((Hero)projectile.Target).GetName(),
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
                    abilityUpdater.EvadableAbilities.OfType<Projectile>()
                        .FirstOrDefault(
                            x =>
                                x.OwnerClassID == source.ClassID && (int)x.GetProjectileSpeed() == projectile.Speed
                                && x.TimeSinceCast() < 1.5);

                if (ability != null)
                {
                    Debugger.WriteLine(
                        "projectile: " + ability.Name + " => " + ((Hero)projectile.Target).GetName(),
                        Debugger.Type.Projectiles);
                    ability.SetProjectile(projectile.Position, (Hero)projectile.Target);
                }
            }

            var allyIntersections = allies.ToDictionary(x => x, x => Pathfinder.GetIntersectingObstacles(x));

            if (Menu.AlliesSettings.HelpAllies && Menu.AlliesSettings.MultiIntersectionEnemyDisable)
            {
                var multiIntersection =
                    allyIntersections.SelectMany(x => x.Value).GroupBy(x => x).SelectMany(x => x.Skip(1));

                foreach (var obstacle in multiIntersection)
                {
                    var ability =
                        abilityUpdater.EvadableAbilities.FirstOrDefault(x => x.Obstacle == obstacle && x.IsDisable) as
                            AOE;

                    if (ability != null)
                    {
                        if (sleeper.Sleeping(ability))
                        {
                            continue;
                        }

                        if (Menu.Debug.LogIntersection)
                        {
                            foreach (
                                var hero in allyIntersections.Where(x => x.Value.Contains(obstacle)).Select(x => x.Key))
                            {
                                Debugger.Write(hero.GetName() + " ", Debugger.Type.Intersectons);
                            }
                            Debugger.WriteLine("intersecting: " + ability.Name, Debugger.Type.Intersectons);
                        }

                        var abilityOwner = ability.AbilityOwner;
                        var disableAbilities = from abilityName in ability.DisableAbilities
                                               join usableAbility in abilityUpdater.UsableAbilities on abilityName
                                               equals usableAbility.Name
                                               where
                                               usableAbility.Type == AbilityType.Disable
                                               && Menu.UsableAbilities.Enabled(abilityName, AbilityType.Disable)
                                               && (usableAbility.IsItem && heroCanUseItems
                                                   || !usableAbility.IsItem && heroCanCast)
                                               select usableAbility;

                        foreach (var disableAbility in disableAbilities)
                        {
                            if (!disableAbility.CanBeCasted(ability, abilityOwner))
                            {
                                continue;
                            }

                            var requiredTime = disableAbility.GetRequiredTime(ability, abilityOwner) + Game.Ping / 1000;

                            var remainingDisableTime = ability.GetRemainingDisableTime();

                            var ignoreRemainingTime = ability.IgnoreRemainingTime(disableAbility, remainingDisableTime);

                            if (requiredTime > remainingDisableTime && !ignoreRemainingTime)
                            {
                                continue;
                            }

                            if (remainingDisableTime - requiredTime <= 0.10 || ignoreRemainingTime)
                            {
                                disableAbility.Use(ability, abilityOwner);
                                sleeper.Sleep(ability.GetSleepTime(), ability);

                                Debugger.WriteLine(">>>>>>>>>>>>>>>");
                                Debugger.WriteLine(
                                    "multi intersection disable: " + disableAbility.Name + " => " + ability.Name);
                                Debugger.Write("allies: ");
                                foreach (var hero in
                                    allyIntersections.Where(x => x.Value.Contains(obstacle)).Select(x => x.Key))
                                {
                                    Debugger.Write(hero.GetName() + " ");
                                }
                                Debugger.WriteLine();
                                Debugger.WriteLine("remaining time: " + remainingDisableTime);
                                Debugger.WriteLine("required time: " + requiredTime);
                            }
                            return;
                        }
                    }
                }
            }

            foreach (var intersection in allyIntersections.OrderByDescending(x => x.Key.Equals(Hero)))
            {
                var ally = intersection.Key;
                var allyLinkens = ally.IsLinkensProtected();
                var allyMagicImmune = ally.IsMagicImmune();
                var allyIsMe = ally.Equals(Hero);
                var allyHp = ally.Health;
                //var heroMp = Hero.Mana;

                foreach (var ability in
                    intersection.Value.Select(
                            x =>
                                abilityUpdater.EvadableAbilities.FirstOrDefault(
                                    z => z.Obstacle == x && (z.AllyHpIgnore <= 0 || allyHp < z.AllyHpIgnore)
                                         //&& (z.HeroMpIgnore <= 0 || heroMp > z.HeroMpIgnore)
                                         && (z.AbilityLevelIgnore <= 0 || z.Level > z.AbilityLevelIgnore)))
                        .Where(x => x != null)
                        .OrderByDescending(x => x.IsDisable))
                {
                    if (ability != null)
                    {
                        if (sleeper.Sleeping(ability))
                        {
                            continue;
                        }

                        Debugger.WriteLine(
                            ally.GetName() + " intersecting: " + ability.Name,
                            Debugger.Type.Intersectons);

                        if ((allyLinkens && (ability is Projectile || ability is LinearTarget))
                            || (!ability.PiercesMagicImmunity && allyMagicImmune))
                        {
                            continue;
                        }

                        var abilityOwner = ability.AbilityOwner;
                        var remainingTime = ability.GetRemainingTime(ally);

                        foreach (var priority in
                            ability.UseCustomPriority ? ability.Priority : Menu.Settings.DefaultPriority)
                        {
                            switch (priority)
                            {
                                case Priority.Walk:
                                    if (allyIsMe && !ability.IgnorePathfinder && Menu.Hotkeys.EnabledPathfinder
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
                                                        Hero.NetworkPosition,
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

                                                if (Menu.Settings.PathfinderEffect)
                                                {
                                                    heroPathfinderEffect =
                                                        new ParticleEffect(
                                                            @"particles/units/heroes/hero_oracle/oracle_fortune_purge.vpcf",
                                                            Hero);
                                                }

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

                                                var sleepTime = Math.Min(time, 1) * 1000;

                                                Utils.Sleep(sleepTime, "Evader.Avoiding");
                                                sleeper.Sleep(sleepTime, "avoiding");
                                                sleeper.Sleep(sleepTime, ability);
                                                sleeper.Sleep(1000, debugPath);

                                                if (Menu.Settings.PathfinderEffect)
                                                {
                                                    heroPathfinderEffect =
                                                        new ParticleEffect(
                                                            @"particles/units/heroes/hero_oracle/oracle_fortune_purge.vpcf",
                                                            Hero);
                                                }

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
                                                         join usableAbility in abilityUpdater.UsableAbilities on
                                                         abilityName equals usableAbility.Name
                                                         where
                                                         usableAbility.Type == AbilityType.Blink
                                                         && Menu.UsableAbilities.Enabled(abilityName, AbilityType.Blink)
                                                         && (usableAbility.IsItem && heroCanUseItems
                                                             || !usableAbility.IsItem && heroCanCast)
                                                         select usableAbility;

                                    foreach (var blinkAbility in blinkAbilities)
                                    {
                                        if (!blinkAbility.CanBeCasted(ability, fountain))
                                        {
                                            continue;
                                        }

                                        var requiredTime = blinkAbility.GetRequiredTime(ability, fountain)
                                                           + Game.Ping / 1000 + 0.05f;

                                        var time = remainingTime - requiredTime;

                                        var ignoreRemainingTime = ability.IgnoreRemainingTime(blinkAbility, time);

                                        if (requiredTime > remainingTime && !ignoreRemainingTime)
                                        {
                                            continue;
                                        }

                                        if (time <= 0.10 && time > 0 || ignoreRemainingTime)
                                        {
                                            Debugger.WriteLine(">>>>>>>>>>>>>>>");

                                            if (!Menu.Randomiser.Enabled
                                                || (Menu.Randomiser.NukesOnly && ability.IsDisable)
                                                || (randomiser.Next(99) > Menu.Randomiser.FailChance))
                                            {
                                                blinkAbility.Use(ability, fountain);
                                            }
                                            else
                                            {
                                                Debugger.WriteLine("you got rekt by randomiser!");
                                            }

                                            sleeper.Sleep(Math.Min(ability.GetSleepTime(), 1000), ability);

                                            Debugger.WriteLine("blink: " + blinkAbility.Name + " => " + ability.Name);
                                            Debugger.WriteLine("ally: " + ally.GetName());
                                            Debugger.WriteLine("remaining time: " + remainingTime);
                                            Debugger.WriteLine("required time: " + requiredTime);
                                        }

                                        return;
                                    }

                                    break;
                                case Priority.Counter:
                                    var counterAbilities = from abilityName in ability.CounterAbilities
                                                           join usableAbility in abilityUpdater.UsableAbilities on
                                                           abilityName equals usableAbility.Name
                                                           where
                                                           usableAbility.Type == AbilityType.Counter
                                                           && Menu.UsableAbilities.Enabled(
                                                               abilityName,
                                                               AbilityType.Counter)
                                                           && (usableAbility.IsItem && heroCanUseItems
                                                               || !usableAbility.IsItem && heroCanCast)
                                                           select usableAbility;

                                    foreach (var counterAbility in counterAbilities)
                                    {
                                        var targetEnemy = !counterAbility.CanBeUsedOnAlly
                                                          && counterAbility.CanBeUsedOnEnemy;

                                        if (!counterAbility.CanBeUsedOnAlly && !allyIsMe && !targetEnemy
                                            || !targetEnemy && !counterAbility.CanBeCasted(ability, ally)
                                            || targetEnemy && !counterAbility.CanBeCasted(ability, abilityOwner))
                                        {
                                            continue;
                                        }

                                        var requiredTime = counterAbility.GetRequiredTime(
                                                               ability,
                                                               targetEnemy ? abilityOwner : ally) + Game.Ping / 1000
                                                           + 0.05f;

                                        var ignoreRemainingTime = false;

                                        var time = remainingTime - requiredTime;

                                        if (counterAbility.Name == AbilityNames.SleightOfFist)
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
                                            Debugger.WriteLine(">>>>>>>>>>>>>>>");

                                            if (!Menu.Randomiser.Enabled
                                                || (Menu.Randomiser.NukesOnly && ability.IsDisable)
                                                || (randomiser.Next(99) > Menu.Randomiser.FailChance))
                                            {
                                                counterAbility.Use(ability, targetEnemy ? abilityOwner : ally);
                                            }
                                            else
                                            {
                                                Debugger.WriteLine("you got rekt by randomiser!");
                                            }

                                            float customSleepTime;
                                            sleeper.Sleep(
                                                counterAbility.UseCustomSleep(out customSleepTime)
                                                    ? customSleepTime
                                                    : ability.GetSleepTime(),
                                                ability);

                                            sleeper.Sleep(200, ability.Handle); // for modifier

                                            Debugger.WriteLine(
                                                "counter: " + counterAbility.Name + " => " + ability.Name);
                                            Debugger.WriteLine("ally: " + ally.GetName());
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
                                                           join usableAbility in abilityUpdater.UsableAbilities on
                                                           abilityName equals usableAbility.Name
                                                           where
                                                           usableAbility.Type == AbilityType.Disable
                                                           && Menu.UsableAbilities.Enabled(
                                                               abilityName,
                                                               AbilityType.Disable)
                                                           && (usableAbility.IsItem && heroCanUseItems
                                                               || !usableAbility.IsItem && heroCanCast)
                                                           select usableAbility;

                                    foreach (var disableAbility in disableAbilities)
                                    {
                                        if (!disableAbility.CanBeCasted(ability, abilityOwner))
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
                                            Debugger.WriteLine(">>>>>>>>>>>>>>>");

                                            if (!Menu.Randomiser.Enabled
                                                || (Menu.Randomiser.NukesOnly && ability.IsDisable)
                                                || (randomiser.Next(99) > Menu.Randomiser.FailChance))
                                            {
                                                disableAbility.Use(ability, abilityOwner);
                                            }
                                            else
                                            {
                                                Debugger.WriteLine("you got rekt by randomiser!");
                                            }

                                            sleeper.Sleep(ability.GetSleepTime(), ability);

                                            Debugger.WriteLine(
                                                "disable: " + disableAbility.Name + " => " + ability.Name);
                                            Debugger.WriteLine("ally: " + ally.GetName());
                                            Debugger.WriteLine("remaining time: " + remainingDisableTime);
                                            Debugger.WriteLine("required time: " + requiredTime);
                                        }
                                        return;
                                    }

                                    break;
                            }
                        }

                        if (allyIsMe && Menu.UsableAbilities.Enabled(AbilityNames.GoldSpender, AbilityType.Counter)
                            && abilityUpdater.GoldSpender.CanBeCasted(ability, ally))
                        {
                            abilityUpdater.GoldSpender.Use(ability, ally);
                        }
                    }
                }
            }
        }

        #endregion
    }
}