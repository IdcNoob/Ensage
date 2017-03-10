namespace Evader.UsableAbilities.Items
{
    using System;
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

    using AbilityType = Data.AbilityType;

    internal class ArmletOfMordiggian : UsableAbility, IDisposable
    {
        #region Constants

        private const float ArmletFullEnableTime = 0.8f;

        private const int ArmletHpGain = 475;

        private const string ArmletModifierName = "modifier_item_armlet_unholy_strength";

        #endregion

        #region Fields

        private readonly MultiSleeper attacking;

        private readonly MultiSleeper attackStart;

        private readonly string[] cantToggleArmletEnemyModifiers =
        {
            "modifier_dark_seer_ion_shell",
            "modifier_ember_spirit_flame_guard",
            "modifier_juggernaut_blade_fury",
            "modifier_leshrac_diabolic_edict",
            "modifier_phoenix_sun_ray",
            "modifier_slark_dark_pact_pulses"
        };

        private readonly string[] cantToggleArmletHeroModifiers =
        {
            "modifier_ice_blast",
            "modifier_necrolyte_heartstopper_aura_effect",
            "modifier_pudge_rot",
            "modifier_arc_warden_flux",
            "modifier_crystal_maiden_freezing_field_slow",
            "modifier_death_prophet_spirit_siphon_slow",
            "modifier_disruptor_static_storm",
            "modifier_skywrath_mystic_flare_aura_effect",
            "modifier_tornado_tempest_debuff"
        };

        private readonly Sleeper delay;

        private bool canToggle;

        private bool manualDisable;

        #endregion

        #region Constructors and Destructors

        public ArmletOfMordiggian(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
            attacking = new MultiSleeper();
            attackStart = new MultiSleeper();
            delay = new Sleeper();

            delay.Sleep(1000);

            Game.OnUpdate += OnUpdate;
            Player.OnExecuteOrder += OnExecuteOrder;
        }

        #endregion

        #region Properties

        private static UsableAbilitiesMenu Menu => Variables.Menu.UsableAbilities;

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted(EvadableAbility ability, Unit unit)
        {
            if (Sleeper.Sleeping)
            {
                return false;
            }

            var nearEnemies =
                ObjectManager.GetEntitiesParallel<Unit>()
                    .Where(
                        x =>
                            x.IsValid && x.IsAlive && x.IsSpawned && x.AttackCapability != AttackCapability.None
                            && x.Team != HeroTeam && x.Distance2D(Hero) < x.GetAttackRange() + 300);

            var armletEnabled = Hero.Modifiers.Any(x => x.Name == ArmletModifierName);
            if (armletEnabled && DotModifiers(nearEnemies))
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

            var damage = Math.Round(AbilityDamage.CalculateDamage(ability.Ability, damageSource, unit));

            if (HpRestored(ability.GetRemainingTime(Hero)) + health < damage)
            {
                return false;
            }

            return health <= damage;
        }

        public void Dispose()
        {
            Game.OnUpdate -= OnUpdate;
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        public override float GetRequiredTime(EvadableAbility ability, Unit unit, float remainingTime)
        {
            return Math.Max(Math.Min(ability.GetRemainingTime(Hero) - 0.15f, ArmletFullEnableTime), 0);
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            if (Hero.Modifiers.Any(x => x.Name == ArmletModifierName))
            {
                Ability.ToggleAbility();
            }
            Ability.ToggleAbility();

            manualDisable = false;
            Sleep(ArmletFullEnableTime * 1000);
        }

        public override bool UseCustomSleep(out float sleepTime)
        {
            sleepTime = 0;
            return true;
        }

        #endregion

        #region Methods

        private static float HpRestored(float time)
        {
            time -= Game.Ping / 1000;

            if (time < 0)
            {
                return 0;
            }

            return Math.Min(time, ArmletFullEnableTime) * ArmletHpGain / ArmletFullEnableTime;
        }

        private bool DotModifiers(ParallelQuery<Unit> nearEnemies)
        {
            var heroModifiers = Hero.HasModifiers(cantToggleArmletHeroModifiers, false);
            var enemyModifiers = nearEnemies.Any(x => x.HasModifiers(cantToggleArmletEnemyModifiers, false));

            return enemyModifiers || heroModifiers;
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!Menu.ArmletAutoToggle || !args.Entities.Contains(Hero) || args.Order != Order.ToggleAbility
                || args.Ability?.ClassID != ClassID.CDOTA_Item_Armlet)
            {
                return;
            }

            if (Sleeper.Sleeping || !canToggle)
            {
                args.Process = false;
                return;
            }

            if (Hero.Modifiers.Any(x => x.Name == ArmletModifierName))
            {
                manualDisable = true;
                Sleep(100 + Game.Ping);
            }
            else
            {
                manualDisable = false;
                Sleep(ArmletFullEnableTime * 1000);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (delay.Sleeping)
            {
                return;
            }

            delay.Sleep(50);

            if (!Game.IsInGame || Game.IsPaused || !Menu.ArmletAutoToggle)
            {
                delay.Sleep(200);
                return;
            }

            if (!Hero.IsAlive || !Hero.CanUseItems())
            {
                return;
            }

            var nearEnemies =
                ObjectManager.GetEntitiesParallel<Unit>()
                    .Where(
                        x =>
                            x.IsValid && x.IsAlive && x.IsSpawned && x.AttackCapability != AttackCapability.None
                            && x.Team != HeroTeam && x.Distance2D(Hero) < x.GetAttackRange() + 200);

            foreach (var enemy in
                nearEnemies.Where(x => x.AttackCapability == AttackCapability.Melee || x.Distance2D(Hero) < 250))
            {
                if (!attackStart.Sleeping(enemy) && enemy.IsAttacking())
                {
                    var sleep = (float)UnitDatabase.GetAttackPoint(enemy) * 1000;

                    if (enemy.AttackCapability == AttackCapability.Ranged)
                    {
                        sleep += (Hero.Distance2D(enemy) - Hero.RingRadius) / (float)enemy.ProjectileSpeed();
                    }

                    attacking.Sleep(sleep, enemy);
                    attackStart.Sleep(enemy.SecondsPerAttack * 1000, enemy);
                }
                else if (attackStart.Sleeping(enemy) && !enemy.IsAttacking())
                {
                    attackStart.Reset(enemy);
                    attacking.Sleep((float)UnitDatabase.GetAttackBackswing(enemy) * 1000, enemy);
                }
            }

            var armletEnabled = Hero.Modifiers.Any(x => x.Name == ArmletModifierName);
            if (armletEnabled && DotModifiers(nearEnemies))
            {
                return;
            }

            var position = Hero.IsMoving && Math.Abs(Hero.RotationDifference) < 60
                               ? Hero.InFront(100)
                               : Hero.NetworkPosition;

            var heroProjectiles =
                ObjectManager.TrackingProjectiles.Where(
                    x => x.Target?.Handle == Hero.Handle && x.Source is Unit).ToList();

            var noProjectiles =
                heroProjectiles.All(
                    x =>
                        HpRestored(Math.Max(x.Position.Distance2D(position) - Hero.RingRadius, 0) / x.Speed - 0.25f)
                        > Math.Round(
                            Hero.DamageTaken(
                                ((Unit)x.Source).MinimumDamage + ((Unit)x.Source).BonusDamage,
                                DamageType.Physical,
                                (Unit)x.Source,
                                minusArmor: 4)));

            var noAutoAttacks = nearEnemies.All(x => x.FindRelativeAngle(Hero.Position) > 0.5 || !attacking.Sleeping(x));

            if (Sleeper.Sleeping)
            {
                return;
            }

            canToggle = noProjectiles && noAutoAttacks || !armletEnabled;

            if (Hero.Health < Menu.ArmetHpThreshold && canToggle
                && (nearEnemies.Any() || heroProjectiles.Any() || !Menu.ArmletEnemiesCheck && !manualDisable))
            {
                Use(null, null);
            }
        }

        #endregion
    }
}