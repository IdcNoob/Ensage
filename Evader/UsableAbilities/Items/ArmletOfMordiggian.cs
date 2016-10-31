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

        private const string ModifierName = "modifier_item_armlet_unholy_strength";

        #endregion

        #region Fields

        private readonly MultiSleeper attacking;

        private readonly MultiSleeper attackStart;

        #endregion

        #region Constructors and Destructors

        public ArmletOfMordiggian(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
            attacking = new MultiSleeper();
            attackStart = new MultiSleeper();

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
            if (Hero.Modifiers.Any(x => x.Name == ModifierName))
            {
                Ability.ToggleAbility();
            }
            Ability.ToggleAbility();

            Sleep(800);
        }

        #endregion

        #region Methods

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
            if (!Game.IsInGame || Game.IsPaused || !Menu.ArmletAutoToggle)
            {
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

            foreach (var enemy in nearEnemies.Where(x => x.AttackCapability == AttackCapability.Melee))
            {
                if (enemy.IsAttacking() && !attackStart.Sleeping(enemy))
                {
                    attacking.Sleep((float)UnitDatabase.GetAttackPoint(enemy) * 1000, enemy);
                    attackStart.Sleep(enemy.AttacksPerSecond * 1000 - Game.Ping, enemy);
                }
                else if (!enemy.IsAttacking() && attackStart.Sleeping(enemy))
                {
                    attackStart.Reset(enemy);
                    attacking.Sleep((float)UnitDatabase.GetAttackBackswing(enemy) * 1000, enemy);
                }
            }

            if (Sleeper.Sleeping)
            {
                return;
            }

            var heroProjectiles =
                ObjectManager.TrackingProjectiles.Where(x => x?.Target is Hero && x.Target.Equals(Hero)).ToList();

            var noProjectile =
                heroProjectiles.All(
                    x =>
                    x.Position.Distance2D(Hero) / x.Speed > 0.30
                    || x.Position.Distance2D(Hero) / x.Speed < Game.Ping / 1000);

            var noAutoAttack = nearEnemies.All(x => x.FindRelativeAngle(Hero.Position) > 0.5 || !attacking.Sleeping(x));

            if (Hero.Health < Menu.ArmetHpThreshold && noProjectile && noAutoAttack
                && (nearEnemies.Any() || heroProjectiles.Any()))
            {
                Use(null, null);
            }
        }

        #endregion
    }
}