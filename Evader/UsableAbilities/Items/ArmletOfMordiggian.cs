namespace Evader.UsableAbilities.Items
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Base;

    using Core;
    using Core.Menus;

    using Data;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.AbilityInfo;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using EvadableAbilities.Base;

    using SharpDX;

    using AbilityType = Data.AbilityType;

    internal class ArmletOfMordiggian : UsableAbility, IDisposable
    {
        private const float ArmletFullEnableTime = 0.6f;

        private const int ArmletHpGain = 470;

        private const string ArmletModifierName = "modifier_item_armlet_unholy_strength";

        private readonly Dictionary<Unit, double> attacks = new Dictionary<Unit, double>();

        private readonly Sleeper delay = new Sleeper();

        private readonly Dictionary<string, Tuple<int, float>> enemyDot = new Dictionary<string, Tuple<int, float>>
        {
            { "modifier_doom_bringer_scorched_earth_effect", Tuple.Create(625, 1f) },
            { "modifier_rattletrap_battery_assault", Tuple.Create(300, 0.7f) },
            { "modifier_leshrac_pulse_nova", Tuple.Create(475, 1f) },
            { "modifier_sandking_sand_storm", Tuple.Create(550, 0.5f) },
            { "modifier_pudge_rot", Tuple.Create(275, 0.2f) },
            { "modifier_dark_seer_ion_shell", Tuple.Create(275, 0.1f) },
            { "modifier_ember_spirit_flame_guard", Tuple.Create(425, 0.2f) },
            { "modifier_juggernaut_blade_fury", Tuple.Create(275, 0.2f) },
            { "modifier_leshrac_diabolic_edict", Tuple.Create(525, 0.25f) },
            { "modifier_phoenix_sun_ray", Tuple.Create(1325, 0.2f) },
            { "modifier_slark_dark_pact", Tuple.Create(350, 1.5f) },
            { "modifier_slark_dark_pact_pulses", Tuple.Create(350, 0.1f) },
            { "modifier_gyrocopter_rocket_barrage", Tuple.Create(425, 0.1f) }
        };

        private readonly Dictionary<string, float> initialDotTimings = new Dictionary<string, float>();

        private readonly Dictionary<string, float> selfDot = new Dictionary<string, float>
        {
            { "modifier_queenofpain_shadow_strike", 3f },
            { "modifier_crystal_maiden_frostbite", 0.5f },
            { "modifier_alchemist_acid_spray", 1f },
            { "modifier_cold_feet", 1f },
            { "modifier_arc_warden_flux", 0.5f },
            { "modifier_axe_battle_hunger", 1f },
            { "modifier_flamebreak_damage", 1f },
            { "modifier_dazzle_poison_touch", 1f },
            { "modifier_disruptor_thunder_strike", 2f },
            { "modifier_doom_bringer_infernal_blade_burn", 1f },
            { "modifier_dragon_knight_corrosive_breath_dot", 1f },
            { "modifier_earth_spirit_magnetize", 0.5f },
            { "modifier_ember_spirit_searing_chains", 1f },
            { "modifier_enigma_malefice", 2f },
            { "modifier_invoker_ice_wall_slow_debuff", 1f },
            { "modifier_invoker_chaos_meteor_burn", 0.5f },
            { "modifier_huskar_burning_spear_debuff", 1f },
            { "modifier_jakiro_dual_breath_burn", 0.5f },
            { "modifier_jakiro_liquid_fire_burn", 0.5f },
            { "modifier_meepo_geostrike_debuff", 1f },
            { "modifier_ogre_magi_ignite", 1f },
            { "modifier_phoenix_icarus_dive_burn", 1f },
            { "modifier_phoenix_fire_spirit_burn", 1f },
            { "modifier_phoenix_sun_debuff", 1f },
            { "modifier_silencer_curse_of_the_silent", 1f },
            { "modifier_silencer_last_word", 4f },
            { "modifier_sniper_shrapnel_slow", 1f },
            { "modifier_lone_druid_spirit_bear_entangle_effect", 1f },
            { "modifier_shredder_chakram_debuff", 0.5f },
            { "modifier_treant_leech_seed", 0.75f },
            { "modifier_treant_overgrowth", 1f },
            { "modifier_abyssal_underlord_firestorm_burn", 1f },
            { "modifier_venomancer_venomous_gale", 3f },
            { "modifier_venomancer_poison_sting", 1f },
            { "modifier_venomancer_poison_sting_ward", 1f },
            { "modifier_venomancer_poison_nova", 1f },
            { "modifier_viper_corrosive_skin_slow", 1f },
            { "modifier_viper_poison_attack_slow", 1f },
            { "modifier_viper_viper_strike_slow", 1f },
            { "modifier_warlock_shadow_word", 1f },
            { "modifier_weaver_swarm_debuff", 0.8f },
            { "modifier_winter_wyvern_arctic_burn_slow", 1f },
            { "modifier_maledict", 1f },
            { "modifier_skeleton_king_hellfire_blast", 1f },
            { "modifier_item_orb_of_venom_slow", 1f },
            { "modifier_item_radiance_debuff", 1f },
            { "modifier_item_urn_damage", 1f },
            { "modifier_spawnlord_master_freeze_root", 0.5f },
            { "modifier_broodmother_poison_sting_debuff", 1f },
            { "modifier_gnoll_assassin_envenomed_weapon_poison", 1f },
            { "modifier_warlock_golem_permanent_immolation_debuff", 1f },
            { "modifier_ice_blast", 0.01f }, // block
            { "modifier_necrolyte_heartstopper_aura_effect", 0.2f },
            { "modifier_crystal_maiden_freezing_field_slow", 0.1f },
            { "modifier_death_prophet_spirit_siphon_slow", 0.25f },
            { "modifier_disruptor_static_storm", 0.25f },
            { "modifier_pugna_life_drain", 0.25f },
            { "modifier_skywrath_mystic_flare_aura_effect", 0.1f },
            { "modifier_tornado_tempest_debuff", 0.25f }
        };

        private bool armletEnabled;

        private bool canToggle;

        private bool manualDisable;

        public ArmletOfMordiggian(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
            armletEnabled = Hero.Modifiers.Any(x => x.Name == ArmletModifierName);

            Game.OnUpdate += OnUpdate;
            Player.OnExecuteOrder += OnExecuteOrder;
            Drawing.OnDraw += OnDraw;
            ObjectManager.OnRemoveEntity += OnRemoveEntity;
            Entity.OnAnimationChanged += OnAnimationChanged;
            Unit.OnModifierAdded += OnModifierAdded;
            Unit.OnModifierRemoved += OnModifierRemoved;
        }

        private static DebugMenu DebugMenu => Variables.Menu.Debug;

        private static UsableAbilitiesMenu Menu => Variables.Menu.UsableAbilities;

        private float TogglePing
        {
            get
            {
                return Game.Ping / 2 + 50;
            }
        }

        public override bool CanBeCasted(EvadableAbility ability, Unit unit)
        {
            if (Sleeper.Sleeping || !Hero.CanUseItems())
            {
                return false;
            }

            if (armletEnabled && AffectedByDot())
            {
                return false;
            }

            var damageSource = ability.AbilityOwner;
            var health = (int)unit.Health;

            if (armletEnabled)
            {
                health = Math.Max(health - ArmletHpGain, 1);
            }

            //if (ability.IsDisable)
            //{
            //    var totalDamage = (damageSource.MinimumDamage + damageSource.BonusDamage)
            //                      * damageSource.AttacksPerSecond * 2;
            //    var totalMana = damageSource.Mana;

            //    foreach (var spell in damageSource.Spellbook.Spells.Where(x => x.Level > 0 && x.Cooldown <= 0))
            //    {
            //        if (totalMana >= spell.ManaCost)
            //        {
            //            totalDamage += (int)Math.Round(AbilityDamage.CalculateDamage(spell, damageSource, unit));
            //            totalMana -= spell.ManaCost;
            //        }
            //    }

            //    return health <= totalDamage;
            //}

            var damage = 150d;
            try
            {
                damage = Math.Round(AbilityDamage.CalculateDamage(ability.Ability, damageSource, unit));
            }
            catch
            {
                Console.WriteLine("[Evader] Failed to calculate damage for: " + ability.Name);
            }

            if (HpRestored(ability.GetRemainingTime(Hero) - TogglePing / 1000) + health < damage)
            {
                return false;
            }

            return health <= damage;
        }

        public void Dispose()
        {
            Game.OnUpdate -= OnUpdate;
            Player.OnExecuteOrder -= OnExecuteOrder;
            Drawing.OnDraw -= OnDraw;
            ObjectManager.OnRemoveEntity -= OnRemoveEntity;
            Entity.OnAnimationChanged -= OnAnimationChanged;
            Unit.OnModifierAdded -= OnModifierAdded;
            Unit.OnModifierRemoved -= OnModifierRemoved;
        }

        public override float GetRequiredTime(EvadableAbility ability, Unit unit, float remainingTime)
        {
            return Math.Max(Math.Min(ability.GetRemainingTime(Hero) - 0.15f, ArmletFullEnableTime), 0);
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            Sleep(ArmletFullEnableTime * 1000 + TogglePing);
            manualDisable = false;

            if (armletEnabled)
            {
                Ability.ToggleAbility(false, true);
            }

            DelayAction.Add(1, () => Ability.ToggleAbility(false, true));
        }

        public override bool UseCustomSleep(out float sleepTime)
        {
            sleepTime = 0;
            return true;
        }

        private static float HpRestored(float time)
        {
            if (time <= 0)
            {
                return 0;
            }

            return Math.Min(time, ArmletFullEnableTime) * ArmletHpGain / ArmletFullEnableTime;
        }

        private bool AffectedByDot()
        {
            var ping = Game.Ping / 1000;

            foreach (var modifier in Hero.Modifiers.Where(x => !x.IsHidden && x.IsDebuff))
            {
                float tick;
                if (!selfDot.TryGetValue(modifier.Name, out tick))
                {
                    continue;
                }

                float initialTime;
                if (!initialDotTimings.TryGetValue(modifier.Name, out initialTime))
                {
                    continue;
                }

                var elapsedTime = (Game.RawGameTime - initialTime) % tick;

                if (elapsedTime + 0.2 + ping > tick || elapsedTime + ping < 0.05)
                {
                    return true;
                }
            }

            foreach (var unit in ObjectManager.GetEntities<Unit>()
                .Where(
                    x => x.IsValid && x.IsAlive && x.IsSpawned && x.Team == Variables.EnemyTeam
                         && x.Distance2D(Hero) < 1325))
            {
                foreach (var modifier in unit.Modifiers.Where(x => !x.IsHidden))
                {
                    Tuple<int, float> tuple;
                    if (!enemyDot.TryGetValue(modifier.Name, out tuple))
                    {
                        continue;
                    }

                    var distance = tuple.Item1;
                    if (Hero.Distance2D(unit) > distance)
                    {
                        continue;
                    }

                    var tick = tuple.Item2;
                    var elapsedTime = modifier.ElapsedTime % tick;

                    if (elapsedTime + 0.2 + ping > tick || elapsedTime + ping < 0.05)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void OnAnimationChanged(Entity sender, EventArgs args)
        {
            if (sender.Team == HeroTeam
                || !sender.Animation.Name.Contains("attack") && sender.Animation.Name != "radiant_tower002")
            {
                return;
            }

            var unit = sender as Unit;
            if (unit == null)
            {
                return;
            }

            attacks[unit] = Game.RawGameTime - Game.Ping / 2000;
        }

        private void OnDraw(EventArgs args)
        {
            if (!DebugMenu.ArmletToggler)
            {
                return;
            }

            if (canToggle)
            {
                Drawing.DrawText(
                    "Can toggle",
                    "Arial",
                    HUDInfo.GetHPbarPosition(Hero) + new Vector2(10, -30),
                    new Vector2(25),
                    Color.White,
                    FontFlags.None);
            }

            foreach (var source in ObjectManager.GetEntities<Unit>()
                .Where(x => x.IsAlive && x.Distance2D(Hero) < 1000 && x.IsAttacking() && !x.Equals(Hero)))
            {
                Drawing.DrawText(
                    "Attacking " + source.AttackPoint().ToString("0.##") + " / "
                    + source.AttackBackswing().ToString("0.##"),
                    "Arial",
                    HUDInfo.GetHPbarPosition(source) + new Vector2(-15, -20),
                    new Vector2(20),
                    Color.White,
                    FontFlags.None);
            }
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!Menu.ArmletAutoToggle || !args.Entities.Contains(Hero) || args.OrderId != OrderId.ToggleAbility
                || args.Ability?.Id != AbilityId.item_armlet)
            {
                return;
            }

            if (Sleeper.Sleeping || !canToggle)
            {
                args.Process = false;
                return;
            }

            if (!args.IsPlayerInput)
            {
                return;
            }

            if (armletEnabled)
            {
                manualDisable = true;
                armletEnabled = false;
            }
            else
            {
                manualDisable = false;
                armletEnabled = true;
                Sleep(ArmletFullEnableTime * 1000);
            }
        }

        private void OnModifierAdded(Unit sender, ModifierChangedEventArgs args)
        {
            if (sender.Handle != Hero.Handle)
            {
                return;
            }

            var name = args.Modifier.Name;

            if (name == ArmletModifierName)
            {
                armletEnabled = true;
            }
            else if (name == "modifier_fountain_aura_buff" && armletEnabled && canToggle && Menu.AutoDisableAtFountain
                     && !Sleeper.Sleeping)
            {
                Ability.ToggleAbility(false, true);
            }
            else if (selfDot.ContainsKey(name))
            {
                initialDotTimings[name] = Game.RawGameTime - Game.Ping / 1000;
            }
        }

        private void OnModifierRemoved(Unit sender, ModifierChangedEventArgs args)
        {
            if (sender.Handle == Hero.Handle && args.Modifier.Name == ArmletModifierName)
            {
                armletEnabled = false;
            }
        }

        private void OnRemoveEntity(EntityEventArgs args)
        {
            var unit = args.Entity as Unit;
            if (unit != null)
            {
                attacks.Remove(unit);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (delay.Sleeping)
            {
                return;
            }

            delay.Sleep(Menu.ArmletCheckDelay);

            if (!Game.IsInGame || Game.IsPaused || !Menu.ArmletAutoToggle)
            {
                delay.Sleep(200);
                return;
            }

            if (!Hero.IsAlive || !Hero.CanUseItems() || armletEnabled && AffectedByDot())
            {
                canToggle = false;
                return;
            }

            var position = Hero.IsMoving && Math.Abs(Hero.RotationDifference) < 60
                               ? Hero.InFront(55)
                               : Hero.NetworkPosition;

            var noProjectiles = true;
            foreach (var projectile in ObjectManager.TrackingProjectiles.Where(x => x.Target?.Handle == Hero.Handle))
            {
                var unit = projectile.Source as Unit;
                if (unit == null)
                {
                    continue;
                }

                var time = Math.Max(projectile.Position.Distance2D(position) - Hero.HullRadius, 0) / projectile.Speed;
                if (time < Game.Ping / 2000)
                {
                    continue;
                }

                var hpRestored = HpRestored(time - TogglePing / 1000 - 0.12f);
                var damage = Hero.DamageTaken(unit.MaximumDamage + unit.BonusDamage, DamageType.Physical, unit) * 1.1f;

                switch (unit.NetworkName)
                {
                    case "CDOTA_Unit_Hero_Silencer":
                        if (!Hero.IsMagicImmune())
                        {
                            damage += ((Hero)unit).TotalIntelligence
                                      * (0.2f + (float)unit.Spellbook.SpellW.Level * 15 / 100);
                        }
                        break;
                    case "CDOTA_Unit_Hero_Obsidian_Destroyer":
                        if (!Hero.IsMagicImmune())
                        {
                            damage += unit.MaximumMana * (0.05f + (float)unit.Spellbook.SpellQ.Level / 100);
                        }
                        break;
                    case "CDOTA_Unit_Hero_Clinkz":
                        damage += Hero.DamageTaken(20 + unit.Spellbook.SpellW.Level * 10, DamageType.Physical, unit);
                        break;
                    case "CDOTA_Unit_Hero_Viper":
                        damage *= 2.5f;
                        break;
                }

                if (Hero.HasModifier("modifier_invoker_cold_snap"))
                {
                    var quas = ObjectManager.GetEntities<Ability>().FirstOrDefault(x => x.Id == AbilityId.invoker_quas);
                    if (quas != null)
                    {
                        damage += Hero.DamageTaken(quas.Level * 7, DamageType.Magical, unit);
                    }
                }

                if (damage >= hpRestored)
                {
                    noProjectiles = false;
                    break;
                }
            }

            var noAutoAttacks = true;
            foreach (var attack in attacks.Where(
                x => x.Key.IsAlive && x.Key.Distance2D(Hero) <= x.Key.GetAttackRange() + 200
                     && x.Key.FindRelativeAngle(Hero.Position) < 0.5
                     && (x.Key.IsMelee || x.Key.Distance2D(Hero) < 400 /*|| x.Key.AttackPoint() < 0.15*/)))
            {
                var unit = attack.Key;
                var attackStart = attack.Value - TogglePing / 1000;
                var attackPoint = unit.AttackPoint();
                var secondsPerAttack = unit.SecondsPerAttack;
                var time = Game.RawGameTime;

                var damageTime = attackStart + attackPoint + 0.12;
                if (unit.IsRanged)
                {
                    damageTime += Math.Max(unit.Distance2D(Hero) - Hero.HullRadius, 0) / unit.ProjectileSpeed();
                }

                var echoSabre = unit.FindItem("item_echo_sabre", true);

                // fuck calcus
                if ((time <= damageTime // no switch before damage
                     && (attackPoint < 0.35 // no switch if low attackpoint before attack start
                         || time + (attackPoint * 0.6) > damageTime)) // or allow switch if big attack point
                    || (attackPoint < 0.25 // dont allow switch if very low attack point 
                        && time > damageTime + (unit.AttackBackswing() * 0.6) // after attack end
                        && time <= attackStart + secondsPerAttack + 0.12) // allow if attack time passed secperatk time
                    || (echoSabre != null && unit.IsMelee // echo sabre check
                        && echoSabre.CooldownLength - echoSabre.Cooldown <= attackPoint * 2))
                {
                    noAutoAttacks = false;
                    break;
                }
            }

            canToggle = (noProjectiles && noAutoAttacks || !armletEnabled) && !Sleeper.Sleeping;

            var nearEnemies = ObjectManager.GetEntities<Unit>()
                .Any(
                    x => x.IsValid && (x.Team == Variables.EnemyTeam || x.Team == Team.Neutral) && x.IsAlive && x.IsSpawned
                         && x.Distance2D(Hero) < 800);

            if (Hero.Health < Menu.ArmletHpThreshold && canToggle
                && (nearEnemies || !Menu.ArmletEnemiesCheck && !manualDisable))
            {
                Use(null, null);
            }
        }
    }
}