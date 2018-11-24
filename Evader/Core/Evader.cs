namespace Evader.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Common;

    using Data;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;
    using Ensage.Common.Objects.UtilityObjects;

    using EvadableAbilities.Base;
    using EvadableAbilities.Base.Interfaces;

    using Menus;

    using SharpDX;

    using UsableAbilities.Items;

    using AbilityType = Data.AbilityType;
    using Projectile = EvadableAbilities.Base.Projectile;
    using Utils = Ensage.Common.Utils;

    internal class Evader
    {
        private AbilityModifiers abilityModifiers;

        private AbilityUpdater abilityUpdater;

        private Unit fountain;

        private ParticleEffect heroPathfinderEffect;

        private Vector3 movePathfinderPosition;

        private Vector3 movePosition;

        private Random randomiser;

        private StatusDrawer statusDrawer;

        private static Hero Hero => Variables.Hero;

        private static Team HeroTeam => Variables.HeroTeam;

        private static MenuManager Menu => Variables.Menu;

        private static Pathfinder Pathfinder => Variables.Pathfinder;

        private MultiSleeper sleeper => Variables.Sleeper;

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

            if (unit.NetworkName != "CDOTA_BaseNPC" || unit.AttackCapability != AttackCapability.None
                || unit.Team == HeroTeam)
            {
                return;
            }

            var dayVision = unit.DayVision;

            if (Menu.Debug.LogInformation && dayVision > 0)
            {
                Debugger.Write("  = >   Unit (" + unit.Name + ") with vision: " + dayVision + " // ");
                var closestHero = ObjectManager.GetEntities<Hero>()
                    .Where(x => x.IsValid && x.IsAlive)
                    .OrderBy(x => x.Distance2D(unit))
                    .FirstOrDefault();
                Debugger.WriteLine(closestHero.GetName() + " (" + closestHero.Distance2D(unit) + ")", showType: false);
            }

            var abilityNames = AdditionalAbilityData.Vision.Where(x => x.Value == dayVision)
                .Select(x => x.Key)
                .ToList();

            if (!abilityNames.Any())
            {
                if (Menu.Debug.LogInformation && dayVision > 0)
                {
                    Debugger.WriteLine("  = >   Ability name not found");
                }
                return;
            }

            var abilities = abilityUpdater.EvadableAbilities.Where(
                    x => abilityNames.Contains(x.Name) && (!x.AbilityOwner.IsVisible || x.TimeSinceCast() < 1))
                .ToList();

            if (abilities.Count != 1)
            {
                if (abilities.Count < 1)
                {
                    return;
                }

                Debugger.Write("=> Same vision for: ");
                foreach (var ability in abilities)
                {
                    Debugger.Write(ability.Name + " ", showType: false);
                }
                Debugger.WriteLine(showType: false);

                return;
            }

            var unitAbility = abilities.FirstOrDefault() as IUnit;
            if (unitAbility == null)
            {
                return;
            }

            Debugger.WriteLine("  = >   Ability name: " + unitAbility.Name);
            unitAbility.AddUnit(unit);
        }

        public void OnClose()
        {
            foreach (var ability in abilityUpdater.EvadableAbilities.Where(x => Game.RawGameTime > x.CreateTime + 300))
            {
                var modifierCounter = ability as IModifier;
                if (modifierCounter != null && modifierCounter.Modifier.AddedTime <= 0)
                {
                    Debugger.WriteLine(
                        " => Probably incorrect modifier counter for: " + ability.Name + " ("
                        + ability.AbilityOwner.GetName() + ")");
                }
            }

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
                MapDrawer.Draw();
            }
        }

        public void OnExecuteAction(Player sender, ExecuteOrderEventArgs args)
        {
            if (!args.Entities.Contains(Hero) || !Menu.Hotkeys.EnabledEvader)
            {
                return;
            }

            if (!Menu.Settings.BlockPlayerInput && args.IsPlayerInput
                || !Menu.Settings.BlockAssemblyInput && !args.IsPlayerInput)
            {
                return;
            }

            switch (args.OrderId)
            {
                case OrderId.AttackLocation:
                case OrderId.AttackTarget:
                case OrderId.Stop:
                case OrderId.Hold:
                case OrderId.MoveTarget:
                case OrderId.MoveLocation:
                case OrderId.ConsumeRune:
                case OrderId.Continue:
                case OrderId.Patrol:
                    movePosition = args.Target?.Position ?? args.TargetPosition;
                    if (sleeper.Sleeping("block") || sleeper.Sleeping("avoiding"))
                    {
                        args.Process = false;
                    }
                    break;
                case OrderId.Ability:
                case OrderId.AbilityTarget:
                case OrderId.AbilityLocation:
                case OrderId.AbilityTargetTree:
                case OrderId.AbilityTargetRune:
                    movePosition = args.TargetPosition;
                    if ((sleeper.Sleeping("block") || sleeper.Sleeping("avoiding")) && Menu.Settings.BlockAbilityUsage)
                    {
                        args.Process = false;
                    }
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
            Variables.EnemyTeam = Hero.GetEnemyTeam();
            Variables.Menu = new MenuManager();
            Variables.Pathfinder = new Pathfinder();

            Variables.Sleeper = new MultiSleeper();
            statusDrawer = new StatusDrawer();
            abilityUpdater = new AbilityUpdater();
            randomiser = new Random();
            abilityModifiers = new AbilityModifiers();

            fountain = ObjectManager.GetEntities<Unit>()
                .First(x => x.NetworkName == "CDOTA_Unit_Fountain" && x.Team == HeroTeam);
        }

        public void OnModifierAdded(Unit sender, Modifier modifier)
        {
            if (!Menu.Hotkeys.EnabledEvader)
            {
                return;
            }

            if (sender.Handle == Hero.Handle && modifier.IsStunDebuff)
            {
                Hero.Stop();
            }

            DelayAction.Add(
                1,
                () =>
                    {
                        if (!modifier.IsValid)
                        {
                            return;
                        }

                        if (sender.Team != HeroTeam || modifier.Name == "modifier_faceless_void_chronosphere")
                        {
                            string name;
                            if (AdditionalAbilityData.Modifiers.TryGetValue(modifier.Name, out name))
                            {
                                foreach (var ability in abilityUpdater.EvadableAbilities
                                    .Where(x => x.Name == name && x.Enabled)
                                    .OfType<IModifierObstacle>())
                                {
                                    ability.AddModifierObstacle(modifier, sender);
                                }

                                return;
                            }
                        }

                        var hero = sender as Hero;
                        if (hero == null)
                        {
                            return;
                        }

                        var abilityName = abilityModifiers.GetAbilityName(modifier);
                        if (string.IsNullOrEmpty(abilityName))
                        {
                            return;
                        }

                        var modifierAbility = abilityUpdater.EvadableAbilities.FirstOrDefault(
                                                  x => x.ModifierCounterEnabled && x.Name == abilityName
                                                       && (x.TimeSinceCast() - x.AdditionalDelay
                                                           < modifier.ElapsedTime + 1.5f || !x.AbilityOwner.IsVisible
                                                           || x.DisableTimeSinceCastCheck)) as IModifier;

                        modifierAbility?.Modifier.Add(modifier, hero);
                    });
        }

        public void OnModifierRemoved(Modifier modifier)
        {
            var abilityName = abilityModifiers.GetAbilityName(modifier);
            if (string.IsNullOrEmpty(abilityName))
            {
                return;
            }

            var modifierAbility = abilityUpdater.EvadableAbilities.OfType<IModifier>()
                .FirstOrDefault(x => x.Modifier.Handle == modifier.Handle);

            modifierAbility?.Modifier.Remove();
        }

        public void OnParticleEffectAdded(Entity sender, ParticleEffectAddedEventArgs args)
        {
            if (!Menu.Hotkeys.EnabledEvader)
            {
                return;
            }

            var particleName = args.Name;

            var abilityName = AdditionalAbilityData.Particles.FirstOrDefault(x => particleName.Contains(x.Key)).Value;

            if (string.IsNullOrEmpty(abilityName))
            {
                return;
            }

            var ability =
                abilityUpdater.EvadableAbilities.FirstOrDefault(x => x.Name == abilityName && x.Enabled) as IParticle;
            if (ability != null)
            {
                var rubick = ObjectManager.GetEntities<Hero>()
                    .FirstOrDefault(
                        x => x.IsValid && !x.IsIllusion && x.HeroId == HeroId.npc_dota_hero_rubick
                             && x.Team == HeroTeam);

                if (rubick == null
                    || rubick.Spellbook.Spells.FirstOrDefault(x => x.Name == abilityName).TimeSinceCasted() >= 0.5)
                {
                    ability.AddParticle(args);
                }
            }
        }

        public void OnUpdate()
        {
            if (!Game.IsInGame || Game.IsPaused || !Menu.Hotkeys.EnabledEvader)
            {
                return;
            }

            if (Menu.Settings.PathfinderEffect && heroPathfinderEffect != null && !sleeper.Sleeping("avoiding"))
            {
                heroPathfinderEffect.Dispose();
                heroPathfinderEffect = null;
            }

            foreach (var ability in abilityUpdater.EvadableAbilities.ToList())
            {
                if (!ability.Enabled)
                {
                    continue;
                }

                try
                {
                    ability.Check();
                }
                catch
                {
                    abilityUpdater.EvadableAbilities.Remove(ability);
                    throw;
                }

                if (ability.IsStopped())
                {
                    ability.End();

                    if (sleeper.Sleeping(ability))
                    {
                        sleeper.Reset(ability);
                        sleeper.Reset("avoiding");

                        if (!Hero.IsChanneling() && !Hero.IsInvul()
                            && Hero.Spellbook.Spells.Any(x => x.IsInAbilityPhase))
                        {
                            Debugger.WriteLine("Hero stop", Debugger.Type.AbilityUsage);
                            Hero.Stop();
                        }
                    }
                }

                var time = ability.GetRemainingTime();
                if (ability.Obstacle != null && (time > 60 || time < -20))
                {
                    var ex = new BrokenAbilityException(ability.GetType().Name);

                    try
                    {
                        ex.Data["Ability"] = new
                        {
                            Ability = ability.Name,
                            Owner = ability.AbilityOwner.Name,
                            RemainingTime = time,
                            ObstacleTime = Game.RawGameTime - ability.StartCast,
                            StartCast = ability.StartCast,
                            EndCast = ability.EndCast
                        };
                    }
                    catch
                    {
                        ex.Data["Ability"] = "Failed to get data";
                    }

                    ability.End();
                    throw ex;
                }
            }

            if (Menu.Settings.InvisIgnore && Hero.IsInvisible() && !Hero.IsVisibleToEnemies)
            {
                return;
            }

            var allies = ObjectManager.GetEntities<Hero>()
                .Where(
                    x => x.Equals(Hero) || Menu.AlliesSettings.HelpAllies && Menu.AlliesSettings.Enabled(x.StoredName())
                         && x.IsValid && x.Team == HeroTeam && x.IsAlive && !x.IsIllusion && x.Distance2D(Hero) < 3000)
                .ToList();

            foreach (var ability in abilityUpdater.EvadableAbilities)
            {
                if (!ability.Enabled)
                {
                    continue;
                }

                if (ability is NoObstacleAbility)
                {
                    Evade(
                        Hero,
                        new List<uint>
                        {
                            ability.Handle
                        });
                }

                var modifierAbility = ability as IModifier;
                if (modifierAbility == null || sleeper.Sleeping(ability.Handle)
                    || !modifierAbility.Modifier.CanBeCountered())
                {
                    continue;
                }

                var enemyCounterList = modifierAbility.Modifier.EnemyCounterAbilities;
                if (Menu.Settings.ModifierEnemyCounter && enemyCounterList.Any())
                {
                    var enemy = modifierAbility.Modifier.GetEnemyHero();
                    if (enemy == null)
                    {
                        continue;
                    }

                    var modifierRemainingTime = modifierAbility.Modifier.GetRemainingTime();

                    var modifierEnemyCounterAbilities = from abilityName in enemyCounterList
                                                        join usableAbility in abilityUpdater.UsableAbilities on
                                                            abilityName equals usableAbility.Name
                                                        where usableAbility.CanBeUsedOnEnemy
                                                              && Menu.UsableAbilities.Enabled(
                                                                  abilityName,
                                                                  usableAbility.Type)
                                                        select usableAbility;

                    foreach (var modifierEnemyCounterAbility in modifierEnemyCounterAbilities)
                    {
                        if (!modifierEnemyCounterAbility.CanBeCasted(ability, enemy))
                        {
                            continue;
                        }

                        var requiredTime =
                            modifierEnemyCounterAbility.GetRequiredTime(ability, enemy, modifierRemainingTime)
                            + Game.Ping / 1000;
                        if (requiredTime < 0.35 && modifierAbility.Modifier.GetElapsedTime() < 0.35)
                        {
                            return;
                        }

                        if (requiredTime <= modifierRemainingTime || modifierAbility.Modifier.IgnoreRemainingTime)
                        {
                            modifierEnemyCounterAbility.Use(ability, enemy);
                            sleeper.Sleep(3000, ability.Handle);

                            Debugger.WriteLine(
                                TimeSpan.FromSeconds(Game.GameTime).ToString(@"mm\:ss") + " >>>>>>>>>>>>>>>",
                                Debugger.Type.AbilityUsage);
                            Debugger.WriteLine(
                                "modifier enemy counter: " + modifierEnemyCounterAbility.Name + " => " + ability.Name,
                                Debugger.Type.AbilityUsage);
                            Debugger.WriteLine("remaining time: " + modifierRemainingTime, Debugger.Type.AbilityUsage);
                            Debugger.WriteLine("required time: " + requiredTime, Debugger.Type.AbilityUsage);
                            return;
                        }
                    }
                }

                var allyCounterList = modifierAbility.Modifier.AllyCounterAbilities;
                if (Menu.Settings.ModifierAllyCounter && allyCounterList.Any())
                {
                    var ally = modifierAbility.Modifier.GetAllyHero(allies);
                    if (ally == null)
                    {
                        continue;
                    }

                    var modifierRemainingTime = modifierAbility.Modifier.GetRemainingTime();
                    var allyIsMe = ally.Equals(Hero);

                    var modifierAllyCounterAbilities = from abilityName in allyCounterList
                                                       join usableAbility in abilityUpdater.UsableAbilities on
                                                           abilityName equals usableAbility.Name
                                                       where Menu.UsableAbilities.Enabled(
                                                           abilityName,
                                                           usableAbility.Type)
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

                        var requiredTime = modifierCounterAbility.GetRequiredTime(ability, ally, modifierRemainingTime)
                                           + Game.Ping / 1000;
                        if (requiredTime < 0.3 && modifierAbility.Modifier.GetElapsedTime() < 0.3)
                        {
                            return;
                        }

                        if (requiredTime <= modifierRemainingTime || modifierAbility.Modifier.IgnoreRemainingTime)
                        {
                            modifierCounterAbility.Use(ability, ally);
                            sleeper.Sleep(1000, ability.Handle);

                            Debugger.WriteLine(
                                TimeSpan.FromSeconds(Game.GameTime).ToString(@"mm\:ss") + " >>>>>>>>>>>>>>>",
                                Debugger.Type.AbilityUsage);
                            Debugger.WriteLine(
                                "modifier counter: " + modifierCounterAbility.Name + " => " + ability.Name,
                                Debugger.Type.AbilityUsage);
                            Debugger.WriteLine("ally: " + ally.GetName(), Debugger.Type.AbilityUsage);
                            Debugger.WriteLine("remaining time: " + modifierRemainingTime, Debugger.Type.AbilityUsage);
                            Debugger.WriteLine("required time: " + requiredTime, Debugger.Type.AbilityUsage);
                            return;
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
                    abilityUpdater.UsableAbilities.FirstOrDefault(x => x.AbilityId == AbilityId.item_blink) as
                        BlinkDagger;

                if (blinkDagger != null && blinkDagger.CanBeCasted(null, null))
                {
                    blinkDagger.UseInFront();
                }
            }

            foreach (var projectile in ObjectManager.TrackingProjectiles.Where(x => x?.Target is Hero))
            {
                var source = projectile.Source;
                if (source == null)
                {
                    var predictedAbilities = abilityUpdater.EvadableAbilities.OfType<Projectile>()
                        .Where(
                            x => (int)x.GetProjectileSpeed() == projectile.Speed
                                 && (x.TimeSinceCast() < 1.5 + x.AdditionalDelay || !x.AbilityOwner.IsVisible))
                        .ToList();
                    if (predictedAbilities.Count == 1)
                    {
                        Debugger.WriteLine(
                            "predicted: " + predictedAbilities.First().Name + " => "
                            + ((Hero)projectile.Target).GetName());
                        predictedAbilities.First().SetProjectile(projectile.Position, (Hero)projectile.Target);
                    }
                    continue;
                }

                if (source != null && !source.IsValid && projectile.Speed == 400)
                {
                    var sparkWraith = abilityUpdater.EvadableAbilities.OfType<Projectile>()
                        .FirstOrDefault(x => x.Name == "arc_warden_spark_wraith");

                    if (sparkWraith != null)
                    {
                        sparkWraith.SetProjectile(projectile.Position, (Hero)projectile.Target);
                    }
                    else
                    {
                        Debugger.WriteLine(
                            "=> Probably another projectile with same speed and not valid source // Spark Wraith");
                    }

                    continue;
                }

                if (!source.IsValid || source.Team == HeroTeam)
                {
                    continue;
                }

                var ability = abilityUpdater.EvadableAbilities.OfType<Projectile>()
                    .FirstOrDefault(
                        x => x.OwnerName == source.Name && (int)x.GetProjectileSpeed() == projectile.Speed
                             && x.TimeSinceCast() < 1.5 + x.AdditionalDelay);

                ability?.SetProjectile(projectile.Position, (Hero)projectile.Target);
            }

            var allyIntersections = allies.ToDictionary(x => x, x => Pathfinder.GetIntersectingObstacles(x));

            //if (Menu.AlliesSettings.HelpAllies && Menu.AlliesSettings.MultiIntersectionEnemyDisable)
            //{
            //    var multiIntersection = allyIntersections.SelectMany(x => x.Value)
            //        .GroupBy(x => x)
            //        .SelectMany(x => x.Skip(1));

            //    foreach (var obstacle in multiIntersection)
            //    {
            //        var ability =
            //            abilityUpdater.EvadableAbilities.FirstOrDefault(x => x.Obstacle == obstacle && x.IsDisable) as
            //                AOE;

            //        if (ability == null || sleeper.Sleeping(ability))
            //        {
            //            continue;
            //        }

            //        if (Menu.Debug.LogIntersection)
            //        {
            //            Debugger.WriteLine("", Debugger.Type.Intersectons, false);
            //            foreach (var hero in allyIntersections.Where(x => x.Value.Contains(obstacle))
            //                .Select(x => x.Key))
            //            {
            //                Debugger.Write(hero.GetName() + " ", Debugger.Type.Intersectons, false);
            //            }
            //            Debugger.WriteLine("intersecting: " + ability.Name, Debugger.Type.Intersectons, true, false);
            //        }

            //        var abilityOwner = ability.AbilityOwner;
            //        var disableAbilities = from abilityName in ability.DisableAbilities
            //                               join usableAbility in abilityUpdater.UsableAbilities on abilityName equals
            //                                   usableAbility.Name
            //                               where usableAbility.Type == AbilityType.Disable
            //                                     && Menu.UsableAbilities.Enabled(abilityName, AbilityType.Disable)
            //                               select usableAbility;

            //        foreach (var disableAbility in disableAbilities)
            //        {
            //            if (!disableAbility.CanBeCasted(ability, abilityOwner))
            //            {
            //                continue;
            //            }

            //            var remainingDisableTime = ability.GetRemainingDisableTime();
            //            var requiredTime = disableAbility.GetRequiredTime(ability, abilityOwner, remainingDisableTime)
            //                               + Game.Ping / 1000;
            //            var ignoreRemainingTime = ability.IgnoreRemainingTime(disableAbility, remainingDisableTime);

            //            if (requiredTime > remainingDisableTime && !ignoreRemainingTime)
            //            {
            //                continue;
            //            }

            //            if (remainingDisableTime - requiredTime <= 0.10 || ignoreRemainingTime)
            //            {
            //                disableAbility.Use(ability, abilityOwner);
            //                sleeper.Sleep(ability.GetSleepTime(), ability);

            //                Debugger.WriteLine(
            //                    TimeSpan.FromSeconds(Game.GameTime).ToString(@"mm\:ss") + " >>>>>>>>>>>>>>>",
            //                    Debugger.Type.AbilityUsage);
            //                Debugger.WriteLine(
            //                    "multi intersection disable: " + disableAbility.Name + " => " + ability.Name,
            //                    Debugger.Type.AbilityUsage);
            //                Debugger.Write("allies: ", Debugger.Type.AbilityUsage);
            //                foreach (var hero in allyIntersections.Where(x => x.Value.Contains(obstacle))
            //                    .Select(x => x.Key))
            //                {
            //                    Debugger.Write(hero.GetName() + " ", Debugger.Type.AbilityUsage, false);
            //                }
            //                Debugger.WriteLine("", Debugger.Type.AbilityUsage, showType: false);
            //                Debugger.WriteLine("remaining time: " + remainingDisableTime, Debugger.Type.AbilityUsage);
            //                Debugger.WriteLine("required time: " + requiredTime, Debugger.Type.AbilityUsage);
            //            }
            //            return;
            //        }
            //    }
            //}

            foreach (var intersection in allyIntersections.OrderByDescending(x => x.Key.Equals(Hero)))
            {
                if (!Evade(intersection.Key, intersection.Value))
                {
                    return;
                }
            }
        }

        private bool Evade(Hero ally, IEnumerable<uint> intersection)
        {
            if (ally.IsInvul())
            {
                return true;
            }

            var heroCanCast = Hero.CanCast();
            var heroCanUseItems = Hero.CanUseItems();
            var allyLinkens = ally.IsLinkensProtected();
            var allyMagicImmune = ally.IsMagicImmune();
            var allyIsMe = ally.Equals(Hero);
            var allyHp = ally.Health;
            var heroMp = Hero.Mana;

            foreach (var ability in intersection.Select(
                    x => abilityUpdater.EvadableAbilities.FirstOrDefault(
                        z => z.Obstacle == x && (z.AllyHpIgnore <= 0 || allyHp < z.AllyHpIgnore)
                             && (z.HeroMpIgnore <= 0 || heroMp > z.HeroMpIgnore)
                             && (z.AbilityLevelIgnore <= 0 || z.Level > z.AbilityLevelIgnore)
                             && (z.AbilityTimeIgnore <= 0 || Game.GameTime / 60 < z.AbilityTimeIgnore)))
                .Where(x => x != null)
                .OrderByDescending(x => x.IsDisable))
            {
                if (ability == null || sleeper.Sleeping(ability)
                    || ability.AbilityOwner.Distance2D(ally) > ability.Ability.GetCastRange() + 300)
                {
                    continue;
                }

                if (allyLinkens && (ability is Projectile || ability is LinearTarget)
                    || !ability.PiercesMagicImmunity && allyMagicImmune)
                {
                    continue;
                }

                if (Hero.IsChanneling() && (!Menu.Settings.CancelAnimation || !ability.IsDisable))
                {
                    continue;
                }

                Debugger.WriteLine(
                    ally.GetName() + " intersecting: " + ability.Name + " // " + ability.GetRemainingTime(ally),
                    Debugger.Type.Intersectons);

                var abilityOwner = ability.AbilityOwner;
                var remainingTime = ability.GetRemainingTime(ally);

                foreach (var priority in ability.UseCustomPriority ? ability.Priority : Menu.Settings.DefaultPriority)
                {
                    switch (priority)
                    {
                        case EvadePriority.Walk:
                            if (allyIsMe && !ability.DisablePathfinder
                                && (Menu.Hotkeys.PathfinderMode == Pathfinder.EvadeMode.All
                                    || Menu.Hotkeys.PathfinderMode == Pathfinder.EvadeMode.Disables && ability.IsDisable
                                   ) && Hero.CanMove())
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
                                        || Pathfinder.GetIntersectingObstacles(Hero.NetworkPosition, Hero.HullRadius)
                                            .Any())
                                    {
                                        Debugger.WriteLine("moving into obstacle", Debugger.Type.Intersectons);

                                        var tempPath = Pathfinder.CalculatePathFromObstacle(
                                                Hero.Position,
                                                Hero.Position,
                                                remainingWalkTime,
                                                out success)
                                            .ToList();

                                        if (success)
                                        {
                                            if (!ability.ObstacleStays)
                                            {
                                                Pathfinder.CalculatePathFromObstacle(
                                                    remainingWalkTime - 0.3f,
                                                    out success);

                                                if (success)
                                                {
                                                    return false;
                                                }
                                            }

                                            //if (Menu.Settings.CancelAnimation && ability.IsDisable
                                            //    && Hero.Spellbook.Spells.Any(x => x.IsInAbilityPhase))
                                            //{
                                            //    Debugger.WriteLine("canceling animation", Debugger.Type.AbilityUsage);
                                            //    Hero.Stop(false, true);
                                            //}

                                            for (var i = 0; i < tempPath.Count; i++)
                                            {
                                                Hero.Move(tempPath[i], i != 0, true);
                                            }

                                            var time = Hero.Position.Distance2D(tempPath[tempPath.Count - 1])
                                                       / Hero.MovementSpeed + 0.15f;

                                            sleeper.Sleep(200, "block");
                                            Utils.Sleep(Math.Min(time, 1) * 1000, "Evader.Avoiding");
                                            sleeper.Sleep(Math.Min(time, 1) * 1000, ability);
                                            sleeper.Sleep(Math.Min(time, 1) * 1000, "avoiding");

                                            if (Menu.Settings.PathfinderEffect)
                                            {
                                                heroPathfinderEffect = new ParticleEffect(
                                                    @"particles/units/heroes/hero_oracle/oracle_fortune_purge.vpcf",
                                                    Hero);
                                            }

                                            Debugger.WriteLine(
                                                TimeSpan.FromSeconds(Game.GameTime).ToString(@"mm\:ss")
                                                + " >>>>>>>>>>>>>>>",
                                                Debugger.Type.AbilityUsage);
                                            Debugger.WriteLine(
                                                "avoid while moving: " + ability.Name,
                                                Debugger.Type.AbilityUsage);
                                            Debugger.WriteLine(
                                                "remaining time: " + remainingWalkTime,
                                                Debugger.Type.AbilityUsage);
                                            return true;
                                        }
                                    }
                                }
                                else
                                {
                                    Pathfinder.CalculatePathFromObstacle(
                                        Math.Max(remainingWalkTime - 0.25f, 0),
                                        out success);

                                    if (success)
                                    {
                                        return false;
                                    }

                                    path = Pathfinder.CalculatePathFromObstacle(remainingWalkTime, out success)
                                        .ToList();

                                    if (success)
                                    {
                                        var time = 0.15f;

                                        //if (Menu.Settings.CancelAnimation && ability.IsDisable
                                        //    && Hero.Spellbook.Spells.Any(x => x.IsInAbilityPhase))
                                        //{
                                        //    Debugger.WriteLine("canceling animation", Debugger.Type.AbilityUsage);
                                        //    Hero.Stop(false, true);
                                        //}

                                        for (var i = 0; i < path.Count; i++)
                                        {
                                            Hero.Move(path[i], i != 0, true);
                                            time += Hero.NetworkPosition.Distance2D(path[i]) / Hero.MovementSpeed;
                                        }

                                        var sleepTime = Math.Min(time, 1) * 1000;

                                        Utils.Sleep(sleepTime, "Evader.Avoiding");
                                        sleeper.Sleep(sleepTime, "avoiding");
                                        sleeper.Sleep(sleepTime, ability);

                                        if (Menu.Settings.PathfinderEffect)
                                        {
                                            heroPathfinderEffect = new ParticleEffect(
                                                @"particles/units/heroes/hero_oracle/oracle_fortune_purge.vpcf",
                                                Hero);
                                        }

                                        Debugger.WriteLine(
                                            TimeSpan.FromSeconds(Game.GameTime).ToString(@"mm\:ss")
                                            + " >>>>>>>>>>>>>>>",
                                            Debugger.Type.AbilityUsage);
                                        Debugger.WriteLine(
                                            "avoid while standing: " + ability.Name,
                                            Debugger.Type.AbilityUsage);
                                        Debugger.WriteLine(
                                            "remaining time: " + remainingTime,
                                            Debugger.Type.AbilityUsage);
                                        return true;
                                    }
                                }
                            }

                            break;
                        case EvadePriority.Blink:
                            if (!allyIsMe)
                            {
                                continue;
                            }

                            var blinkAbilities = from abilityName in ability.BlinkAbilities
                                                 join usableAbility in abilityUpdater.UsableAbilities on abilityName
                                                     equals usableAbility.Name
                                                 where usableAbility.Type == AbilityType.Blink
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

                                var requiredTime =
                                    blinkAbility.GetRequiredTime(ability, fountain, remainingTime) + 0.02f;
                                var time = remainingTime - requiredTime;
                                var ignoreRemainingTime = ability.IgnoreRemainingTime(blinkAbility, time);

                                if (requiredTime > remainingTime && !ignoreRemainingTime)
                                {
                                    continue;
                                }

                                if (time <= 0.10 && time > 0 || ignoreRemainingTime)
                                {
                                    Debugger.WriteLine(
                                        TimeSpan.FromSeconds(Game.GameTime).ToString(@"mm\:ss") + " >>>>>>>>>>>>>>>",
                                        Debugger.Type.AbilityUsage);

                                    if (!Menu.Randomiser.Enabled || Menu.Randomiser.NukesOnly && ability.IsDisable
                                        || randomiser.Next(99) > Menu.Randomiser.FailChance)
                                    {
                                        //if (Menu.Settings.CancelAnimation && ability.IsDisable && allyIsMe)
                                        //{
                                        //    Debugger.WriteLine("canceling animation", Debugger.Type.AbilityUsage);
                                        //    Hero.Stop(false, true);
                                        //}

                                        blinkAbility.Use(ability, fountain);
                                    }
                                    else
                                    {
                                        Debugger.WriteLine("you got rekt by randomiser!", Debugger.Type.AbilityUsage);
                                    }

                                    sleeper.Sleep(Math.Min(ability.GetSleepTime(), 1000), ability);

                                    Debugger.WriteLine(
                                        "blink: " + blinkAbility.Name + " => " + ability.Name,
                                        Debugger.Type.AbilityUsage);
                                    Debugger.WriteLine("ally: " + ally.GetName(), Debugger.Type.AbilityUsage);
                                    Debugger.WriteLine("remaining time: " + remainingTime, Debugger.Type.AbilityUsage);
                                    Debugger.WriteLine("required time: " + requiredTime, Debugger.Type.AbilityUsage);
                                    return true;
                                }

                                return false;
                            }

                            break;
                        case EvadePriority.Counter:
                            var counterAbilities = from abilityName in ability.CounterAbilities
                                                   join usableAbility in abilityUpdater.UsableAbilities on abilityName
                                                       equals usableAbility.Name
                                                   where usableAbility.Type == AbilityType.Counter
                                                         && Menu.UsableAbilities.Enabled(
                                                             abilityName,
                                                             AbilityType.Counter)
                                                         && (usableAbility.IsItem && heroCanUseItems
                                                             || !usableAbility.IsItem && heroCanCast)
                                                   select usableAbility;

                            foreach (var counterAbility in counterAbilities)
                            {
                                if (!Menu.Hotkeys.EnabledBkb && counterAbility.Name == "item_black_king_bar")
                                {
                                    continue;
                                }

                                var targetEnemy = !counterAbility.CanBeUsedOnAlly && counterAbility.CanBeUsedOnEnemy;

                                if (!counterAbility.CanBeUsedOnAlly && !allyIsMe && !targetEnemy
                                    || !targetEnemy && !counterAbility.CanBeCasted(ability, ally)
                                    || targetEnemy && !counterAbility.CanBeCasted(ability, abilityOwner))
                                {
                                    continue;
                                }

                                var requiredTime = counterAbility.GetRequiredTime(
                                                       ability,
                                                       targetEnemy ? abilityOwner : ally,
                                                       remainingTime) + Game.Ping / 1000 + 0.05f;

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
                                    Debugger.WriteLine(
                                        TimeSpan.FromSeconds(Game.GameTime).ToString(@"mm\:ss") + " >>>>>>>>>>>>>>>",
                                        Debugger.Type.AbilityUsage);

                                    if (!Menu.Randomiser.Enabled || Menu.Randomiser.NukesOnly && ability.IsDisable
                                        || randomiser.Next(99) > Menu.Randomiser.FailChance)
                                    {
                                        //if (Menu.Settings.CancelAnimation && ability.IsDisable && allyIsMe)
                                        //{
                                        //    Debugger.WriteLine("canceling animation", Debugger.Type.AbilityUsage);
                                        //    Hero.Stop(false, true);
                                        //}

                                        counterAbility.Use(ability, targetEnemy ? abilityOwner : ally);
                                    }
                                    else
                                    {
                                        Debugger.WriteLine("you got rekt by randomiser!", Debugger.Type.AbilityUsage);
                                    }

                                    float customSleepTime;
                                    sleeper.Sleep(
                                        counterAbility.UseCustomSleep(out customSleepTime)
                                            ? customSleepTime
                                            : ability.GetSleepTime(),
                                        ability);

                                    sleeper.Sleep(200, ability.Handle); // for modifier

                                    Debugger.WriteLine(
                                        "counter: " + counterAbility.Name + " => " + ability.Name,
                                        Debugger.Type.AbilityUsage);
                                    Debugger.WriteLine("ally: " + ally.GetName(), Debugger.Type.AbilityUsage);
                                    Debugger.WriteLine("remaining time: " + remainingTime, Debugger.Type.AbilityUsage);
                                    Debugger.WriteLine("required time: " + requiredTime, Debugger.Type.AbilityUsage);
                                    return true;
                                }

                                return false;
                            }

                            break;
                        case EvadePriority.Disable:
                            if (abilityOwner.IsMagicImmune() || abilityOwner.IsInvul())
                            {
                                continue;
                            }

                            var disableAbilities = from abilityName in ability.DisableAbilities
                                                   join usableAbility in abilityUpdater.UsableAbilities on abilityName
                                                       equals usableAbility.Name
                                                   where usableAbility.Type == AbilityType.Disable
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

                                var remainingDisableTime = ability.GetRemainingDisableTime();
                                var requiredTime =
                                    disableAbility.GetRequiredTime(ability, abilityOwner, remainingDisableTime)
                                    + Game.Ping / 1000 + 0.02f;

                                var ignoreRemainingTime = ability.IgnoreRemainingTime(
                                    disableAbility,
                                    remainingDisableTime);

                                if (requiredTime > remainingDisableTime && !ignoreRemainingTime)
                                {
                                    continue;
                                }

                                if (remainingDisableTime - requiredTime <= 0.10 || ignoreRemainingTime)
                                {
                                    Debugger.WriteLine(
                                        TimeSpan.FromSeconds(Game.GameTime).ToString(@"mm\:ss") + " >>>>>>>>>>>>>>>",
                                        Debugger.Type.AbilityUsage);

                                    if (!Menu.Randomiser.Enabled || Menu.Randomiser.NukesOnly && ability.IsDisable
                                        || randomiser.Next(99) > Menu.Randomiser.FailChance)
                                    {
                                        //if (Menu.Settings.CancelAnimation && ability.IsDisable && allyIsMe)
                                        //{
                                        //    Debugger.WriteLine("canceling animation", Debugger.Type.AbilityUsage)
                                        //    Hero.Stop(false, true);
                                        //}

                                        disableAbility.Use(ability, abilityOwner);
                                    }
                                    else
                                    {
                                        Debugger.WriteLine("you got rekt by randomiser!", Debugger.Type.AbilityUsage);
                                    }

                                    sleeper.Sleep(ability.GetSleepTime(), ability);

                                    Debugger.WriteLine(
                                        "disable: " + disableAbility.Name + " => " + ability.Name,
                                        Debugger.Type.AbilityUsage);
                                    Debugger.WriteLine("ally: " + ally.GetName(), Debugger.Type.AbilityUsage);
                                    Debugger.WriteLine(
                                        "remaining time: " + remainingDisableTime,
                                        Debugger.Type.AbilityUsage);
                                    Debugger.WriteLine("required time: " + requiredTime, Debugger.Type.AbilityUsage);
                                    return true;
                                }
                                return false;
                            }

                            break;
                    }
                }

                if (allyIsMe)
                {
                    if (Menu.UsableAbilities.Enabled(AbilityNames.GoldSpender, AbilityType.Counter))
                    {
                        var remaining = ability.GetRemainingTime(Hero) - Game.Ping / 1000;
                        if (abilityUpdater.GoldSpender.ShouldSpendGold(ability) && remaining < 0.3 && remaining > 0.1)
                        {
                            abilityUpdater.GoldSpender.Spend();
                            return true;
                        }
                    }
                    //  return false;
                }
            }
            return true;
        }
    }
}