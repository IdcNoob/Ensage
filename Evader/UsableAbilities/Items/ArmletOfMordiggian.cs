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

        private const string ArmletModifierName = "modifier_item_armlet_unholy_strength";

        private const float FullEnableTime = 0.8f;

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

            if (ability.IsDisable)
            {
                var totalDamage = (damageSource.MinimumDamage + damageSource.BonusDamage)
                                  * damageSource.SecondsPerAttack * 2;
                var totalMana = damageSource.Mana;

                foreach (var spell in damageSource.Spellbook.Spells.Where(x => x.Level > 0 && x.Cooldown <= 0))
                {
                    if (totalMana >= spell.ManaCost)
                    {
                        totalDamage += (int)Math.Round(AbilityDamage.CalculateDamage(spell, damageSource, unit));
                        totalMana -= spell.ManaCost;
                    }
                }

                return unit.Health <= totalDamage;
            }

            var damage = (int)Math.Round(AbilityDamage.CalculateDamage(ability.Ability, damageSource, unit));

            if (HpRestored(ability.GetRemainingTime(Hero)) < damage)
            {
                return false;
            }

            return unit.Health <= damage;
        }

        public void Dispose()
        {
            Game.OnUpdate -= OnUpdate;
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        public override float GetRequiredTime(EvadableAbility ability, Unit unit)
        {
            return Math.Max(ability.GetRemainingTime(Hero) - 0.1f, 0);
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            if (Hero.Modifiers.Any(x => x.Name == ArmletModifierName))
            {
                Ability.ToggleAbility();
            }
            Ability.ToggleAbility();

            Sleep(800);
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
            return Math.Min(time, FullEnableTime) * 475 / FullEnableTime;
        }

        private bool DotModifiers(ParallelQuery<Unit> nearEnemies)
        {
            var heroModifiers = Hero.HasModifiers(cantToggleArmletHeroModifiers, false);
            var enemyModifiers = nearEnemies.Any(x => x.HasModifiers(cantToggleArmletEnemyModifiers, false));

            return enemyModifiers || heroModifiers;
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!args.Entities.Contains(Hero))
            {
                return;
            }

            if (args.Ability?.ClassID == ClassID.CDOTA_Item_Armlet)
            {
                Sleep(100 + Game.Ping);
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
                        sleep += Hero.Distance2D(enemy) / (float)enemy.ProjectileSpeed();
                    }

                    attacking.Sleep(sleep, enemy);
                    attackStart.Sleep(enemy.AttacksPerSecond * 1000, enemy);
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
                    x => x?.Target is Hero && x.Source is Unit && x.Target.Equals(Hero)).ToList();

            var noProjectiles =
                heroProjectiles.All(
                    x =>
                        x.Position.Distance2D(position) / x.Speed > 0.30
                        || x.Position.Distance2D(position) / x.Speed < Game.Ping / 1000);

            var noAutoAttacks = nearEnemies.All(x => x.FindRelativeAngle(Hero.Position) > 0.5 || !attacking.Sleeping(x));

            if (Sleeper.Sleeping)
            {
                return;
            }

            if (Hero.Health < Menu.ArmetHpThreshold && ((noProjectiles && noAutoAttacks) || !armletEnabled)
                && (nearEnemies.Any() || heroProjectiles.Any()))
            {
                Use(null, null);
            }
        }

        #endregion
    }
}